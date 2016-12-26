using LagoVista.Core.Attributes;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace LagoVista.Core.Models.UIMetaData
{
    public class DetailResponse<TModel> where TModel : new()
    {
        private DetailResponse() { }

        public string Title { get; set; }
        public string Help { get; set; }

        public IDictionary<string, FormField> View { get; set; }

        public TModel Model { get; set; }

        public static DetailResponse<TModel> Create(TModel model)
        {
            var response = new DetailResponse<TModel>();
            response.Model = model;
            var viewItems = new Dictionary<string, FormField>();

            var properties = typeof(TModel).GetTypeInfo().DeclaredProperties;
            foreach(var property in properties)
            {
                var fieldAttributes = property.GetCustomAttributes<FormFieldAttribute>();
                if (fieldAttributes.Any())
                {
                    viewItems.Add(property.Name.ToLower(), FormField.Create(property.Name.ToLower(), fieldAttributes.First()));
                }
            }
            response.View = viewItems;
            return response;
        }

        public static DetailResponse<TModel> Create()
        {
            return Create(new TModel());
        }

    }
}
