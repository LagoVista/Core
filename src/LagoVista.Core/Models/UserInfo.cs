// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c1183f37d4a16e6e6c3f973765fa8e36cc60962d088292ddafc10f4314224e5b
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Resources;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Models
{

    public enum ExternalLoginTypes
    {
        [EnumLabel(ExternalLogin.ExternalLogin_GitHub, AuthenticationResources.Names.ExternalLogin_GitHub, ResourceType: typeof(AuthenticationResources))]
        GitHub,
        [EnumLabel(ExternalLogin.ExternalLogin_LinkedIn, AuthenticationResources.Names.ExternalLogin_LinkedIn, ResourceType: typeof(AuthenticationResources))]
        LinkedIn,
        [EnumLabel(ExternalLogin.ExternalLogin_Microsoft, AuthenticationResources.Names.ExternalLogin_Microsoft, ResourceType: typeof(AuthenticationResources))]
        Microsoft,
        [EnumLabel(ExternalLogin.ExternalLogin_Google, AuthenticationResources.Names.ExternalLogin_Google, ResourceType: typeof(AuthenticationResources))]
        Google,
        [EnumLabel(ExternalLogin.ExternalLogin_Twitter, AuthenticationResources.Names.ExternalLogin_Twitter, ResourceType: typeof(AuthenticationResources))]
        Twitter,
        [EnumLabel(ExternalLogin.ExternalLogin_Facebook, AuthenticationResources.Names.ExternalLogin_Facebook, ResourceType: typeof(AuthenticationResources))]
        Facebook,
        [EnumLabel(ExternalLogin.ExternalLogin_SLTesting, AuthenticationResources.Names.ExternalLogin_SLTesting, ResourceType: typeof(AuthenticationResources))]
        SLTesting,
    }

    /* The AppUser is used for managing identity for the system, we want something with basic user information that doesn't
     * do anything with security or identity...this is that object */

    [EntityDescription(AuthDomain.AuthenticationDomain, AuthenticationResources.Names.UserInfo_ObjectTitle, AuthenticationResources.Names.UserInfo_Help, AuthenticationResources.Names.UserInfo_Description,
        EntityDescriptionAttribute.EntityTypes.Dto, typeof(AuthenticationResources), Icon: "icon-pz-user-profile-1")]
    public class UserInfo : IUserInfo, ISummaryFactory
    {
     
        public String Id { get; set; }

        public String Key { get; set; }


        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsSystemAdmin, ResourceType: typeof(AuthenticationResources))]
        public String Name { get; set; }
   
        [FormField(LabelResource: Resources.AuthenticationResources.Names.Common_CreationDate, FieldType: FieldTypes.JsonDateTime, ResourceType: typeof(AuthenticationResources), IsRequired: true, IsUserEditable: false)]
        public String CreationDate { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.Common_CreatedBy, ResourceType: typeof(AuthenticationResources), IsRequired: true, IsUserEditable: false)]
        public EntityHeader CreatedBy { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.Common_LastUpdatedDate, FieldType: FieldTypes.JsonDateTime, ResourceType: typeof(AuthenticationResources), IsRequired: true, IsUserEditable: false)]
        public String LastUpdatedDate { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.Common_LastUpdatedBy, ResourceType: typeof(AuthenticationResources), IsRequired: true, IsUserEditable: false)]
        public EntityHeader LastUpdatedBy { get; set; }

        public string LastLogin { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_FirstName, IsRequired: true, ResourceType: typeof(Resources.AuthenticationResources))]
        public string FirstName { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_LastName, IsRequired: true, ResourceType: typeof(Resources.AuthenticationResources))]
        public string LastName { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_Title, IsRequired: false, ResourceType: typeof(Resources.AuthenticationResources))]
        public string Title { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_Bio, IsRequired: false, ResourceType: typeof(Resources.AuthenticationResources))]
        public string Bio { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_IsSystemAdmin, HelpResource: Resources.AuthenticationResources.Names.UserInfo_IsSystemAdmin_Help, FieldType:FieldTypes.CheckBox, ResourceType: typeof(Resources.AuthenticationResources))]
        public bool IsSystemAdmin { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_IsOrgAdmin, HelpResource: Resources.AuthenticationResources.Names.UserInfo_IsOrgAdmin_Help, FieldType: FieldTypes.CheckBox, ResourceType: typeof(Resources.AuthenticationResources))]
        public bool IsOrgAdmin { get; set; }

        [FormField(LabelResource: AuthenticationResources.Names.UserInfo_IsAppBuilder, FieldType: FieldTypes.CheckBox, ResourceType: typeof(AuthenticationResources))]
        public bool IsAppBuilder { get; set; }

        [FormField(LabelResource: AuthenticationResources.Names.UserInfo_IsUserDevice, FieldType: FieldTypes.CheckBox, ResourceType: typeof(AuthenticationResources))]
        public bool IsUserDevice { get; set; }

        [FormField(LabelResource: AuthenticationResources.Names.UserInfo_IsRuntimeUser, FieldType: FieldTypes.CheckBox, ResourceType: typeof(AuthenticationResources))]
        public bool IsRuntimeUser { get; set; }

        [FormField(LabelResource: AuthenticationResources.Names.UserInfo_IsAccountDisabled, FieldType: FieldTypes.CheckBox, ResourceType: typeof(AuthenticationResources))]
        public bool IsAccountDisabled { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_IsPreviewUser, HelpResource: Resources.AuthenticationResources.Names.UserInfo_IsPreviewUser, IsRequired: true, FieldType: FieldTypes.CheckBox, ResourceType: typeof(Resources.AuthenticationResources))]
        public bool IsPreviewUser { get; set; }


        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_UserName, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.AuthenticationResources))]
        public String UserName { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_PhoneNumber, FieldType: FieldTypes.Phone, ResourceType: typeof(Resources.AuthenticationResources))]
        public string PhoneNumber { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_TeamsAccountName, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.AuthenticationResources))]
        public string TeamsAccountName { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_IsPhoneConfirmed, IsUserEditable: false, FieldType: FieldTypes.CheckBox, ResourceType: typeof(Resources.AuthenticationResources))]
        public bool PhoneNumberConfirmed { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_ShowWelcome, IsUserEditable: true, FieldType: FieldTypes.CheckBox, ResourceType: typeof(Resources.AuthenticationResources))]
        public bool ShowWelcome{ get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_Notes, FieldType: FieldTypes.ChildList, ResourceType: typeof(Resources.AuthenticationResources))]
        public List<EntityHeader<string>> Notes { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_SocialSecurityNumber, HelpResource:Resources.AuthenticationResources.Names.UserInfo_SocialSecurityNumber_Help, 
          SecureIdFieldName:nameof(SsnSecretId), FieldType: FieldTypes.Password, ResourceType: typeof(Resources.AuthenticationResources))]
        public string Ssn { get; set; }

        public string SsnSecretId { get; set; }
        public EntityHeader PrimaryOrganization { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_Address1, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.AuthenticationResources))]
        public string Address1 { get; set; }
        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_Address2, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.AuthenticationResources))]
        public string Address2 { get; set; }
        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_City, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.AuthenticationResources))]
        public string City { get; set; }
        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_StateProvince, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.AuthenticationResources))]
        public string State { get; set; }
        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_Country, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.AuthenticationResources))]
        public string Country { get; set; }
        
        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_PostalCode, FieldType: FieldTypes.Text, ResourceType: typeof(Resources.AuthenticationResources))]
        public string PostalCode { get; set; }


        [FormField(LabelResource: Resources.AuthenticationResources.Names.Common_TimeZone, FieldType: FieldTypes.Picker, WaterMark:AuthenticationResources.Names.Common_TimeZone_Select, ResourceType: typeof(Resources.AuthenticationResources))]
        public EntityHeader TimeZone { get; set; }

        public bool TermsAndConditionsAccepted { get; set; }
        public string TermsAndConditionsAcceptedIPAddress { get; set; }
        public string TermsAndConditionsAcceptedDateTime { get; set; }

        public EntityHeader CurrentOrganization { get; set; }

        public EntityHeader PrimaryDevice { get; set; }
        public EntityHeader DeviceRepo { get; set; }
        public EntityHeader DeviceConfiguration { get; set; }

        public List<EntityHeader> MediaResources { get; set; }

        public ImageDetails ProfileImageUrl { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_Email, IsRequired: true, FieldType: FieldTypes.Email, ResourceType: typeof(Resources.AuthenticationResources))]
        public string Email { get; set; }
        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_IsEmailConfirmed, IsUserEditable: false, FieldType: FieldTypes.CheckBox, ResourceType: typeof(Resources.AuthenticationResources))]
        public bool EmailConfirmed { get; set; }

        [FormField(LabelResource: Resources.AuthenticationResources.Names.UserInfo_EmailSignature, IsRequired: false, FieldType: FieldTypes.HtmlEditor, ResourceType: typeof(Resources.AuthenticationResources))]
        public string EmailSignature { get; set; }

        public List<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();
        public List<EntityHeader> Roles { get; set; } = new List<EntityHeader>();

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
                CreationDate = CreationDate,
                Name = $"{FirstName} {LastName}",
                Email = Email,
                UserName = UserName,
                IsSystemAdmin = IsSystemAdmin,
                IsAppBuilder = IsAppBuilder,
                IsUserDevice = IsUserDevice,
                IsOrgAdmin = IsOrgAdmin,
                IsAccountDisabled = IsAccountDisabled,
                IsRuntimeUser = IsRuntimeUser,
                ProfileImageUrl = ProfileImageUrl,
                EmailConfirmed = EmailConfirmed,
                PhoneNumberConfirmed = PhoneNumberConfirmed,
                TeamsAccountName = TeamsAccountName,
                LastLogin = LastLogin,
                Title = Title,
                TimeZone = TimeZone,
            };
        }

        ISummaryData ISummaryFactory.CreateSummary()
        {
            return CreateSummary();
        }
    }

    [EntityDescription(AuthDomain.AuthenticationDomain, AuthenticationResources.Names.UserInfo_ObjectTitle, AuthenticationResources.Names.UserInfo_Help, AuthenticationResources.Names.UserInfo_Description,
        EntityDescriptionAttribute.EntityTypes.Dto, typeof(AuthenticationResources), Icon: "icon-pz-user-profile-1")]

    public class UserInfoSummary : SummaryData
    {        
        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsSystemAdmin, ResourceType: typeof(AuthenticationResources))]
        public bool IsSystemAdmin { get; set; }

        [ListColumn(HeaderResource:AuthenticationResources.Names.UserInfo_DateCreated, ResourceType: typeof(AuthenticationResources))]
        public string CreationDate { get; set; }

        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsOrgAdmin, ResourceType: typeof(AuthenticationResources))]
        public bool IsOrgAdmin { get; set; }
        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsAppBuilder, ResourceType: typeof(AuthenticationResources))]
        public bool IsAppBuilder { get; set; }
        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsUserDevice, ResourceType: typeof(AuthenticationResources))]
        public bool IsUserDevice { get; set; }
        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsRuntimeUser, ResourceType: typeof(AuthenticationResources))]
        public bool IsRuntimeUser { get; set; }

        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsAccountDisabled, ResourceType: typeof(AuthenticationResources))]
        public bool IsAccountDisabled { get; set; }


        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_UserName, ResourceType: typeof(AuthenticationResources))]
        public String UserName { get; set; }

        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_Email, ResourceType: typeof(AuthenticationResources))]
        public String Email { get; set; }

        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_PhoneNumber, ResourceType: typeof(AuthenticationResources))]
        public String PhoneNumber { get; set; }

        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_TeamsAccountName, ResourceType: typeof(AuthenticationResources))]
        public string TeamsAccountName { get; set; }

        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_CurrentOrganization, ResourceType: typeof(AuthenticationResources))]
        public EntityHeader CurrentOrganization { get; set; }
        
        [ListColumn(Visible: false)]
        public ImageDetails ProfileImageUrl { get; set; }
        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsEmailConfirmed, ResourceType: typeof(AuthenticationResources))]
        public bool EmailConfirmed { get; set; }
        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_IsPhoneConfirmed, ResourceType: typeof(AuthenticationResources))]
        public bool PhoneNumberConfirmed { get; set; }

        [ListColumn(HeaderResource: AuthenticationResources.Names.UserInfo_Title, ResourceType: typeof(AuthenticationResources))]
        public string Title { get; set; }

        public string LastLogin { get; set; }

        public EntityHeader Customer { get; set; }
        public EntityHeader CustomerContact { get; set; }

        public EntityHeader TimeZone { get; set; }
    }

    public class ExternalLogin
    {
        public const string ExternalLogin_GitHub = "github";
        public const string ExternalLogin_Microsoft = "microsoft";
        public const string ExternalLogin_Google = "google";
        public const string ExternalLogin_LinkedIn = "linkedin";
        public const string ExternalLogin_Facebook = "facebook";
        public const string ExternalLogin_Twitter = "twitter";
        public const string ExternalLogin_SLTesting = "sltesting";

        public EntityHeader<ExternalLoginTypes> Provider { get; set; }
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }
        public string OAuthToken { get; set; }
        public string OAuthTokenSecretId { get; set; }
        public string OAuthTokenVerifier { get; set; }
        public string OAuthTokenVerifierSecretId { get; set; }
    }
}