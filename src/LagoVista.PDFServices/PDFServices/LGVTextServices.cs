// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c6f4a236f43b194607d4553e83aa906954c5999e6f6aa74984a3cc517c6fb244
// IndexVersion: 0
// --- END CODE INDEX META ---
#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange (mailto:Stefan.Lange@pdfsharp.com)
//
// Copyright (c) 2005-2009 empira Software GmbH, Cologne (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System.Text;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf.IO;

namespace LagoVista.PDFServices
{
    /// <summary>
    /// Represents a very simple text formatter.
    /// If this class does not satisfy your needs on formatting paragraphs I recommend to take a look
    /// at MigraDoc Foundation. Alternatively you should copy this class in your own source code and modify it.
    /// </summary>
    public partial class LGVTextServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XTextFormatter"/> class.
        /// </summary>
        public LGVTextServices(XGraphics gfx)
        {
            if (gfx == null)
                throw new ArgumentNullException("gfx");
            this.gfx = gfx;
        }
        XGraphics gfx;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
        string text;

        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        public XFont Font
        {
            get { return this.font; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("font");
                this.font = value;

                this.lineSpace = font.GetHeight(); //GetHeight(this.gfx);
                // HACK - don't know enough about fonts to get correct word spacing so just make it smaller.
                this.cyAscent = lineSpace * font.CellAscent / font.CellSpace;
                this.cyDescent = lineSpace * font.CellAscent / font.CellSpace;

                // HACK in XTextFormatter
                this.spaceWidth = gfx.MeasureString("x x", value).Width;
                this.spaceWidth -= gfx.MeasureString("xx", value).Width;
            }
        }
        XFont font;
        double lineSpace;
        double cyAscent;
        double cyDescent;
        double spaceWidth;

        /// <summary>
        /// Gets or sets the bounding box of the layout.
        /// </summary>
        public XRect LayoutRectangle
        {
            get { return this.layoutRectangle; }
            set { this.layoutRectangle = value; }
        }
        XRect layoutRectangle;

