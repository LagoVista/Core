// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 13f5e8ee7ee21ffbad276a940bdf457fba38e4cb49de904683456544c7c99031
// IndexVersion: 0
// --- END CODE INDEX META ---
// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using TheArtOfDev.HtmlRenderer.Adapters;


namespace TheArtOfDev.HtmlRenderer.PdfSharp.Adapters
{
    /// <summary>
    /// Adapter for WinForms brushes objects for core.
    /// </summary>
    internal sealed class BrushAdapter : RBrush
    {
        /// <summary>
        /// The actual PdfSharp brush instance.<br/>
        /// Should be <see cref="XBrush"/> but there is some fucking issue inheriting from it =/
        /// </summary>
        private readonly Object _brush;

        /// <summary>
        /// Init.
        /// </summary>
        public BrushAdapter(Object brush)
        {
            _brush = brush;
        }

        /// <summary>
        /// The actual WinForms brush instance.
        /// </summary>
        public Object Brush
        {
            get { return _brush; }
        }

        public override void Dispose()
        { }
    }
}