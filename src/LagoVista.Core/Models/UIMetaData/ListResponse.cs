using LagoVista.Core.Attributes;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System;
using LagoVista.Core.Validation;

namespace LagoVista.Core.Models.UIMetaData
{
    public class ListResponse<TModel>  : InvokeResult where TModel :class
    {        
        public string Title { get; set; }
        public string Help { get; set; }

        public IEnumerable<ListColumn> Columns { get;  set; }

        public IEnumerable<TModel> Model { get;  set; }

        public int PageSize { get;  set; }
        public int PageIndex { get; set; }
        public int PageCount { get; set; }
        public string NextPartitionKey { get; set; }
        public string NextRowKey { get; set; }
        public bool HasMoreRecords { get; set; }

        public static new ListResponse<TModel> FromError(string errorMessage, string errorCode = "")
        {
            var response = new ListResponse<TModel>();
            response.Errors.Add(new ErrorMessage(errorCode, errorMessage));
            return response;
        }

        public static new ListResponse<TModel> FromErrors(params ErrorMessage[] errMessages)
        {
            var response = new ListResponse<TModel>();
            response.Errors.Concat(errMessages);
            return response;
        }

        public static ListResponse<TModel> Create(IEnumerable<TModel> model)
        {
            var response = new ListResponse<TModel>();
            response.Model = model;

            var attr = typeof(TModel).GetTypeInfo().GetCustomAttribute<EntityDescriptionAttribute>();

            if (attr != null)
            {
                var titleProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.TitleResource);
                response.Title = titleProperty != null ? titleProperty.GetValue(titleProperty.DeclaringType, null) as String : typeof(TModel).Name;

                var helpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.UserHelpResource);
                if (helpProperty != null)
                {
                    response.Help = helpProperty.GetValue(helpProperty.DeclaringType, null) as String;
                }
            }
            else
            {
                response.Title = typeof(TModel).Name;
            }

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
            response.PageSize = model.Count();

            return response;
        }
    }
}
