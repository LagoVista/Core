using LagoVista.Core.Attributes;
using System.Reflection;
using System.Collections.Generic;


namespace LagoVista.Core.Models.UIMetaData
{
    public class ListResponse<TModel> where TModel :class
    {
        private ListResponse() { }

        public IEnumerable<FormFieldAttribute> View { get; private set; }

        public IEnumerable<TModel> Model { get; private set; }

        public int Top { get; private set; }
        public int RowCount { get; private set; }

        public static ListResponse<TModel> Create(IEnumerable<TModel> model)
        {
            var response = new ListResponse<TModel>();
            response.Model = model;
            response.View = typeof(TModel).GetTypeInfo().GetCustomAttributes<FormFieldAttribute>();
            return response;
        }
    }
}
