using LagoVista.Core.Attributes;
using LagoVista.Core.Resources;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{
    /* The AppUser is used for managing identity for the system, we want something with basic user information that doesn't
     * do anything with security or identity...this is that object */

    [EntityDescription(Domain.AuthenticationDomain, AuthenticationResources.Names.UserInfo_Title, AuthenticationResources.Names.UserInfo_Help, AuthenticationResources.Names.UserInfo_Description, EntityDescriptionAttribute.EntityTypes.Dto, typeof(AuthenticationResources))]
    public class UserInfo : IUserInfo
    {
        public String Id { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.Common_CreationDate, FieldType: FieldTypes.JsonDateTime, ResourceType: typeof(AuthenticationResources), IsRequired: true, IsUserEditable: false)]
        public String CreationDate { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.Common_CreatedBy, ResourceType: typeof(AuthenticationResources), IsRequired: true, IsUserEditable: false)]
        public EntityHeader CreatedBy { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.Common_LastUpdatedDate, FieldType: FieldTypes.JsonDateTime, ResourceType: typeof(AuthenticationResources), IsRequired: true, IsUserEditable: false)]
        public String LastUpdatedDate { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.Common_LastUpdatedBy, ResourceType: typeof(AuthenticationResources), IsRequired: true, IsUserEditable: false)]
        public EntityHeader LastUpdatedBy { get; set; }


        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_FirstName, IsRequired: true, ResourceType: typeof(Resources.AuthenticationResources))]
        public string FirstName { get; set; }
        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_LastName, IsRequired: true, ResourceType: typeof(Resources.AuthenticationResources))]
        public string LastName { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_IsSystemAdmin, IsRequired: true, ResourceType: typeof(Resources.AuthenticationResources))]
        public bool IsSystemAdmin { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_PhoneNumber, FieldType: FieldTypes.Phone, ResourceType: typeof(Resources.AuthenticationResources))]
        public string PhoneNumber { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_IsPhoneConfirmed, IsUserEditable: false, FieldType: FieldTypes.CheckBox, ResourceType: typeof(Resources.AuthenticationResources))]
        public bool PhoneNumberConfirmed { get; set; }

        public EntityHeader CurrentOrganization { get; set; }

        public ImageDetails ProfileImageUrl { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_Email, IsRequired: true, FieldType: FieldTypes.Email, ResourceType: typeof(Resources.AuthenticationResources))]
        public string Email { get; set; }
        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_IsEmailConfirmed, IsUserEditable: false, FieldType: FieldTypes.CheckBox, ResourceType: typeof(Resources.AuthenticationResources))]
        public bool EmailConfirmed { get; set; }



        public EntityHeader ToEntityHeader()
        {
            return new EntityHeader()
            {
                Id = Id,
                Text = $"{FirstName} {LastName}"
            };
        }

        public UserInfoSummary CreateSummary()
        {
            return new UserInfoSummary()
            {
                Id = Id,
                Key = Id,
                Name = $"{FirstName} {LastName}",
                Email = Email,
                ProfileImageUrl = ProfileImageUrl,
                EmailConfirmed = EmailConfirmed,
                PhoneNumberConfirmed = PhoneNumberConfirmed
            };
        }
    }


    public class UserInfoSummary
    {
        [ListColumn(Visible: false)]
        public String Id { get; set; }
        [ListColumn(Visible: false)]
        public String Key { get; set; }

        [ListColumn(HeaderResource: AuthenticationResources.Names.Common_Name, ResourceType: typeof(AuthenticationResources))]
        public bool IsSystemAdmin { get; set; }
        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsSystemAdmin, ResourceType: typeof(AuthenticationResources))]
        public String Name { get; set; }
        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_Email, ResourceType: typeof(AuthenticationResources))]
        public String Email { get; set; }
        [ListColumn(Visible: false)]
        public ImageDetails ProfileImageUrl { get; set; }
        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsEmailConfirmed, ResourceType: typeof(AuthenticationResources))]
        public bool EmailConfirmed { get; set; }
        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsPhoneConfirmed, ResourceType: typeof(AuthenticationResources))]
        public bool PhoneNumberConfirmed { get; set; }
    }
}