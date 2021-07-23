using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;

namespace LagoVista.PDFServices
{
    public enum Style
    {
        H1,
        H2,
        H3,
        H4,
        H5,
        Body,
        ColHeader
    }

    public enum WidthTypes
    {
        Star,
        Points
    }

    public class ColWidth
    {
        public static ColWidth Create(double width)
        {
            var col = new ColWidth();
            col.RequestedWidth = width;
            col.WidthType = WidthTypes.Points;
            col.AppliedWidth = width;
            return col;
        }

        public static ColWidth CreateStar(int units = 1)
        {
            var col = new ColWidth();
            col.Units = units;
            col.WidthType = WidthTypes.Star;
            return col;
        }

        public double AppliedWidth { get; internal set; }
        public int Units { get; internal set; }
        public double RequestedWidth { get; private set; }
        public WidthTypes WidthType { get; private set; }
    }


    public class Margin
    {
        public double Top { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }
        public double Right { get; set; }
    }

    public class PDFGenerator : IDisposable
    {
        enum RenderMode
        {
            Paragraph,
            Table
        }

        RenderMode _renderMode = RenderMode.Paragraph;

        PdfDocument _pdfDocument;
        PdfPage _currentPage;
        XGraphics _graphics;
        LGVTextServices _textFormatter;
        ColWidth[] _colWidths;

        public Margin Margin
        {
            get; set;
        }

        public double ColumnRightMargin { get; set; } = 10;

        public double RowBottomMargin { get; set; } = 8;

        public double ParagraphBottomMargin { get; set; } = 8;

        public double HeaderTopMargin { get; set; } = 7;
        public double HeaderBottomMargin { get; set; } = 2;

        private double? _tempYMaxHeight = null;

        private bool _showPageNumbers = true;
        public bool ShowPageNumbers
        {
            get => _showPageNumbers;
            set => _showPageNumbers = value;
        }


        static PDFGenerator()
        {
            GlobalFontSettings.FontResolver = new FontResolver();
        }

        private bool _pageNumberOnFirstPage = true;
        public bool PageNumberOnFirstPage
        {
            get => _pageNumberOnFirstPage;
            set => _pageNumberOnFirstPage = value;
        }

        private List<PdfPage> _pages = new List<PdfPage>();

        private double _currentY;
        private double CurrentY
        {
            get { return _currentY; }
            set
            {
                _currentY = value;
                if (_currentY > (_currentPage.Height - Margin.Bottom))
                {
                    _pageIndex++;

                    _currentPage = _pdfDocument.AddPage();
                    _pages.Add(_currentPage);
                    _graphics.Dispose();

                    _currentY = Margin.Top;
                    _graphics = XGraphics.FromPdfPage(_currentPage);
                    _textFormatter = new LGVTextServices(_graphics);
                }
            }
        }

        private void AddMargin(double margin)
        {
            //Margins are ALWAYS added to the current Y, if CurrentY is zero, never add the margin to the top of the page.
            if (CurrentY > Margin.Top)
            {
                CurrentY += margin;
            }
        }

        public PDFGenerator()
        {
            _pdfDocument = new PdfDocument();

            Margin = new Margin()
            {
                Left = 24,
                Right = 36,
                Top = 36,
                Bottom = 36,
            };
        }

        private XFont ResolveFont(Style style, XFontStyle fontStyle = XFontStyle.Regular)
        {
            switch (style)
            {
                case Style.H1: return new XFont("Roboto", 22, fontStyle);
                case Style.H2: return new XFont("Roboto", 20, fontStyle);
                case Style.H3: return new XFont("Roboto", 18, fontStyle);
                case Style.H4: return new XFont("Roboto", 16, fontStyle);
                case Style.H5: return new XFont("Roboto", 14, fontStyle);
                case Style.Body: return new XFont("Roboto", 10, fontStyle);
                case Style.ColHeader: return new XFont("Roboto", 10, fontStyle);
            }

            throw new NotSupportedException("Invalid Font Style");
        }

        public void StartDocument()
        {
            _currentPage = _pdfDocument.AddPage();
            _pages.Add(_currentPage);
            _graphics = XGraphics.FromPdfPage(_currentPage);
            _textFormatter = new LGVTextServices(_graphics);
            CurrentY = Margin.Top;
        }

        private int _pageIndex = 1;

        public void NewPage()
        {
            _pageIndex++;
            _currentY = Margin.Top;
            _currentPage = _pdfDocument.AddPage();
            _pages.Add(_currentPage);
            _graphics.Dispose();

            _graphics = XGraphics.FromPdfPage(_currentPage);
            _textFormatter = new LGVTextServices(_graphics);
        }

