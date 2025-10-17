// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: fe0845073cd54135658dff9b42d3359fc11826bf34d89e7e62a846f6709abc2a
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class ImageDetails
    {
        [JsonProperty("id")]
        public String Id { get; set; }
        public String ImageUrl { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
