using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LagoVista.Core.Models.UIMetaData
{
    public class FormConditional
    {
        public FormConditional()
        {
            VisibleFields = new List<string>();
            RequiredFields = new List<string>();
        }

        public bool ForCreate { get; set; } = true;
        public bool ForUpdate { get; set; } = true;

        public bool NotEquals { get; set; } = false;
        public string Field { get; set; }
        public string Value { get; set; }
        public List<string> VisibleFields { get; set; }
        public List<string> RequiredFields { get; set; }

        public FormConditional ValuesAsCamelCase()
        {
            var conditional = new FormConditional();
            conditional.Field = Field?.CamelCase();
            conditional.Value = Value?.CamelCase();
            conditional.ForCreate = ForCreate;
            conditional.ForUpdate = ForUpdate;
            conditional.NotEquals = NotEquals;

            conditional.VisibleFields = VisibleFields.Select(fld => fld.CamelCase()).ToList();
            conditional.RequiredFields = RequiredFields.Select(fld => fld.CamelCase()).ToList();
            return conditional;
        }
    }

    public class FormConditionals
    {
        public FormConditionals()
        {
            ConditionalFields = new List<string>();
            Conditionals = new List<FormConditional>();
        }

        public List<string> ConditionalFields { get; set; }
    
        public List<FormConditional> Conditionals { get; set; }

        public FormConditionals ValuesAsCamelCase()
        {
            var conditionals = new FormConditionals();
            conditionals.ConditionalFields = ConditionalFields.Select(fld => fld.CamelCase()).ToList();
            conditionals.Conditionals = Conditionals.Select(fld=>fld.ValuesAsCamelCase()).ToList();    
            return conditionals;
        }
    }
}
