using LagoVista.Core.Attributes;
using System;
using System.Reflection;

namespace LagoVista.Core.Models.UIMetaData
{
    public class ListColumn
    {
        private ListColumn() { }

        public String Header { get; set; }
        public String FieldName { get; set; }
        public String Alignment { get; set; }
        public bool Sortable { get; set; }
        public bool Visible { get; set; }
        public String FormatString { get; set; }
        public String Help { get; set; }

        public static ListColumn Create(String name, ListColumnAttribute attr)
        {
            var field = new ListColumn();

            if (!String.IsNullOrEmpty(attr.HeaderResource))
            {
                if(attr.ResourceType == null)
                {
                    throw new NullReferenceException("Missing Resource Type on Property " + name);
                }
                var headerProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.HeaderResource);
                field.Header = (string)headerProperty.GetValue(headerProperty.DeclaringType, null);
            }
            else
            {
                field.Header = name;
            }
            
            if(!String.IsNullOrEmpty(attr.HelpResource))
            {
                if (attr.ResourceType == null)
                {
                    throw new NullReferenceException("Missing Resource Type on Property " + name);
                }

                var helpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.HelpResource);
                field.Help = (string)helpProperty.GetValue(helpProperty.DeclaringType, null);
            }

            field.FieldName = name;
            field.Alignment = attr.Alignment.ToString();
            field.Sortable = attr.Sortable;
            field.Visible = attr.Visible;
            field.FormatString = attr.FormatString;
            return field;
        }

    }
}
