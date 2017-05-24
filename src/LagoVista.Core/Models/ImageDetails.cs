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
