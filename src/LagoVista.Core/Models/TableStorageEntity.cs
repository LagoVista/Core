using LagoVista.Core.PlatformSupport;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Models
{
    public class TableStorageEntity
    {
        public TableStorageEntity()
        {
        
        }
        
        public String RowKey { get; set; }
        public String PartitionKey { get; set; }
        [JsonProperty("odata.etag", NullValueHandling = NullValueHandling.Ignore)]
        public String ETag { get; set; }
        [JsonProperty("odata.metadata",NullValueHandling =NullValueHandling.Ignore)]
        public String MetaData { get; set; }
   }
}
