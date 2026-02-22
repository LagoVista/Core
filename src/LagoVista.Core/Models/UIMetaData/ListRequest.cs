// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b3c776f7129d780a10942320ab4164b1dc4ef35bd9898d9a19f5d3a00550d3e1
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private static readonly string[] DateFormats = new string[] { "yyyy/MM/dd", "yyyy-MM-dd" };

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

        public bool HasCursor => !string.IsNullOrEmpty(NextPartitionKey) && !string.IsNullOrEmpty(NextRowKey);

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

        public bool TryGetDateRange(out DateTime? startInclusive, out DateTime? endExclusive, out string? error)
        {
            startInclusive = null;
            endExclusive = null;
            error = null;

            if (!string.IsNullOrWhiteSpace(StartDate))
            {
                if (!TryParseDate(StartDate, out var s))
                {
                    error = $"Invalid StartDate '{StartDate}'. Expected yyyy/MM/dd or yyyy-MM-dd.";
                    return false;
                }

                startInclusive = s;
            }

            if (!string.IsNullOrWhiteSpace(EndDate))
            {
                if (!TryParseDate(EndDate, out var eInclusive))
                {
                    error = $"Invalid EndDate '{EndDate}'. Expected yyyy/MM/dd or yyyy-MM-dd.";
                    return false;
                }

                endExclusive = eInclusive.AddDays(1);
            }

            if (startInclusive.HasValue &&
                endExclusive.HasValue &&
                endExclusive.Value < startInclusive.Value)
            {
                error = "EndDate must be greater than or equal to StartDate.";
                return false;
            }

            return true;
        }

        private static bool TryParseDate(string s, out DateTime date)
        {
            if (DateTime.TryParseExact(
                    s.Trim(),
                    DateFormats,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out var parsed))
            {
                // Normalize to midnight, Kind = Unspecified
                date = new DateTime(parsed.Year, parsed.Month, parsed.Day, 0, 0, 0, DateTimeKind.Unspecified);
                return true;
            }

            date = default;
            return false;
        }


        public InvokeResult<ListRequestRelationalFilters> CreateRelationalFilters()
        {
            var result = new InvokeResult<ListRequestRelationalFilters>();
            if (!DateTime.TryParse(StartDate, out var start))
            {
                result.Errors.Add(new ErrorMessage() { Message = "Start date is not a valid date time" });
            }
            if (!DateTime.TryParse(EndDate, out var end))
            {
                result.Errors.Add(new ErrorMessage() { Message = "End date is not a valid date time" });
            }
            if (result.Successful)
            {
                result.Result = new ListRequestRelationalFilters()
                {
                    Start = start,
                    End = end,
                    Skip = (PageIndex - 1) * PageSize,
                    Take = PageSize
                };
            }
            return result;
        } 
    }

    public class ListRequestRelationalFilters
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
    }
}
