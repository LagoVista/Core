// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 48169e72c7e5afa7497bd7ad13261d6c1622a6b38a348605f505385bbbe69adf
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Resources;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    [EntityDescription(Domains.CoreDomainName, LagoVistaCommonStrings.Names.Category_Title, LagoVistaCommonStrings.Names.Category_Description,
              LagoVistaCommonStrings.Names.Category_Description, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(LagoVistaCommonStrings), Icon: "icon-ae-book-1",
                GetListUrl: "/api/categories/{type}", GetUrl: "/api/category/{id}", SaveUrl: "/api/category", FactoryUrl: "/api/category/factory", DeleteUrl: "/api/category/{id}")]
    public class Category : EntityBase, IIconEntity, IValidateable
    {
        public Category(String categorytype)
        {
            Id = Guid.NewGuid().ToId();
            CategoryType = categorytype;
            Icon = "icon-ae-book-1";
        }

        public Category()
        {

        }

        public string CategoryType { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Icon, FieldType: FieldTypes.Icon, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, IsUserEditable: true)]
        public string Icon { get; set; } = "icon-ae-book-1";
    }
}
