using LagoVista.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Models.UIMetaData
{
    public class ListHeader
    {
        public ListHeader() { }

        public String Caption { get; set; }
        public String FieldName { get; set; }
        public bool Sortable { get; set; }
        public bool Hidden { get; set; }

        public static ListHeader Create(FormFieldAttribute attr)
        {
            var field = new ListHeader();

            return field;
        }

    }
}
