// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 920d9153462e38289bfdc90d16457aa0ba88633976992b8b39b9b7b8fb360ce1
// IndexVersion: 2
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

using TheArtOfDev.HtmlRenderer.Adapters.Entities;

namespace TheArtOfDev.HtmlRenderer.Adapters
{
    /// <summary>
    /// Adapter for platform specific pen objects - used to draw graphics (lines, rectangles and paths) 
    /// </summary>
    public abstract class RPen
    {
        /// <summary>
        /// Gets or sets the width of this Pen, in units of the Graphics object used for drawing.
        /// </summary>
        public abstract double Width { get; set; }

        /// <summary>
        /// Gets or sets the style used for dashed lines drawn with this Pen.
        /// </summary>
        public abstract RDashStyle DashStyle { set; }
    }
}