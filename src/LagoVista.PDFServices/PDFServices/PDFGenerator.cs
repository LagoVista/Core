using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using System.Reflection;
using SixLabors.Fonts;
using static System.Net.Mime.MediaTypeNames;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace LagoVista.PDFServices
{
    public enum Style
    {
        H1,
        H2,
        H3,
        H4,
        H5,
        H6,
        Body,
        Small,
        ColHeader,
        PageTitle,
        PageSubTitle
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
        public Margin() { }
        public Margin(double value)
        {
            Top = value;
            Bottom = value;
            Left = value;
            Right = value;
        }

        public Margin(double left, double top, double right, double bottom)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

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
        bool _addLogo;

        public Margin Margin
        {
            get; set;
        }

        public string Footer { get; set; }

        public double ColumnRightMargin { get; set; } = 10;

        public double RowBottomMargin { get; set; } = 4;

        public double? RowHeight { get; set; }

        public double ParagraphBottomMargin { get; set; } = 0;

        public double HeaderTopMargin { get; set; } = 7;
        public double HeaderBottomMargin { get; set; } = 2;

        private double? _tempYMaxHeight = null;

        private bool _showPageNumbers = true;
        
        public bool ShowPageNumbers
        {
            get => _showPageNumbers;
            set => _showPageNumbers = value;
        }

        public bool ShowPageNumbersOnTitlelPage
        {
            get; set;
        }

        public string DocumentTitle
        {
            get; set;
        }

        public string DocumentSubTitle
        {
            get; set;
        }

        public string PreparedByOrgName
        {
            get; set;
        }

        public string PreparedByUserName
        {
            get; set;
        }

        public string PreparedForOrgName
        {
            get; set;
        }

        public bool HasTItlePage
        {
            get; set;
        }

        public string NDAMessage
        {
            get
            {
                if (PreparedForOrgName == PreparedByOrgName)
                    return $"Confidential and Proprietary {PreparedByOrgName} - {DateTime.Now.Year}";

                if (!String.IsNullOrEmpty(PreparedByOrgName) && !String.IsNullOrEmpty(PreparedForOrgName))
                    return $"Confidential and Proprietary {PreparedForOrgName}/{PreparedByOrgName} - {DateTime.Now.Year}";

                return $"Confidential and Proprietary {PreparedByOrgName} - {DateTime.Now.Year}";
    }
        }

        static PDFGenerator()
        {
            GlobalFontSettings.FontResolver = new FontResolver();
        }

        private bool _pageNumberOnFirstPage = true;
        public bool PageNumbersOnTitlePage
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
            PageNumbersOnTitlePage = true;
            HasTItlePage = false;

            Margin = new Margin()
            {
                Left = 50,
                Right = 50,
                Top = 50,
                Bottom = 50,
            };
        }

        private XFont ResolveFont(Style style, XFontStyle fontStyle = XFontStyle.Regular)
        {
            switch (style)
            {
                case Style.PageTitle: return new XFont("Roboto", 32, fontStyle);
                case Style.PageSubTitle: return new XFont("Roboto", 24, fontStyle);
                case Style.H1: return new XFont("Roboto", 20, fontStyle);
                case Style.H2: return new XFont("Roboto", 18, fontStyle);
                case Style.H3: return new XFont("Roboto", 16, fontStyle);
                case Style.H4: return new XFont("Roboto", 14, fontStyle);
                case Style.H5: return new XFont("Roboto", 12, fontStyle);
                case Style.H6: return new XFont("Roboto", 10, fontStyle);
                case Style.Body: return new XFont("Roboto", 10, fontStyle);
                case Style.Small: return new XFont("Roboto", 8, fontStyle);
                case Style.ColHeader: return new XFont("Roboto", 10, fontStyle);
            }

            throw new NotSupportedException("Invalid Font Style");
        }

        private void AddNDAMessage()
        {
            if (!String.IsNullOrEmpty(NDAMessage))
            {
                var valueFont = ResolveFont(Style.Body, XFontStyle.Regular);
                var rect = new XRect(Margin.Left, 10, _currentPage.Width - (Margin.Left + Margin.Right), 0);
                _graphics.DrawString(NDAMessage, valueFont, XBrushes.Black, rect, XStringFormats.TopRight);
            }
        }

        public void RenderTitlePage()
        {
            AddLogo(300, 100, _currentPage.Height * 0.5, center: true);

            var rect = new XRect(Margin.Left, _currentPage.Height * 0.25, _currentPage.Width - (Margin.Left + Margin.Right), 0);
            if (!String.IsNullOrEmpty(DocumentTitle)) _graphics.DrawString(DocumentTitle, ResolveFont(Style.PageTitle, XFontStyle.Regular), XBrushes.Black, rect, XStringFormats.Center);
            rect = new XRect(Margin.Left, _currentPage.Height * 0.30, _currentPage.Width - (Margin.Left + Margin.Right), 0);
            if (!String.IsNullOrEmpty(DocumentSubTitle)) _graphics.DrawString(DocumentSubTitle, ResolveFont(Style.PageSubTitle, XFontStyle.Regular), XBrushes.Black, rect, XStringFormats.Center);

            if (!String.IsNullOrEmpty(PreparedForOrgName))
            {
                rect = new XRect(Margin.Left, _currentPage.Height * 0.8, _currentPage.Width - (Margin.Left + Margin.Right), 0);
                _graphics.DrawString($"Prepared For: {PreparedForOrgName}", ResolveFont(Style.H4, XFontStyle.Regular), XBrushes.Black, rect, XStringFormats.CenterRight);
            }

            if (!String.IsNullOrEmpty(PreparedByUserName))
            {
                rect = new XRect(Margin.Left, _currentPage.Height * 0.8 + 20, _currentPage.Width - (Margin.Left + Margin.Right), 0);
                _graphics.DrawString($"Prepared by: {PreparedByUserName}", ResolveFont(Style.H4, XFontStyle.Regular), XBrushes.Black, rect, XStringFormats.CenterRight);
            }

            rect = new XRect(Margin.Left, (_currentPage.Height * 0.8) + 40, _currentPage.Width - (Margin.Left + Margin.Right), 0);
            var timeStamp = $"Prepared: {DateTime.Now.ToShortDateString()}";
            _graphics.DrawString(timeStamp, ResolveFont(Style.H4, XFontStyle.Regular), XBrushes.Black, rect, XStringFormats.CenterRight);


            NewPage();
        }

        public void StartDocument(bool addLogo = true, bool addNda = true, double? width = null, double? height = null)
        {
            _addLogo = addLogo;
            _currentPage = _pdfDocument.AddPage();
            if(width.HasValue && height.HasValue)
            {
                _currentPage.Width = XUnit.FromInch(width.Value);
                _currentPage.Height = XUnit.FromInch(height.Value);
            }

            _pages.Add(_currentPage);
            _graphics = XGraphics.FromPdfPage(_currentPage);
            _textFormatter = new LGVTextServices(_graphics);
            if (addLogo && !HasTItlePage)
                AddLogo(100, 100);

            if(addNda)
                AddNDAMessage();

            if (HasTItlePage)
            {
                RenderTitlePage();
            }
            else if (addLogo)
            {
                AddLogo(100, 100);
            }

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

            AddNDAMessage();

            if (_addLogo)
                AddLogo(100, 100);
        }


        public void AddText(double left, double top, string text, Style style = Style.Body, int color = 0x000000)
        {
            var xColor = XColor.FromArgb(color);           
            var brush = new XSolidBrush(xColor);
            var labelFont = ResolveFont(style);
            var size = _graphics.MeasureString(text, labelFont);
            _graphics.DrawString(text, labelFont, brush, new XRect(XUnit.FromInch(left), XUnit.FromInch(top), size.Width, 0), XStringFormats.TopLeft);
        }

        public void AddLabelValue(String label, string value, XSolidBrush brush = null, bool horizontalAlign = false, double? labelWidth = null)
        {
            if(value == null)
            {
                throw new ArgumentNullException($"Value for Label: [{label}] is null.");
            }

            if (_renderMode == RenderMode.Table)
            {
                throw new Exception("Current render mode is table");
            }

            var width = _currentPage.Width - (Margin.Left + Margin.Right);
            var labelFont = ResolveFont(Style.Body, XFontStyle.Bold);
            var valueFont = ResolveFont(Style.Body, XFontStyle.Regular);

            if (brush == null) brush = XBrushes.Black;

            if (labelWidth == null)
                labelWidth = _graphics.MeasureStringExact(label, labelFont, width).Width;

            var labelHeight = _graphics.MeasureStringExact(label, labelFont, width).Height;
            var valueHeight = _graphics.MeasureStringExact(value, valueFont, width).Height + ParagraphBottomMargin;
            if (CurrentY + (labelHeight + valueHeight) + 50 > (_currentPage.Height - (Margin.Bottom)))
            {
                NewPage();
            }

            if (horizontalAlign)
            {
                _graphics.DrawString(label, labelFont, brush, new XRect(Margin.Left, CurrentY, labelWidth.Value, 0), XStringFormats.TopLeft);
                _graphics.DrawString(value, valueFont, brush, new XRect(labelWidth.Value, CurrentY, width - labelWidth.Value, 0), XStringFormats.TopLeft);
                CurrentY += valueHeight;
            }
            else
            {
                _graphics.DrawString(label, labelFont, brush, new XRect(Margin.Left, CurrentY, width, 0), XStringFormats.TopLeft);
                CurrentY += labelHeight;
                _graphics.DrawString(value, valueFont, brush, new XRect(Margin.Left, CurrentY, width, 0), XStringFormats.TopLeft);
                CurrentY += valueHeight;
            }

            CurrentY += 5;
        }

        public void AddClickableLink(string name, string link, string description = "")
        {
            var fullPageWidth = _currentPage.Width - (Margin.Left + Margin.Right);
            var valueFont = ResolveFont(Style.Body, XFontStyle.Regular);
            var linkFont = ResolveFont(Style.Body, XFontStyle.Underline);
            var linkBrush = XBrushes.Blue;
            var valueSize = _graphics.MeasureStringExact(name, linkFont, fullPageWidth);
            var valueWidth = valueSize.Width;
            var valueHeight = valueSize.Height;

            var rect = new XRect(Margin.Left, CurrentY, valueWidth, valueHeight);
            var pdfRect = _graphics.Transformer.WorldToDefaultPage(rect);

            var url = link.ToLower().StartsWith("http") ? link : $"https://{link}";
            _currentPage.AddWebLink(new PdfRectangle(pdfRect), url);
            _graphics.DrawString(name, linkFont, linkBrush, rect, XStringFormats.TopLeft);
            var labelSize = _graphics.MeasureStringExact(name, valueFont, fullPageWidth);
            CurrentY += valueHeight;

            if (!String.IsNullOrEmpty(description))
            {
                var descriptionSize = _graphics.MeasureStringExact(description, valueFont, fullPageWidth);
                var descriptionWidth = descriptionSize.Width;
                var descriptionHeight = descriptionSize.Height;
                Console.WriteLine($" {descriptionWidth} - {descriptionHeight}");

                var brush = XBrushes.Black;
                var descriptionRect = new XRect(Margin.Left, CurrentY, descriptionWidth, descriptionHeight);
                _textFormatter.DrawString(description, valueFont, brush, descriptionRect, XStringFormats.TopLeft);

                CurrentY += descriptionHeight + ParagraphBottomMargin;
            }
        }

        public void AddClickableLink(string link)
        {
            var fullPageWidth = _currentPage.Width - (Margin.Left + Margin.Right);
            var linkFont = ResolveFont(Style.Body, XFontStyle.Underline);
            var linkBrush = XBrushes.Blue;
            var valueSize = _graphics.MeasureStringExact(link, linkFont, fullPageWidth);
            var valueWidth = valueSize.Width;
            var valueHeight = valueSize.Height;

            var rect = new XRect(Margin.Left, CurrentY, valueWidth, valueHeight);
            var pdfRect = _graphics.Transformer.WorldToDefaultPage(rect);

            var url = link.ToLower().StartsWith("http") ? link : $"https://{link}";
            _currentPage.AddWebLink(new PdfRectangle(pdfRect), url);
            _graphics.DrawString(link, linkFont, linkBrush, rect, XStringFormats.TopLeft);
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

            if(!text.EndsWith('\n'))
            {
                text += "\r\n";
            }

            if (align == null) align = XStringFormats.TopLeft;
            if (!width.HasValue) width = _currentPage.Width;
            if (brush == null) brush = XBrushes.Black;
            var font = ResolveFont(Style.Body, fontStyle);
            var height = _graphics.MeasureStringExact(text, font, width.Value - (Margin.Left + Margin.Right)).Height + ParagraphBottomMargin;
            if (CurrentY + (height) + 50 > (_currentPage.Height - (Margin.Bottom)))
            {
                NewPage();
            }

            _textFormatter.DrawString(text, font, brush, new XRect(Margin.Left, CurrentY, _currentPage.Width - (Margin.Left + Margin.Right), height), align);
            CurrentY += height;
        }

        public void AddLine(String text, double? width = null, XSolidBrush brush = null, XStringFormat align = null, XFontStyle fontStyle = XFontStyle.Regular)
        {
            if (String.IsNullOrEmpty(text))
                return;

            if (_renderMode == RenderMode.Table)
            {
                throw new Exception("Current render mode is table");
            }


            if (align == null) align = XStringFormats.TopLeft;
            if (!width.HasValue) width = _currentPage.Width;
            if (brush == null) brush = XBrushes.Black;
            var font = ResolveFont(Style.Body, fontStyle);
            var height = _graphics.MeasureStringExact(text, font, width.Value - (Margin.Left + Margin.Right)).Height;
            if (CurrentY + (height) + 50 > (_currentPage.Height - (Margin.Bottom)))
            {
                NewPage();
            }

            _textFormatter.DrawString(text, font, brush, new XRect(Margin.Left, CurrentY, _currentPage.Width - (Margin.Left + Margin.Right), height), align);
            CurrentY += height;
        }


        public void AddParagraph(String text, string label, double? width = null, XSolidBrush brush = null, XStringFormat align = null, XFontStyle fontStyle = XFontStyle.Regular)
        {
            if (String.IsNullOrEmpty(text))
            {
                return;
            }

            if (!text.EndsWith('\n'))
            {
                text += "\r\n";
            }

            text = text.Replace("\n", "\n\n");

            if (_renderMode == RenderMode.Table)
            {
                throw new Exception("Current render mode is table");
            }

            if (align == null) align = XStringFormats.TopLeft;
            if (!width.HasValue) width = _currentPage.Width;
            if (brush == null) brush = XBrushes.Black;
            var font = ResolveFont(Style.Body, fontStyle);
            var labelFont = ResolveFont(Style.Body, XFontStyle.Bold);
            var labelHeight = _graphics.MeasureStringExact(label, labelFont, width.Value - (Margin.Left + Margin.Right)).Height;
            var height = _graphics.MeasureStringExact(text, font, width.Value - (Margin.Left + Margin.Right)).Height + ParagraphBottomMargin;

            if (CurrentY + (labelHeight + height) + 50 > (_currentPage.Height - (Margin.Bottom)))
            {
                NewPage();
            }

            _graphics.DrawString(label, labelFont, brush, new XRect(Margin.Left, CurrentY, width.Value, 0), XStringFormats.TopLeft);
            CurrentY += labelHeight;
            _textFormatter.DrawString(text, font, brush, new XRect(Margin.Left, CurrentY, _currentPage.Width - (Margin.Left + Margin.Right), height), align);
            CurrentY += height;
        }

        public byte[] GetFont(string imageName)
        {
            using (var ms = new MemoryStream())
            {
                var assembly = typeof(FontResolver).GetTypeInfo().Assembly;
                var fullName = $"{assembly.FullName}.images.{imageName}";
                var resources = assembly.GetManifestResourceNames();
                var resourceName = resources.First(x => x == imageName);
                using (var rs = assembly.GetManifestResourceStream(resourceName))
                {
                    rs.CopyTo(ms);
                    ms.Position = 0;
                    return ms.ToArray();
                }
            }
        }

        public void AddLogo(int maxWidth, int maxHeight, double? top = null, double? left = null, bool? center = false)
        {
            var imageName = "sllogo.png";
            using (var ms = new MemoryStream())
            {
                var assembly = typeof(FontResolver).GetTypeInfo().Assembly;
                var fullName = $"{assembly.GetName().Name}.images.{imageName}";
                var resources = assembly.GetManifestResourceNames();
                var resourceName = resources.SingleOrDefault(x => x == imageName);
                if (resourceName == null)
                {
                    //   throw new ArgumentOutOfRangeException("Could not find resource: " + fullName);
                }

                if (!top.HasValue) top = 10;
                if (!left.HasValue) left = Margin.Left;


                using (var rs = assembly.GetManifestResourceStream(fullName))
                {
                    rs.CopyTo(ms);
                    ms.Position = 0;
                    using (var img = XImage.FromStream(() => ms))
                    {
                        var scalingFactor = img.PixelWidth > img.PixelHeight ? (float)maxWidth / (float)img.PixelWidth : (float)maxHeight / (float)img.PixelHeight;

                        if (center.HasValue && center.Value)
                        {
                            left = (_currentPage.Width / 2) - ((img.PointWidth * scalingFactor) / 2);
                        }

                        _graphics.DrawImage(img, left.Value, top.Value, img.PointWidth * scalingFactor, img.PointHeight * scalingFactor);
                    }
                }
            }
        }


        public void AddLogoToHeader(string imageName, int maxWidth, int maxHeight)
        {
            using (var ms = new MemoryStream())
            {
                var assembly = typeof(FontResolver).GetTypeInfo().Assembly;
                var fullName = $"{assembly.FullName}.images.{imageName}";
                var resources = assembly.GetManifestResourceNames();
                var resourceName = resources.First(x => x == imageName);
                using (var rs = assembly.GetManifestResourceStream(resourceName))
                {
                    rs.CopyTo(ms);
                    ms.Position = 0;
                    using (var img = XImage.FromStream(() => ms))
                    {
                        var scalingFactor = img.PixelWidth > img.PixelHeight ? (float)maxWidth / (float)img.PixelWidth : (float)maxHeight / (float)img.PixelHeight;
                        _graphics.DrawImage(img, Margin.Left, CurrentY, img.PointWidth * scalingFactor, img.PointHeight * scalingFactor);
                        CurrentY += img.PointHeight * scalingFactor;
                    }
                }
            }
        }

        public void AddImage(MemoryStream ms, int maxWidth, int maxHeight)
        {
            using (var img = XImage.FromStream(() => ms))
            {
                var scalingFactor = img.PixelWidth > img.PixelHeight ? (float)maxWidth / (float)img.PixelWidth : (float)maxHeight / (float)img.PixelHeight;
                _graphics.DrawImage(img, Margin.Left, CurrentY, img.PointWidth * scalingFactor, img.PointHeight * scalingFactor);
                CurrentY += img.PointHeight * scalingFactor;
            }
        }

        public void AddColImage(int colIdx, MemoryStream ms, int maxWidth, int maxHeight, XStringFormat align = null)
        {
            using (var img = XImage.FromStream(() => ms))
            {
                double left = Margin.Left;
                for (var idx = 0; idx < colIdx; ++idx)
                {
                    left += _colWidths[idx].AppliedWidth + ColumnRightMargin;
                }

                double top = CurrentY;

                var width = _colWidths[colIdx].AppliedWidth;
                var height = RowHeight.HasValue ? RowHeight.Value : img.PixelHeight;

                var scalingFactor = img.PixelWidth > img.PixelHeight ? (float)maxWidth / (float)img.PixelWidth : (float)maxHeight / (float)img.PixelHeight;

                switch(align.LineAlignment)
                {
                    case XLineAlignment.Center:
                        top += (height - (img.PointHeight * scalingFactor)) / 2;
                        break;
                    case XLineAlignment.BaseLine:
                        
                        break;
                    case XLineAlignment.Far:
                        top += height - ((img.PointHeight * scalingFactor));
                        break;
                }

                switch(align.Alignment)
                {
                    case XStringAlignment.Center:
                        left += (width - (img.PointWidth * scalingFactor)) / 2;
                        break;
                    case XStringAlignment.Near:
                        left += 5 * scalingFactor;
                        break;
                    case XStringAlignment.Far:
                        left += width - ((img.PointWidth * scalingFactor));
                        break;

                }

                _graphics.DrawImage(img, left, top, img.PointWidth * scalingFactor, img.PointHeight * scalingFactor);
            }
        }

        public void AddImage(double left, double top, double width, double height, MemoryStream ms)
        {
            using (var img = XImage.FromStream(() => ms))
            {
                var scalingFactor = img.PixelWidth > img.PixelHeight ? (float)XUnit.FromInch(width) / (float)img.PixelWidth : (float)XUnit.FromInch(height) / (float)img.PixelHeight;
                _graphics.DrawImage(img, XUnit.FromInch(left), XUnit.FromInch(top), img.PointWidth * scalingFactor, img.PointHeight * scalingFactor);
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
            var height = RowHeight.HasValue ? RowHeight.Value : _graphics.MeasureStringExact(text, font, width).Height;

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

            if(RowHeight.HasValue)
                _tempYMaxHeight = Math.Max(_tempYMaxHeight.Value, RowHeight.Value);
            else
                _tempYMaxHeight = Math.Max(_tempYMaxHeight.Value, height);
        }

        public void StartTable(params ColWidth[] widths)
        {
            if (_currentPage == null)
                throw new Exception("Must start page before starting table.");

            _colWidths = widths;

            _renderMode = RenderMode.Table;

            var starCols = _colWidths.Where(col => col.WidthType == WidthTypes.Star);
            var usedWidth = _colWidths.Where(col => col.WidthType == WidthTypes.Points).Sum(cl => (cl.RequestedWidth + ColumnRightMargin));
            var reamining = (_currentPage.Width - (Margin.Left + Margin.Right)) - (usedWidth + ((_colWidths.Length - 1) * ColumnRightMargin));
            if (starCols.Count() > 0)
            {
                var starWidth = Convert.ToDouble(reamining / starCols.Count());
                foreach (var starCol in starCols)
                {
                    starCol.AppliedWidth = starWidth;
                } 
            }
        }

        public void DrawHorizontalLine(int color, int thickness = 1, int? width = null)
        {
            var pen = new XPen(XColor.FromArgb(color), thickness);

            var lineWidth = width.HasValue ? width.Value : _currentPage.Width - (Margin.Left + Margin.Right);

            _graphics.DrawLine(pen, Margin.Left, CurrentY, _currentPage.Width - Margin.Right, CurrentY);
            CurrentY += 5;
        }

        public void StartRow(int rowIndex = 0)
        {
            if (rowIndex > 0 && !RowHeight.HasValue)
                throw new ArgumentNullException("To skip rows, you must populate RowHeight");
            else if(RowHeight > 0)
                CurrentY += (RowHeight.Value + RowBottomMargin) * rowIndex;

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
        
        public void AddHtml(string html)
        {
            //PdfGenerator.GeneratePdf()
        }

        private void RenderPageNumbers()
        {
            var idx = 1;
            foreach (var page in _pages)
            {
                if (HasTItlePage && !ShowPageNumbersOnTitlelPage && idx == 1)
                {
                    idx++;
                    continue;
                }

                if (_showPageNumbers && _pageIndex > 0 || _pageNumberOnFirstPage)
                {
                    var centerRect = new XRect(Margin.Left, page.Height - 34, _currentPage.Width - (Margin.Left + Margin.Right), 34);
                    var font = ResolveFont(Style.Body);
                    using (var gx = XGraphics.FromPdfPage(page))
                    {
                        gx.DrawLine(XPens.DarkGray, Margin.Left, page.Height - 35, _currentPage.Width - Margin.Left, page.Height - 35);
                        var pageMessage = HasTItlePage && !ShowPageNumbersOnTitlelPage ? $"Page {idx - 1} of {_pages.Count - 1}" : $"Page {idx} of {_pages.Count}";
                        if (!String.IsNullOrEmpty(Footer))
                        {
                            gx.DrawString(Footer, font, XBrushes.Black, centerRect, XStringFormats.CenterLeft);
                            gx.DrawString(pageMessage, font, XBrushes.Black, centerRect, XStringFormats.CenterRight);
                        }
                        else
                        {
                            gx.DrawString(pageMessage, font, XBrushes.Black, centerRect, XStringFormats.Center);
                        }
                    }
                    idx++;
                }
            }
        }

        public void Write(String fileName, bool renderPageNumbers = true)
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }

            if(renderPageNumbers && ShowPageNumbers)
                RenderPageNumbers();

            _pdfDocument.Save(fileName);
        }

        public void Write(Stream fileName, bool renderPageNumbers = true)
        {
            if (_graphics != null)
            {
                _graphics.Dispose();
                _graphics = null;
            }

            if(renderPageNumbers && ShowPageNumbers)
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
