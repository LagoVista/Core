// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 753c8ed08eb0c06350ebf12cf6572d88a5306eaff89d61afd2288f4a3a5bdc4d
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using static LagoVista.Core.Attributes.EntityDescriptionAttribute;

namespace LagoVista.Core.Models.UIMetaData
{
    public class EntityDescription
    {
        public static EntityDescription Create(Type entityType, EntityDescriptionAttribute attr)
        {
            var entityDescription = new EntityDescription();
            entityDescription.Elements = new List<FormField>();
            entityDescription.ListColumns = new List<ListColumn>();
            entityDescription.SaveUrl = attr.SaveUrl;
            entityDescription.InsertUrl = attr.InsertUrl;
            entityDescription.UpdateUrl = attr.UpdateUrl;
            entityDescription.FactoryUrl = attr.FactoryUrl;
            entityDescription.GetUrl = attr.GetUrl;
            entityDescription.GetListUrl = attr.GetListUrl;
            entityDescription.HelpUrl = attr.HelpUrl;
            entityDescription.Icon = attr.Icon;
            entityDescription.ListUIUrl = attr.ListUIUrl;
            entityDescription.EditUIUrl =  attr.EditUIUrl;
            entityDescription.CreateUIUrl = attr.CreateUIUrl;
            entityDescription.PreviewUIUrl = attr.PreviewUIUrl;
            entityDescription.Col1WidthPercent = attr.Col1WidthPercent;
            entityDescription.Col2WidthPercent = attr.Col2WidthPercent;
            entityDescription.CanExport = attr.CanExport;
            entityDescription.CanImport = attr.CanImport;
            entityDescription.AutoSave = attr.AutoSave;
            entityDescription.AutoSaveIntervalSeconds = attr.AutoSaveIntervalSeconds;

            if (String.IsNullOrEmpty(entityDescription.UpdateUrl))
                entityDescription.UpdateUrl = attr.SaveUrl;

            if (String.IsNullOrEmpty(entityDescription.InsertUrl))
                entityDescription.InsertUrl = attr.SaveUrl;

            var properties = entityType.GetRuntimeProperties();
            foreach (var property in properties)
            {
                var fieldAttributes = property.GetCustomAttributes<FormFieldAttribute>();
                if (fieldAttributes.Any())
                {
                    try
                    {
                        entityDescription.Elements.Add(FormField.Create(property.Name, fieldAttributes.First(), property));
                    }
                    catch(Exception ex)
                    {
                        throw new Exception($"Could not create form field for field: {entityType.FullName}/{property.Name} - {ex.Message}");
                    }
                 }

                var listAttributes = property.GetCustomAttributes<ListColumnAttribute>();
                if(listAttributes.Any())
                {
                    entityDescription.ListColumns.Add(ListColumn.Create(property.Name, listAttributes.First()));
                }
            }
           
            entityDescription.Name = entityType.FullName;

            var titleProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.TitleResource);
            if (titleProperty != null)
            {
                entityDescription.Title = titleProperty.GetValue(titleProperty.DeclaringType, null) as String;
            }
            else
            {
                entityDescription.Title = entityType.Name;
            }

            var descriptionProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.DescriptionResource);
            if (descriptionProperty != null)
            {
                entityDescription.Description = descriptionProperty.GetValue(descriptionProperty.DeclaringType, null) as String;
            }

            var userHelpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.UserHelpResource);
            if (userHelpProperty != null)
            {
                entityDescription.UserHelp = userHelpProperty.GetValue(userHelpProperty.DeclaringType, null) as String;
            }

            entityDescription.DomainName = attr.Domain;
           

            entityDescription.EntityType = attr.EntityType;
            entityDescription.Cloneable = attr.Cloneable;
            entityDescription.SaveDraft = attr.SaveDraft;
            return entityDescription;
        }

        public EntityTypes EntityType { get; set; }

        public String Name { get; set; }
        public String Description { get; set; }
        public String DomainName { get; set; }
        public DomainDescription Domain { get; set; }
        public String UserHelp { get; set; }
        public String Title { get; set; }

        public int? Col1WidthPercent { get; set; }
        public int? Col2WidthPercent { get; set; }

        public bool AutoSave { get; set; }
        public int? AutoSaveIntervalSeconds { get; set; }


        public bool CanImport { get; set; }
        public bool CanExport { get; set; }

        public string Icon { get; set; }

        public string FactoryUrl { get; set; }
        public string GetUrl { get; set; }
        public string DeleteUrl { get; set; }
        public string GetListUrl { get; set; }
        public string UpdateUrl { get; set; }
        public string SaveUrl { get; set; }
        public string InsertUrl { get; set; }
        public string HelpUrl { get; set; }

        public bool Cloneable { get; set; }

        public bool SaveDraft { get; set; }

        public List<FormField> Elements { get;  set; }

        public List<ListColumn> ListColumns { get;  set; }

        public string ListUIUrl { get; set; }
        public string PreviewUIUrl { get; set; }
        public string EditUIUrl { get; set; }
        public string CreateUIUrl { get; set; }
    }
}