        public void AddLabelValue(String label, string value, XSolidBrush brush = null)
        {
            if (_renderMode == RenderMode.Table)
            {
                throw new Exception("Current render mode is table");
            }

            var width = _currentPage.Width - (Margin.Left + Margin.Right);
            var labelFont = ResolveFont(Style.Body, XFontStyle.Bold);
            var valueFont = ResolveFont(Style.Body, XFontStyle.Regular);

            if (brush == null) brush = XBrushes.Black;

            var labelHeight = _graphics.MeasureStringExact(label, labelFont, width).Height;
            var valueHeight = _graphics.MeasureStringExact(label, valueFont, width).Height + ParagraphBottomMargin;
            if (CurrentY + (labelHeight + valueHeight) + 50 > (_currentPage.Height - (Margin.Bottom)))
            {
                NewPage();
            }

            _graphics.DrawString(label, labelFont, brush, new XRect(Margin.Left, CurrentY, width, 0), XStringFormats.TopLeft);
            CurrentY += labelHeight;
            _graphics.DrawString(value, valueFont, brush, new XRect(Margin.Left, CurrentY, width, 0), XStringFormats.TopLeft);
            CurrentY += valueHeight;
        }

        public void AddHeader(Style style, String text, double? width = null, XSolidBrush brush = null, XStringFormat align = null, XFontStyle fontStyle = XFontStyle.Regular)
        {
            if (_renderMode == RenderMode.Table)
            {
                throw new Exception("Current render mode is table");
            }

            if (style != Style.Body) AddMargin(HeaderTopMargin);

            if (align == null) align = XStringFormats.TopLeft;
            if (!width.HasValue) width = _currentPage.Width - (Margin.Left + Margin.Right);
            if (brush == null) brush = XBrushes.Black;
            var font = ResolveFont(style, fontStyle);

            var headerHeight = _graphics.MeasureStringExact(text, font, width.Value).Height + ((style != Style.Body) ? HeaderBottomMargin : ParagraphBottomMargin);
            if (CurrentY + headerHeight + 50 > (_currentPage.Height - (Margin.Bottom)))
            {
                NewPage();
            }

            _graphics.DrawString(text, font, brush, new XRect(Margin.Left, CurrentY, width.Value, 0), align);

            CurrentY += headerHeight;
        }

        public void AddParagraph(String text, double? width = null, XSolidBrush brush = null, XStringFormat align = null, XFontStyle fontStyle = XFontStyle.Regular)
        {
            if (_renderMode == RenderMode.Table)
            {
                throw new Exception("Current render mode is table");
            }

            if (align == null) align = XStringFormats.TopLeft;
            if (!width.HasValue) width = _currentPage.Width;
            if (brush == null) brush = XBrushes.Black;
            var font = ResolveFont(Style.Body, fontStyle);
            var height = _graphics.MeasureStringExact(text, font, width.Value).Height + ParagraphBottomMargin;
            if (CurrentY + (height) + 50 > (_currentPage.Height - (Margin.Bottom)))
            {
                NewPage();
            }

            _textFormatter.DrawString(text, font, brush, new XRect(Margin.Left, CurrentY, _currentPage.Width - (Margin.Left + Margin.Right), height), align);
            CurrentY += height;
        }

        public void AddParagraph(String text, string label, double? width = null, XSolidBrush brush = null, XStringFormat align = null, XFontStyle fontStyle = XFontStyle.Regular)
        {
            if (_renderMode == RenderMode.Table)
            {
                throw new Exception("Current render mode is table");
            }

            if (align == null) align = XStringFormats.TopLeft;
            if (!width.HasValue) width = _currentPage.Width;
            if (brush == null) brush = XBrushes.Black;
            var font = ResolveFont(Style.Body, fontStyle);
            var labelFont = ResolveFont(Style.Body, XFontStyle.Bold);
            var labelHeight = _graphics.MeasureStringExact(label, labelFont, width.Value).Height;
            var height = _graphics.MeasureStringExact(text, font, width.Value).Height + ParagraphBottomMargin;

            if (CurrentY + (labelHeight + height) + 50 > (_currentPage.Height - (Margin.Bottom)))
            {
                NewPage();
            }

            _graphics.DrawString(label, labelFont, brush, new XRect(Margin.Left, CurrentY, width.Value, 0), XStringFormats.TopLeft);
            CurrentY += labelHeight;
            _textFormatter.DrawString(text, font, brush, new XRect(Margin.Left, CurrentY, _currentPage.Width - (Margin.Left + Margin.Right), height), align);
            CurrentY += height;
        }

