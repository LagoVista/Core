#region PDFsharp - A .NET library for processing PDF
//
// Code was loosely based off of:
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


using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;

namespace LagoVista.PDFServices
{
    public partial class LGVTextServices
    {
        /// <summary>
        /// Represents a single word.
        /// </summary>
        public class Block
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Block"/> class.
            /// </summary>
            /// <param name="text">The text of the block.</param>
            /// <param name="type">The type of the block.</param>
            /// <param name="width">The width of the text.</param>
            public Block(string text, BlockType type, double width)
            {
                Text = text;
                Type = type;
                Width = width;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Block"/> class.
            /// </summary>
            /// <param name="type">The type.</param>
            public Block(BlockType type)
            {
                Type = type;
            }

            /// <summary>
            /// The text represented by this block.
            /// </summary>
            public string Text;

            /// <summary>
            /// The type of the block.
            /// </summary>
            public BlockType Type;

            /// <summary>
            /// The width of the text.
            /// </summary>
            public double Width;

            /// <summary>
            /// The location relative to the upper left corner of the layout rectangle.
            /// </summary>
            public XPoint Location;

            /// <summary>
            /// The alignment of this line.
            /// </summary>
            public XParagraphAlignment Alignment;

            /// <summary>
            /// A flag indicating that this is the last bock that fits in the layout rectangle.
            /// </summary>
            public bool Stop;
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