// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b3c776f7129d780a10942320ab4164b1dc4ef35bd9898d9a19f5d3a00550d3e1
// IndexVersion: 0
// --- END CODE INDEX META ---
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models.UIMetaData
{
    public enum OrderByTypes
    {
        Name,
        Rating,
        CreationDate,
        LastUpdateDate,
    }


    public class ListRequest
    {
        public const int MaxPageSize = 9999999;

        public string NextPartitionKey { get; set; }
        public string NextRowKey { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string GroupBy { get; set; }
        public string GroupByType { get; set; }
        public int GroupBySize { get; set; }
        public string Url { get; set; }

        public string CategoryKey { get; set; }

        public double TimeBucketSize { get; set; } 
        public string TimeBucket { get; set; }
        public bool ShowDrafts { get; set; }
        public bool ShowDeleted { get; set; }

        public OrderByTypes? OrderBy { get; set; }
        public OrderByTypes? OrderByDesc { get; set; }
         
        public override string ToString()
        {
            return $@"[List Request]
\tNext Parition Key: {NextPartitionKey}
\tNext Row Key     : {NextRowKey}
\tPage Index       : {PageIndex}
\tPage Size        : {PageSize}
\tStart Date       : {StartDate}
\tEnd Date         : {EndDate}
\tTime Bucket      : {TimeBucket}
\tTime Bucket Size : {TimeBucketSize}
\tGroup By         : {GroupBy}
\tGroup By Type    : {GroupByType}
\tGroup By Size    : {GroupBySize}
\tUrl              : {Url}";
        }

        public static ListRequest CreateForAll()
        {
            return new ListRequest()
            {
                PageIndex = 1,
                PageSize = MaxPageSize
            };
        }

        public static ListRequest Create(int pageIndex = 1, int pageSize = ListRequest.MaxPageSize)
        {
            return new ListRequest()
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
