﻿using LagoVista.Core.Attributes;
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
        public List<string> FormFieldsCol2 { get; set; }
        public List<string> FormFieldsAdvanced { get; set; }
        public List<string> FormFieldsAdvancedCol2 { get; set; }
        public List<string> FormFieldsSimple { get; set; }
        public List<string> FormFieldsBottom { get; set; }
        public List<string> FormFieldsTabs { get; set; }

        public FormConditionals ConditionalFields { get; set; }
        public List<FormAdditionalAction> FormAdditionalActions { get; set; }

        public string Icon { get; set; }

        public bool IsEditing { get; set; }

        public string UpdateUrl { get; set; }

        public string InsertUrl { get; set; }
        public string FactoryUrl { get; set; }

        public string GetUrl { get; set; }
        public string DeleteUrl { get; set; }
        public string GetListUrl { get; set; }
        public string HelpUrl { get; set; }

        private string _saveUrl;
        public string SaveUrl
        {
            get { return _saveUrl; }
            set
            {
                _saveUrl = value;
                if (string.IsNullOrEmpty((UpdateUrl)))
                    UpdateUrl = value;

                if(string.IsNullOrEmpty(InsertUrl))
                    InsertUrl = value;
            }
        }

        public TModel Model { get; set; }

        public string FullClassName { get; set; }
        public string AssemblyName { get; set; }

        public static DetailResponse<TModel> Create(TModel model, bool isEditing = true)
        {
            var response = new DetailResponse<TModel>();
            response.Model = model;
            response.IsEditing = isEditing;
            response.FormFields = new List<string>();
            var viewItems = new Dictionary<string, FormField>();
            var attr = typeof(TModel).GetTypeInfo().GetCustomAttributes<EntityDescriptionAttribute>().FirstOrDefault();
            var entity = EntityDescription.Create(typeof(TModel), attr);

            if(model is IFormDescriptor)
            {
                response.FormFields = (model as IFormDescriptor).GetFormFields().Select(fld => fld.CamelCase()).ToList();
            }

            if (model is IFormDescriptorCol2)
            {
                response.FormFieldsCol2 = (model as IFormDescriptorCol2).GetFormFieldsCol2().Select(fld => fld.CamelCase()).ToList();
            }

            if (model is IFormDescriptorAdvanced)
            {
                response.FormFieldsAdvanced = (model as IFormDescriptorAdvanced).GetAdvancedFields().Select(fld => fld.CamelCase()).ToList();
            }

            if (model is IFormDescriptorAdvancedCol2)
            {
                response.FormFieldsAdvancedCol2 = (model as IFormDescriptorAdvancedCol2).GetAdvancedFieldsCol2().Select(fld => fld.CamelCase()).ToList();
            }

            if (model is IFormDescriptorSimple)
            {
                response.FormFieldsSimple = (model as IFormDescriptorSimple).GetSimpleFields().Select(fld => fld.CamelCase()).ToList();
            }

            if (model is IFormDescriptorBottom)
            {
                response.FormFieldsBottom = (model as IFormDescriptorBottom).GetFormFieldsBottom().Select(fld => fld.CamelCase()).ToList();
            }

            if (model is IFormDescriptorTabs)
            {
                response.FormFieldsTabs = (model as IFormDescriptorTabs).GetFormFieldsTabs().Select(fld => fld.CamelCase()).ToList();
            }

            if (model is IFormAdditionalActions)
            {
                response.FormAdditionalActions = (model as IFormAdditionalActions).GetAdditionalActions();
            }

            if (model is IFormConditionalFields)
            {
                var conditionalFields  = (model as IFormConditionalFields).GetConditionalFields();
                response.ConditionalFields = conditionalFields.ValuesAsCamelCase();
            }

            response.ModelTitle = entity.Title;
            response.Icon = entity.Icon;
            response.ModelHelp = entity.UserHelp;
            response.FullClassName = model.GetType().FullName;
            response.AssemblyName = model.GetType().AssemblyQualifiedName;
            if (!string.IsNullOrEmpty(entity.InsertUrl))
                response.InsertUrl = entity.InsertUrl;

            if (!string.IsNullOrEmpty(entity.UpdateUrl))
                response.UpdateUrl = entity.UpdateUrl;

            response.SaveUrl = entity.SaveUrl;
            response.FactoryUrl = entity.FactoryUrl;
            response.GetUrl = entity.GetUrl;
            response.DeleteUrl = entity.DeleteUrl;
            response.GetListUrl = entity.GetListUrl;
            response.HelpUrl = entity.HelpUrl;

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
