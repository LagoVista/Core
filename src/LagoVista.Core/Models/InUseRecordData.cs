// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 340a951c857bd204389461175a316145422939b9407acaa25e5a48361970b16f
// IndexVersion: 2
// --- END CODE INDEX META ---
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class InUseRecordData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("EntityType")]
        public string RecordType { get; set; }
        public string Uri { get; set; }
    }
}
