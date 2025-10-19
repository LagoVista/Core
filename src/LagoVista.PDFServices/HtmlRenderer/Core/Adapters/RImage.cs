// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: fd10aa05a575bc6ffe02f94ba9776515afca2d74778ff4e1e3255673c8e87940
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

namespace TheArtOfDev.HtmlRenderer.Adapters
{
    /// <summary>
    /// Adapter for platform specific image object - used to render images.
    /// </summary>
    public abstract class RImage : IDisposable
    {
        /// <summary>
        /// Get the width, in pixels, of the image.
        /// </summary>
        public abstract double Width { get; }

        /// <summary>
        /// Get the height, in pixels, of the image.
        /// </summary>
        public abstract double Height { get; }

        public abstract void Dispose();
    }
}