using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using PdfSharpCore.Drawing;

namespace LagoVista.PDFServices
{
    public static class PDFHelpers
    {

        /// <summary>
        /// Enhanced measure string function for PdfSharp Xgraphics 
        /// wich take to account lineBreaks to calculate the real string 
        /// height in a rectagle
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="text">Text to measure</param>
        /// <param name="maxWitdh">Maximum allowed width</param>
        /// <returns></returns>
        public static XSize MeasureStringExact(this XGraphics gfx, string content, XFont font, double maxWidth)
        {
            //Split by hard line break
            var contents = content.Split(new List<string> { "\n", "\r\n" }.ToArray(), StringSplitOptions.None);

            Func<string, XFont, XSize> measPdfSharp = (ct, ft) =>
            {
                var size = gfx.MeasureString(ct, ft);

                var lineSpace = font.GetHeight();
                var cellSpace = font.FontFamily.GetLineSpacing(ft.Style);
                var cellLeading = cellSpace - font.FontFamily.GetCellAscent(ft.Style) - font.FontFamily.GetCellDescent(ft.Style);
                var leading = lineSpace * cellLeading / cellSpace;

                size.Height += leading;

                if (size.Width > maxWidth)
                {
                    var nbLine = gfx.GetSplittedLineCount(ct, ft, maxWidth);

                    size.Height = (nbLine * lineSpace) + leading;
                    size.Width = maxWidth;
                }

                return size;
            };

            var sizes = contents.Select(c => measPdfSharp(c, font));

            return new XSize(sizes.Max(w => w.Width), sizes.Sum(h => h.Height));
        }

        /// <summary>
        /// Calculate the number of soft line breaks
        /// </summary>
        /// <param name="gfx"></param>
        /// <param name="content"></param>
        /// <param name="font"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        private static int GetSplittedLineCount(this XGraphics gfx, string content, XFont font, double maxWidth)
        {
            //handy function for creating list of string
            Func<string, IList<string>> listFor = val => new List<string> { val };
            // string.IsNullOrEmpty is too long :p
            Func<string, bool> nOe = str => string.IsNullOrEmpty(str);
            // return a space for an empty string (sIe = Space if Empty)
            Func<string, string> sIe = str => nOe(str) ? " " : str;
            // check if we can fit a text in the maxWidth
            Func<string, string, bool> canFitText = (t1, t2) => gfx.MeasureString($"{(nOe(t1) ? "" : $"{t1} ")}{sIe(t2)}", font).Width <= maxWidth;

            Func<IList<string>, string, IList<string>> appendtoLast =
                (list, val) => list.Take(list.Count - 1)
                                   .Concat(listFor($"{(nOe(list.Last()) ? "" : $"{list.Last()} ")}{sIe(val)}"))
                                   .ToList();

            var splitted = content.Split(' ');

            var lines = splitted.Aggregate(listFor(""),
                (lfeed, next) => canFitText(lfeed.Last(), next) ? appendtoLast(lfeed, next) : lfeed.Concat(listFor(next)).ToList(),
                list => list.Count());

            return lines;
        }
    }

}