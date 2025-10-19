// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7937630a83fb3267122d732316e632902344e1dfde987a1b278c24be135a0493
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

namespace TheArtOfDev.HtmlRenderer.Core.Entities
{
    /// <summary>
    /// Controls the way styles are generated when html is generated.
    /// </summary>
    public enum HtmlGenerationStyle
    {
        /// <summary>
        /// styles are not generated at all
        /// </summary>
        None = 0,

        /// <summary>
        /// style are inserted in style attribute for each html tag
        /// </summary>
        Inline = 1,

        /// <summary>
        /// style section is generated in the head of the html
        /// </summary>
        InHeader = 2
    }
}