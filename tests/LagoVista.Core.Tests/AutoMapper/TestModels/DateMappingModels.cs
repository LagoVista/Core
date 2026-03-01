using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.AutoMapper.TestModels
{
    public class DateMapping
    {
        public UtcTimestamp? TheDate { get; set; }
        public CalendarDate? TheDateOnly { get; set; }
    }

    public class DateMappingDTO
    {
        public DateTime? TheDate { get; set; }
        public DateOnly? TheDateOnly { get; set; }
    }
}