        public void AddImage(MemoryStream ms, int maxWidth, int maxHeight)
        {
            using (var img = XImage.FromStream(() => ms))
            {
                Console.WriteLine("IMAGE HEIGHT: " + img.PointHeight.ToString());
                _graphics.DrawImage(img, Margin.Left, CurrentY, img.PixelWidth, img.PointHeight);
                CurrentY += img.PixelHeight;
            }
        }

        public void AddColText(Style style, int colIdx, String text, XSolidBrush brush = null, XStringFormat align = null, XFontStyle fontStyle = XFontStyle.Regular)
        {
            if (_renderMode == RenderMode.Paragraph)
            {
                throw new Exception("Current render mode is paragraph");
            }

            if (_colWidths == null || _colWidths.Length == 0)
            {
                throw new Exception("Col Widths not set");
            }

            if (colIdx - 1 > _colWidths.Length)
            {
                throw new Exception("Provided column index is not in column width array");
            }

            if (!_tempYMaxHeight.HasValue)
            {
                throw new Exception("Please call StartRow with column widths before adding a row.");
            }

            double leftMargin = Margin.Left;
            for (var idx = 0; idx < colIdx; ++idx)
            {
                leftMargin += _colWidths[idx].AppliedWidth + ColumnRightMargin;
            }

            double width = _colWidths[colIdx].AppliedWidth;

            if (align == null) align = XStringFormats.TopLeft;
            if (brush == null) brush = XBrushes.Black;
            var font = ResolveFont(style, fontStyle);
            var height = _graphics.MeasureStringExact(text, font, width).Height;

            if (style == Style.ColHeader)
            {
                var colHeaderFont = new XFont("Roboto", 10, XFontStyle.Bold);
                _graphics.DrawString(text, colHeaderFont, brush, new XRect(leftMargin, CurrentY, width, height), XStringFormats.Center);
            }
            else if (align.LineAlignment == XLineAlignment.Near &&
                align.Alignment == XStringAlignment.Near)
            {
                _textFormatter.DrawString(text, font, brush, new XRect(leftMargin, CurrentY, width, height), align);
            }
            else
            {
                _graphics.DrawString(text, font, brush, new XRect(leftMargin, CurrentY, width, height), align);
            }

            _tempYMaxHeight = Math.Max(_tempYMaxHeight.Value, height);
        }

        public void StartTable(params ColWidth[] widths)
        {
            _colWidths = widths;

            _renderMode = RenderMode.Table;

            var starCols = _colWidths.Where(col => col.WidthType == WidthTypes.Star);
            var usedWidth = _colWidths.Where(col => col.WidthType == WidthTypes.Points).Sum(cl => (cl.RequestedWidth + ColumnRightMargin));
            var reamining = (_currentPage.Width - (Margin.Left + Margin.Right)) - usedWidth;
            if (starCols.Count() > 0)
            {
                var starWidth = Convert.ToDouble(reamining / starCols.Count());
                foreach (var starCol in starCols)
                {
                    starCol.AppliedWidth = starWidth;
                }
            }
        }


        public void StartRow()
        {
            _tempYMaxHeight = 0;
        }

        public void EndRow()
        {
            CurrentY += _tempYMaxHeight.Value + RowBottomMargin;
            _tempYMaxHeight = 0;
        }

        public void EndTable()
        {
            _renderMode = RenderMode.Paragraph;
            _colWidths = null;
            CurrentY += 10;
        }

        public void AddWhiteSpace(double y)
        {
            CurrentY += y;
        }

        private void RenderPageNumbers()
        {
            var idx = 1;
            foreach (var page in _pages)
            {
                if (_showPageNumbers && _pageIndex > 0 || _pageNumberOnFirstPage)
                {
                    var centerRect = new XRect(0, page.Height - 22, _currentPage.Width, 22);
                    var font = ResolveFont(Style.Body);
                    using (var gx = XGraphics.FromPdfPage(page))
                    {
                        var pageMessage = $"Page {idx++} of {_pages.Count}";
                        gx.DrawString(pageMessage, font, XBrushes.Black, centerRect, XStringFormats.Center);
                    }
                }
            }
        }

        public void Write(String fileName)
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }

            RenderPageNumbers();
            _pdfDocument.Save(fileName);
        }

        public void Write(Stream fileName)
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }

            RenderPageNumbers();
            _pdfDocument.Save(fileName);
        }

        public void Dispose()
        {
            if (_pdfDocument != null)
            {
                _pdfDocument.Dispose();
                _pdfDocument = null;
            }

            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }
        }
    }
}
