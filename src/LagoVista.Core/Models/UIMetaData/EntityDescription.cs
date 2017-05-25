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

            var properties = entityType.GetRuntimeProperties();
            foreach (var property in properties)
            {
                var fieldAttributes = property.GetCustomAttributes<FormFieldAttribute>();
                if (fieldAttributes.Any())
                {
                    entityDescription.Elements.Add(FormField.Create(property.Name, fieldAttributes.First()));
                }

                var listAttributes = property.GetCustomAttributes<ListColumnAttribute>();
                if(listAttributes.Any())
                {
                    entityDescription.ListColumns.Add(ListColumn.Create(property.Name, listAttributes.First()));
                }
            }
           
            entityDescription.Name = entityType.Name;

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

            return entityDescription;
        }

        public EntityTypes EntityType { get; set; }

        public String Name { get; set; }
        public String Description { get; set; }
        public String DomainName { get; set; }
        public DomainDescription Domain { get; set; }
        public String UserHelp { get; set; }
        public String Title { get; set; }

        public List<FormField> Elements { get;  set; }

        public List<ListColumn> ListColumns { get;  set; }
    }
}
