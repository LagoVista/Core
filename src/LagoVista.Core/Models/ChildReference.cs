using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class ChildReference : EntityHeader
    {
        [JsonProperty("_t")]
        public new string Indexer
        {
            get; set;
        } = "chref";

        public string Path { get; set; }
    }
}
