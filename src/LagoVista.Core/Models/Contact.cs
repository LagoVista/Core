using LagoVista.Core.Attributes;
using LagoVista.Core.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    public class ContactDTO : TableStorageEntity
    {
        public ContactDTO(Company company, Contact contact)
        {
            RowKey = contact.Id;
            OwnerOrganization = company.OwnerOrganization.Text;
            OwnerOrganizationId = company.OwnerOrganization.Id;

            Industry = company.Industry.Text;
            Industry = company.Industry.Id;
            Company = company.Name;
            CompanyId = company.Id;
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            Email = contact.Email.Trim();
            Phone = contact.Phone.Replace("(", String.Empty).Replace(")", String.Empty).Replace(" ", String.Empty).Replace("-", String.Empty).Replace("+", String.Empty);

            PartitionKey = company.OwnerOrganization.Id;
            
            DateAdded = DateTime.UtcNow.ToJSONString();
        }

        public string DateAdded { get; set; }

        public string OwnerOrganizationId { get; set; }
        public string OwnerOrganization { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Industry { get; set; }
        public string IndustryId { get; set; }
        public string Company { get; set; }
        public string CompanyId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool DoNotContact { get; set; }
    }

    [EntityDescription(Domains.CoreDomainName, LagoVistaCommonStrings.Names.Company_Title, LagoVistaCommonStrings.Names.Company_Help,
                      LagoVistaCommonStrings.Names.Company_Help, EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(LagoVistaCommonStrings))]
    public class Company : EntityBase
    {
        [FormField(LabelResource: LagoVistaCommonStrings.Names.Common_Description, IsRequired: false, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(LagoVistaCommonStrings))]
        public string Description { get; set; }


        [FormField(LabelResource: LagoVistaCommonStrings.Names.Company_Industry, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, FieldType: FieldTypes.Picker,SortEnums: true, AddEnumSelect: true)]
        public EntityHeader Industry { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Customer_Address1, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, IsUserEditable: true)]
        public string Address1 { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Customer_Address2, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, IsUserEditable: true)]
        public string Address2 { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Customer_City, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, IsUserEditable: true)]
        public string City { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Customer_State, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, IsUserEditable: true)]
        public string State { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Customer_ZipCode, FieldType: FieldTypes.Text, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, IsUserEditable: true)]
        public string Zip { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Customer_WebSite, FieldType: FieldTypes.WebLink, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, IsUserEditable: true)]
        public string WebSite { get; set; }



        [FormField(LabelResource: LagoVistaCommonStrings.Names.Customer_Notes, FieldType: FieldTypes.MultiLineText, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, IsUserEditable: true)]
        public string Notes { get; set; }

    }

    [EntityDescription(Domains.CoreDomainName, LagoVistaCommonStrings.Names.Contact_Title, LagoVistaCommonStrings.Names.Contact_Help, LagoVistaCommonStrings.Names.Contact_Description,
                  EntityDescriptionAttribute.EntityTypes.SimpleModel, typeof(LagoVistaCommonStrings))]
    public class Contact
    {
        public Contact()
        {
            Id = Guid.NewGuid().ToId();
        }

        public string Id { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Contact_FirstName, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, FieldType: FieldTypes.Text)]
        public string FirstName { get; set; }


        [FormField(LabelResource: LagoVistaCommonStrings.Names.Contact_LastName, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: true, FieldType: FieldTypes.Text)]
        public string LastName { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Contact_Title, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, FieldType: FieldTypes.Text)]
        public string Title { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Contact_LinkedInPage, FieldType: FieldTypes.WebLink, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, IsUserEditable: true)]
        public string LinkedInPage { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Contact_Phone, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, FieldType: FieldTypes.Phone)]
        public string Phone { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Contact_Email, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, FieldType: FieldTypes.Email)]
        public string Email { get; set; }

        [FormField(LabelResource: LagoVistaCommonStrings.Names.Contact_Notes, ResourceType: typeof(LagoVistaCommonStrings), IsRequired: false, FieldType: FieldTypes.HtmlEditor)]
        public string Notes { get; set; }

        public string CreationDate { get; set; }
        public string LastUpdatedDate { get; set; }
        public EntityHeader CreatedBy { get; set; }
        public EntityHeader LastUpdatedBy { get; set; }

        public bool IsDeleted { get; set; }
        public EntityHeader DeletedBy { get; set; }
        public String DeletionDate { get; set; }

        public bool DoNotContact { get; set; }
        public string DoNotContactReason { get; set; }
        public string DoNotContactSetDate { get; set; }

        public List<string> GetFormFields()
        {
            return new List<string>()
            {
                nameof(FirstName),
                nameof(LastName),
                nameof(Title),
                nameof(LinkedInPage),
                nameof(Phone),
                nameof(Email),
                nameof(Notes),
            };
        }
    }
}
