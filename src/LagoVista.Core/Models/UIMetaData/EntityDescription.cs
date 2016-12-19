using LagoVista.Core.Attributes;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace LagoVista.Core.Models.UIMetaData
{
    public class EntityDescription
    {
        public static EntityDescription Create(Type entityType, EntityDescriptionAttribute attr)
        {
            var entityDescription = new EntityDescription();

            var properties = entityType.GetRuntimeProperties();
            foreach (var property in properties)
            {
                var fieldAttributes = property.GetCustomAttributes<FormFieldAttribute>();
                if (fieldAttributes.Any())
                {
                    entityDescription.Elements.Add(FormField.Create(property.Name.ToLower(), fieldAttributes.First()));
                }
            }
           
            entityDescription.Name = entityType.Name;

            var titleProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.DescriptionResource);
            entityDescription.Title = titleProperty.GetValue(titleProperty.DeclaringType, null) as String;

            var descriptionProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.DescriptionResource);
            entityDescription.Description = descriptionProperty.GetValue(descriptionProperty.DeclaringType, null) as String;

            var userHelpProperty = attr.ResourceType.GetTypeInfo().GetDeclaredProperty(attr.DescriptionResource);
            entityDescription.UserHelp = userHelpProperty.GetValue(userHelpProperty.DeclaringType, null) as String;

            entityDescription.DomainName = attr.Domain;
            entityDescription.Elements = new List<FormField>();

            return entityDescription;
        }

        public String Name { get; set; }
        public String Description { get; set; }
        public String DomainName { get; set; }
        public DomainDescription Domain { get; set; }
        public String UserHelp { get; set; }
        public String Title { get; set; }

        public List<FormField> Elements { get; private set; }
    }
}
