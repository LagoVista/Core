using LagoVista.Core.Attributes;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;

namespace LagoVista.Core.Models.UIMetaData
{
    public class ListResponse<TModel> where TModel :class
    {
        private ListResponse() { }

        public string Title { get; set; }
        public string Help { get; set; }

        public IEnumerable<ListColumn> Columns { get; private set; }

        public IEnumerable<TModel> Model { get; private set; }

        public int Top { get; private set; }
        public int RowCount { get; private set; }

        public static ListResponse<TModel> Create(IEnumerable<TModel> model)
        {
            var response = new ListResponse<TModel>();
            response.Model = model;

            var attr = typeof(TModel).GetTypeInfo().GetCustomAttribute<EntityDescriptionAttribute>();

            var titleProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.TitleResource);
            response.Title = titleProperty.GetValue(titleProperty.DeclaringType, null) as String;

            var helpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.UserHelpResource);
            response.Help = titleProperty.GetValue(titleProperty.DeclaringType, null) as String;

            var columns = new List<ListColumn>();
            var properties = typeof(TModel).GetRuntimeProperties();
            foreach (var property in properties)
            {
                var fieldAttributes = property.GetCustomAttributes<ListColumnAttribute>();
                if (fieldAttributes.Any())
                {
                    columns.Add(ListColumn.Create(property.Name.ToLower(), fieldAttributes.First()));
                }
            }

            response.Columns = columns;

            return response;
        }
    }
}
