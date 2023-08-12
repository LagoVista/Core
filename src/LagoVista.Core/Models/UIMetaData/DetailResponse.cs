using LagoVista.Core.Attributes;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using LagoVista.Core.Validation;
using LagoVista.Core.Interfaces;

namespace LagoVista.Core.Models.UIMetaData
{
    public class DetailResponse<TModel> : InvokeResult where TModel : new()
    {
        private DetailResponse() 
        { 
        
        }

        public string ModelTitle { get; set; }
        public string ModelHelp { get; set; }

        public IDictionary<string, FormField> View { get; set; }

        public List<string> FormFields { get; set; }
        public FormConditionals ConditionalFields { get; set; }

        public bool IsEditing { get; set; }

        public TModel Model { get; set; }

        public string FullClassName { get; set; }
        public string AssemblyName { get; set; }

        public static DetailResponse<TModel> Create(TModel model)
        {
            var response = new DetailResponse<TModel>();
            response.Model = model;
            response.IsEditing = true;
            response.FormFields = new List<string>();
            var viewItems = new Dictionary<string, FormField>();
            var attr = typeof(TModel).GetTypeInfo().GetCustomAttributes<EntityDescriptionAttribute>().FirstOrDefault();
            var entity = EntityDescription.Create(typeof(TModel), attr);

            if(model is IFormDescriptor)
            {
                response.FormFields = (model as IFormDescriptor).GetFormFields().Select(fld => fld.CamelCase()).ToList();
            }

            if (model is IFormConditionalFields)
            {
                var conditionalFields  = (model as IFormConditionalFields).GetConditionalFields();
                response.ConditionalFields = conditionalFields.ValuesAsCamelCase();
            }

            response.ModelTitle = entity.Title;
            response.ModelHelp = entity.UserHelp;
            response.FullClassName = model.GetType().FullName;
            response.AssemblyName = model.GetType().AssemblyQualifiedName;
       
            var properties = typeof(TModel).GetRuntimeProperties();
            foreach(var property in properties)
            {
                var fieldAttributes = property.GetCustomAttributes<FormFieldAttribute>();
                if (fieldAttributes.Any())
                {
                    var camelCaseName = property.Name.Substring(0, 1).ToLower() + property.Name.Substring(1);
                    var field = FormField.Create(camelCaseName, fieldAttributes.First(), property);

                    if (!property.GetType().GenericTypeArguments.Any())
                    {
                        var defaultValue = property.GetValue(model);
                        if (defaultValue != null)
                        {
                            field.DefaultValue = defaultValue.ToString();
                        }
                    }
                    
                    viewItems.Add(camelCaseName, field);
                }
            }
            response.View = viewItems;
            return response;
        }

        public static DetailResponse<TModel> Create()
        {
            var response = Create(new TModel());
            response.IsEditing = false;
            return response;
        }
    }
}
