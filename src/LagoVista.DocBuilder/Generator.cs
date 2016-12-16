using HtmlTags;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LagoVista.Common.Attributes;

namespace LagoVista.DocBuilder
{
    public class Generator
    {
        static Generator _instance = new Generator();

        public static Generator Instance { get { return _instance; }}
        private Generator()
        {
        }

        public HtmlTag Generate(Type entity)
        {
            var topLevel = new HtmlTag("div");

            var entityAttr = entity.GetTypeInfo().GetCustomAttribute<EntityDescriptionAttribute>();

            var titleTag = topLevel.Add("h1");
            titleTag.Text(entityAttr.Name);

            topLevel.Add("p").Text(entityAttr.Description);

            var propertyList = topLevel.Add("ul");
            foreach (var property in entity.GetTypeInfo().GetProperties()) 
            {
                var propertyListItem = propertyList.Add("li");
                propertyListItem.Add("div").Text(property.Name);
            }

            return topLevel;
        }

    }
}
