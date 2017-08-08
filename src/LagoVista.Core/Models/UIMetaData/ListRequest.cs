using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.UIMetaData
{
    public class ListRequest
    {
        public string NextPartitionKey { get; set; }
        public string NextRowKey { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
