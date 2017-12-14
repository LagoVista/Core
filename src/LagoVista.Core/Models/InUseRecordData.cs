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