        /// <summary>
        /// Gets or sets the alignment of the text.
        /// </summary>
        public XParagraphAlignment Alignment
        {
            get { return this.alignment; }
            set { this.alignment = value; }
        }
        XParagraphAlignment alignment = XParagraphAlignment.Left;

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle)
        {
            DrawString(text, font, brush, layoutRectangle, XStringFormats.TopLeft);
        }

        /// <summary>
        /// Draws the text.
        /// </summary>
        /// <param name="text">The text to be drawn.</param>
        /// <param name="font">The font.</param>
        /// <param name="brush">The text brush.</param>
        /// <param name="layoutRectangle">The layout rectangle.</param>
        /// <param name="format">The format. Must be <c>XStringFormat.TopLeft</c></param>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (font == null)
                throw new ArgumentNullException("font");
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (format.Alignment != XStringAlignment.Near || format.LineAlignment != XLineAlignment.Near)
                throw new ArgumentException("Only TopLeft alignment is currently implemented.");

            Text = text;
            Font = font;
            LayoutRectangle = layoutRectangle;

            if (text.Length == 0)
                return;

            CreateBlocks();

            CreateLayout();

            double dx = layoutRectangle.Location.X;
            double dy = layoutRectangle.Location.Y + cyAscent;
            int count = this.blocks.Count;
            for (int idx = 0; idx < count; idx++)
            {
                var block = (Block)this.blocks[idx];
                if (block.Stop)
                {
                    break;

                }
                else if (block.Type == BlockType.LineBreak)
                {
                    continue;
                }
                else
                {
                    gfx.DrawString(block.Text, font, brush, dx + block.Location.X, dy + block.Location.Y);
                }
            }
        }

        void CreateBlocks()
        {
            this.blocks.Clear();
            int length = this.text.Length;
            bool inNonWhiteSpace = false;
            int startIndex = 0, blockLength = 0;
            for (int idx = 0; idx < length; idx++)
            {
                char ch = text[idx];

                // Treat CR and CRLF as LF
                if (ch == Chars.CR)
                {
                    if (idx < length - 1 && text[idx + 1] == Chars.LF)
                        idx++;
                    ch = Chars.LF;
                }
                if (ch == Chars.LF)
                {
                    if (blockLength != 0)
                    {
                        string token = text.Substring(startIndex, blockLength);
                        this.blocks.Add(new Block(token, BlockType.Text,
                          this.gfx.MeasureString(token, this.font).Width));
                    }
                    startIndex = idx + 1;
                    blockLength = 0;
                    this.blocks.Add(new Block(BlockType.LineBreak));
                }
                else if (Char.IsWhiteSpace(ch))
                {
                    if (inNonWhiteSpace)
                    {
                        string token = text.Substring(startIndex, blockLength);
                        this.blocks.Add(new Block(token, BlockType.Text,
                          this.gfx.MeasureString(token, this.font).Width));
                        startIndex = idx + 1;
                        blockLength = 0;
                    }
                    else
                    {
                        blockLength++;
                    }
                }
                else
                {
                    inNonWhiteSpace = true;
                    blockLength++;
                }
            }
            if (blockLength != 0)
            {
                string token = text.Substring(startIndex, blockLength);
                this.blocks.Add(new Block(token, BlockType.Text,
                  this.gfx.MeasureString(token, this.font).Width));
            }
        }

        void CreateLayout()
        {
            double rectWidth = this.layoutRectangle.Width;
            double rectHeight = this.layoutRectangle.Height - this.cyAscent - this.cyDescent;
            int firstIndex = 0;
            double x = 0, y = 0;
            int count = this.blocks.Count;
            for (int idx = 0; idx < count; idx++)
            {
                Block block = (Block)this.blocks[idx];
                if (block.Type == BlockType.LineBreak)
                {
                    if (Alignment == XParagraphAlignment.Justify)
                        ((Block)this.blocks[firstIndex]).Alignment = XParagraphAlignment.Left;
                    AlignLine(firstIndex, idx - 1, rectWidth);
                    firstIndex = idx + 1;
                    x = 0;
                    y += this.lineSpace;
                }
                else
                {
                    double width = block.Width; //!!!modTHHO 19.11.09 don't add this.spaceWidth here
                    if ((x + width <= rectWidth || x == 0) && block.Type != BlockType.LineBreak)
                    {
                        block.Location = new XPoint(x, y);
                        x += width + spaceWidth; //!!!modTHHO 19.11.09 add this.spaceWidth here
                    }
                    else
                    {
                        AlignLine(firstIndex, idx - 1, rectWidth);
                        firstIndex = idx;
                        y += lineSpace;
                        block.Location = new XPoint(0, y);
                        x = width + spaceWidth; //!!!modTHHO 19.11.09 add this.spaceWidth here
                    }
                }
            }
            if (firstIndex < count && Alignment != XParagraphAlignment.Justify)
                AlignLine(firstIndex, count - 1, rectWidth);
        }

        /// <summary>
        /// Align center, right or justify.
        /// </summary>
        void AlignLine(int firstIndex, int lastIndex, double layoutWidth)
        {
            XParagraphAlignment blockAlignment = ((Block)(this.blocks[firstIndex])).Alignment;
            if (this.alignment == XParagraphAlignment.Left || blockAlignment == XParagraphAlignment.Left)
                return;

            int count = lastIndex - firstIndex + 1;
            if (count == 0)
                return;

            double totalWidth = -this.spaceWidth;
            for (int idx = firstIndex; idx <= lastIndex; idx++)
                totalWidth += ((Block)(this.blocks[idx])).Width + this.spaceWidth;

            double dx = Math.Max(layoutWidth - totalWidth, 0);
            //Debug.Assert(dx >= 0);
            if (this.alignment != XParagraphAlignment.Justify)
            {
                if (this.alignment == XParagraphAlignment.Center)
                    dx /= 2;
                for (int idx = firstIndex; idx <= lastIndex; idx++)
                {
                    Block block = (Block)this.blocks[idx];
                    block.Location += new XSize(dx, 0);
                }
            }
            else if (count > 1) // case: justify
            {
                dx /= count - 1;
                for (int idx = firstIndex + 1, i = 1; idx <= lastIndex; idx++, i++)
                {
                    Block block = (Block)this.blocks[idx];
                    block.Location += new XSize(dx * i, 0);
                }
            }
        }

        readonly List<Block> blocks = new List<Block>();

        public enum BlockType
        {
            Text, Space, Hyphen, LineBreak,
        }
        // TODO:
        // - more XStringFormat variations
        // - calculate bounding box
        // - left and right indent
        // - first line indent
        // - margins and paddings
        // - background color
        // - text background color
        // - border style
        // - hyphens, soft hyphens, hyphenation
        // - kerning
        // - change font, size, text color etc.
        // - line spacing
        // - underine and strike-out variation
        // - super- and sub-script
        // - ...
    }
}