/*5/28/2025 1:43:15 PM*/
using System.Globalization;
using System.Reflection;

//Resources:AuthenticationResources:Common_CreatedBy
namespace LagoVista.Core.Resources
{
	public class AuthenticationResources
	{
        private static global::System.Resources.ResourceManager _resourceManager;
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static global::System.Resources.ResourceManager ResourceManager 
		{
            get 
			{
                if (object.ReferenceEquals(_resourceManager, null)) 
				{
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.Core.Resources.AuthenticationResources", typeof(AuthenticationResources).GetTypeInfo().Assembly);
                    _resourceManager = temp;
                }
                return _resourceManager;
            }
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static string GetResourceString(string key, params string[] tokens)
		{
			var culture = CultureInfo.CurrentCulture;;
            var str = ResourceManager.GetString(key, culture);

			for(int i = 0; i < tokens.Length; i += 2)
				str = str.Replace(tokens[i], tokens[i+1]);
										
            return str;
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
		/*
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static HtmlString GetResourceHtmlString(string key, params string[] tokens)
		{
			var str = GetResourceString(key, tokens);
							
			if(str.StartsWith("HTML:"))
				str = str.Substring(5);

			return new HtmlString(str);
        }*/
		
		public static string Common_CreatedBy { get { return GetResourceString("Common_CreatedBy"); } }
//Resources:AuthenticationResources:Common_CreationDate

		public static string Common_CreationDate { get { return GetResourceString("Common_CreationDate"); } }
//Resources:AuthenticationResources:Common_Description

		public static string Common_Description { get { return GetResourceString("Common_Description"); } }
//Resources:AuthenticationResources:Common_EmailAddress

		public static string Common_EmailAddress { get { return GetResourceString("Common_EmailAddress"); } }
//Resources:AuthenticationResources:Common_Id

		public static string Common_Id { get { return GetResourceString("Common_Id"); } }
//Resources:AuthenticationResources:Common_Key

		public static string Common_Key { get { return GetResourceString("Common_Key"); } }
//Resources:AuthenticationResources:Common_Key_Help

		public static string Common_Key_Help { get { return GetResourceString("Common_Key_Help"); } }
//Resources:AuthenticationResources:Common_Key_Validation

		public static string Common_Key_Validation { get { return GetResourceString("Common_Key_Validation"); } }
//Resources:AuthenticationResources:Common_LastUpdatedBy

		public static string Common_LastUpdatedBy { get { return GetResourceString("Common_LastUpdatedBy"); } }
//Resources:AuthenticationResources:Common_LastUpdatedDate

		public static string Common_LastUpdatedDate { get { return GetResourceString("Common_LastUpdatedDate"); } }
//Resources:AuthenticationResources:Common_Name

		public static string Common_Name { get { return GetResourceString("Common_Name"); } }
//Resources:AuthenticationResources:Common_Namespace

		public static string Common_Namespace { get { return GetResourceString("Common_Namespace"); } }
//Resources:AuthenticationResources:Common_Notes

		public static string Common_Notes { get { return GetResourceString("Common_Notes"); } }
//Resources:AuthenticationResources:Common_PhoneNumber

		public static string Common_PhoneNumber { get { return GetResourceString("Common_PhoneNumber"); } }
//Resources:AuthenticationResources:Common_Role

		public static string Common_Role { get { return GetResourceString("Common_Role"); } }
//Resources:AuthenticationResources:Common_Status

		public static string Common_Status { get { return GetResourceString("Common_Status"); } }
//Resources:AuthenticationResources:ExternalLogin_Facebook

		public static string ExternalLogin_Facebook { get { return GetResourceString("ExternalLogin_Facebook"); } }
//Resources:AuthenticationResources:ExternalLogin_GitHub

		public static string ExternalLogin_GitHub { get { return GetResourceString("ExternalLogin_GitHub"); } }
//Resources:AuthenticationResources:ExternalLogin_Google

		public static string ExternalLogin_Google { get { return GetResourceString("ExternalLogin_Google"); } }
//Resources:AuthenticationResources:ExternalLogin_LinkedIn

		public static string ExternalLogin_LinkedIn { get { return GetResourceString("ExternalLogin_LinkedIn"); } }
//Resources:AuthenticationResources:ExternalLogin_Microsoft

		public static string ExternalLogin_Microsoft { get { return GetResourceString("ExternalLogin_Microsoft"); } }
//Resources:AuthenticationResources:ExternalLogin_SLTesting

		public static string ExternalLogin_SLTesting { get { return GetResourceString("ExternalLogin_SLTesting"); } }
//Resources:AuthenticationResources:ExternalLogin_Twitter

		public static string ExternalLogin_Twitter { get { return GetResourceString("ExternalLogin_Twitter"); } }
//Resources:AuthenticationResources:UserInfo_Address1

		public static string UserInfo_Address1 { get { return GetResourceString("UserInfo_Address1"); } }
//Resources:AuthenticationResources:UserInfo_Address2

		public static string UserInfo_Address2 { get { return GetResourceString("UserInfo_Address2"); } }
//Resources:AuthenticationResources:UserInfo_Bio

		public static string UserInfo_Bio { get { return GetResourceString("UserInfo_Bio"); } }
//Resources:AuthenticationResources:UserInfo_City

		public static string UserInfo_City { get { return GetResourceString("UserInfo_City"); } }
//Resources:AuthenticationResources:UserInfo_Country

		public static string UserInfo_Country { get { return GetResourceString("UserInfo_Country"); } }
//Resources:AuthenticationResources:UserInfo_CurrentOrganization

		public static string UserInfo_CurrentOrganization { get { return GetResourceString("UserInfo_CurrentOrganization"); } }
//Resources:AuthenticationResources:UserInfo_DateCreated

		public static string UserInfo_DateCreated { get { return GetResourceString("UserInfo_DateCreated"); } }
//Resources:AuthenticationResources:UserInfo_Description

		public static string UserInfo_Description { get { return GetResourceString("UserInfo_Description"); } }
//Resources:AuthenticationResources:UserInfo_Email

		public static string UserInfo_Email { get { return GetResourceString("UserInfo_Email"); } }
//Resources:AuthenticationResources:UserInfo_EmailSignature

		public static string UserInfo_EmailSignature { get { return GetResourceString("UserInfo_EmailSignature"); } }
//Resources:AuthenticationResources:UserInfo_FirstName

		public static string UserInfo_FirstName { get { return GetResourceString("UserInfo_FirstName"); } }
//Resources:AuthenticationResources:UserInfo_Help

		public static string UserInfo_Help { get { return GetResourceString("UserInfo_Help"); } }
//Resources:AuthenticationResources:UserInfo_IsAccountDisabled

		public static string UserInfo_IsAccountDisabled { get { return GetResourceString("UserInfo_IsAccountDisabled"); } }
//Resources:AuthenticationResources:UserInfo_IsAppBuilder

		public static string UserInfo_IsAppBuilder { get { return GetResourceString("UserInfo_IsAppBuilder"); } }
//Resources:AuthenticationResources:UserInfo_IsEmailConfirmed

		public static string UserInfo_IsEmailConfirmed { get { return GetResourceString("UserInfo_IsEmailConfirmed"); } }
//Resources:AuthenticationResources:UserInfo_IsOrgAdmin

		public static string UserInfo_IsOrgAdmin { get { return GetResourceString("UserInfo_IsOrgAdmin"); } }
//Resources:AuthenticationResources:UserInfo_IsOrgAdmin_Help

		public static string UserInfo_IsOrgAdmin_Help { get { return GetResourceString("UserInfo_IsOrgAdmin_Help"); } }
//Resources:AuthenticationResources:UserInfo_IsPhoneConfirmed

		public static string UserInfo_IsPhoneConfirmed { get { return GetResourceString("UserInfo_IsPhoneConfirmed"); } }
//Resources:AuthenticationResources:UserInfo_IsPreviewUse_Help

		public static string UserInfo_IsPreviewUse_Help { get { return GetResourceString("UserInfo_IsPreviewUse_Help"); } }
//Resources:AuthenticationResources:UserInfo_IsPreviewUser

		public static string UserInfo_IsPreviewUser { get { return GetResourceString("UserInfo_IsPreviewUser"); } }
//Resources:AuthenticationResources:UserInfo_IsRuntimeUser

		public static string UserInfo_IsRuntimeUser { get { return GetResourceString("UserInfo_IsRuntimeUser"); } }
//Resources:AuthenticationResources:UserInfo_IsSystemAdmin

		public static string UserInfo_IsSystemAdmin { get { return GetResourceString("UserInfo_IsSystemAdmin"); } }
//Resources:AuthenticationResources:UserInfo_IsSystemAdmin_Help

		public static string UserInfo_IsSystemAdmin_Help { get { return GetResourceString("UserInfo_IsSystemAdmin_Help"); } }
//Resources:AuthenticationResources:UserInfo_IsUserDevice

		public static string UserInfo_IsUserDevice { get { return GetResourceString("UserInfo_IsUserDevice"); } }
//Resources:AuthenticationResources:UserInfo_LastName

		public static string UserInfo_LastName { get { return GetResourceString("UserInfo_LastName"); } }
//Resources:AuthenticationResources:UserInfo_Notes

		public static string UserInfo_Notes { get { return GetResourceString("UserInfo_Notes"); } }
//Resources:AuthenticationResources:UserInfo_ObjectTitle

		public static string UserInfo_ObjectTitle { get { return GetResourceString("UserInfo_ObjectTitle"); } }
//Resources:AuthenticationResources:UserInfo_PhoneNumber

		public static string UserInfo_PhoneNumber { get { return GetResourceString("UserInfo_PhoneNumber"); } }
//Resources:AuthenticationResources:UserInfo_PostalCode

		public static string UserInfo_PostalCode { get { return GetResourceString("UserInfo_PostalCode"); } }
//Resources:AuthenticationResources:UserInfo_ShowWelcome

		public static string UserInfo_ShowWelcome { get { return GetResourceString("UserInfo_ShowWelcome"); } }
//Resources:AuthenticationResources:UserInfo_SocialSecurityNumber

		public static string UserInfo_SocialSecurityNumber { get { return GetResourceString("UserInfo_SocialSecurityNumber"); } }
//Resources:AuthenticationResources:UserInfo_SocialSecurityNumber_Help

		public static string UserInfo_SocialSecurityNumber_Help { get { return GetResourceString("UserInfo_SocialSecurityNumber_Help"); } }
//Resources:AuthenticationResources:UserInfo_StateProvince

		public static string UserInfo_StateProvince { get { return GetResourceString("UserInfo_StateProvince"); } }
//Resources:AuthenticationResources:UserInfo_TeamsAccountName

		public static string UserInfo_TeamsAccountName { get { return GetResourceString("UserInfo_TeamsAccountName"); } }
//Resources:AuthenticationResources:UserInfo_Title

		public static string UserInfo_Title { get { return GetResourceString("UserInfo_Title"); } }
//Resources:AuthenticationResources:UserInfo_UserName

		public static string UserInfo_UserName { get { return GetResourceString("UserInfo_UserName"); } }
//Resources:ComparatorResources:EmptyValue

		public static class Names
		{
			public const string Common_CreatedBy = "Common_CreatedBy";
			public const string Common_CreationDate = "Common_CreationDate";
			public const string Common_Description = "Common_Description";
			public const string Common_EmailAddress = "Common_EmailAddress";
			public const string Common_Id = "Common_Id";
			public const string Common_Key = "Common_Key";
			public const string Common_Key_Help = "Common_Key_Help";
			public const string Common_Key_Validation = "Common_Key_Validation";
			public const string Common_LastUpdatedBy = "Common_LastUpdatedBy";
			public const string Common_LastUpdatedDate = "Common_LastUpdatedDate";
			public const string Common_Name = "Common_Name";
			public const string Common_Namespace = "Common_Namespace";
			public const string Common_Notes = "Common_Notes";
			public const string Common_PhoneNumber = "Common_PhoneNumber";
			public const string Common_Role = "Common_Role";
			public const string Common_Status = "Common_Status";
			public const string ExternalLogin_Facebook = "ExternalLogin_Facebook";
			public const string ExternalLogin_GitHub = "ExternalLogin_GitHub";
			public const string ExternalLogin_Google = "ExternalLogin_Google";
			public const string ExternalLogin_LinkedIn = "ExternalLogin_LinkedIn";
			public const string ExternalLogin_Microsoft = "ExternalLogin_Microsoft";
			public const string ExternalLogin_SLTesting = "ExternalLogin_SLTesting";
			public const string ExternalLogin_Twitter = "ExternalLogin_Twitter";
			public const string UserInfo_Address1 = "UserInfo_Address1";
			public const string UserInfo_Address2 = "UserInfo_Address2";
			public const string UserInfo_Bio = "UserInfo_Bio";
			public const string UserInfo_City = "UserInfo_City";
			public const string UserInfo_Country = "UserInfo_Country";
			public const string UserInfo_CurrentOrganization = "UserInfo_CurrentOrganization";
			public const string UserInfo_DateCreated = "UserInfo_DateCreated";
			public const string UserInfo_Description = "UserInfo_Description";
			public const string UserInfo_Email = "UserInfo_Email";
			public const string UserInfo_EmailSignature = "UserInfo_EmailSignature";
			public const string UserInfo_FirstName = "UserInfo_FirstName";
			public const string UserInfo_Help = "UserInfo_Help";
			public const string UserInfo_IsAccountDisabled = "UserInfo_IsAccountDisabled";
			public const string UserInfo_IsAppBuilder = "UserInfo_IsAppBuilder";
			public const string UserInfo_IsEmailConfirmed = "UserInfo_IsEmailConfirmed";
			public const string UserInfo_IsOrgAdmin = "UserInfo_IsOrgAdmin";
			public const string UserInfo_IsOrgAdmin_Help = "UserInfo_IsOrgAdmin_Help";
			public const string UserInfo_IsPhoneConfirmed = "UserInfo_IsPhoneConfirmed";
			public const string UserInfo_IsPreviewUse_Help = "UserInfo_IsPreviewUse_Help";
			public const string UserInfo_IsPreviewUser = "UserInfo_IsPreviewUser";
			public const string UserInfo_IsRuntimeUser = "UserInfo_IsRuntimeUser";
			public const string UserInfo_IsSystemAdmin = "UserInfo_IsSystemAdmin";
			public const string UserInfo_IsSystemAdmin_Help = "UserInfo_IsSystemAdmin_Help";
			public const string UserInfo_IsUserDevice = "UserInfo_IsUserDevice";
			public const string UserInfo_LastName = "UserInfo_LastName";
			public const string UserInfo_Notes = "UserInfo_Notes";
			public const string UserInfo_ObjectTitle = "UserInfo_ObjectTitle";
			public const string UserInfo_PhoneNumber = "UserInfo_PhoneNumber";
			public const string UserInfo_PostalCode = "UserInfo_PostalCode";
			public const string UserInfo_ShowWelcome = "UserInfo_ShowWelcome";
			public const string UserInfo_SocialSecurityNumber = "UserInfo_SocialSecurityNumber";
			public const string UserInfo_SocialSecurityNumber_Help = "UserInfo_SocialSecurityNumber_Help";
			public const string UserInfo_StateProvince = "UserInfo_StateProvince";
			public const string UserInfo_TeamsAccountName = "UserInfo_TeamsAccountName";
			public const string UserInfo_Title = "UserInfo_Title";
			public const string UserInfo_UserName = "UserInfo_UserName";
		}
	}
	public class ComparatorResources
	{
        private static global::System.Resources.ResourceManager _resourceManager;
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static global::System.Resources.ResourceManager ResourceManager 
		{
            get 
			{
                if (object.ReferenceEquals(_resourceManager, null)) 
				{
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.Core.Resources.ComparatorResources", typeof(ComparatorResources).GetTypeInfo().Assembly);
                    _resourceManager = temp;
                }
                return _resourceManager;
            }
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static string GetResourceString(string key, params string[] tokens)
		{
			var culture = CultureInfo.CurrentCulture;;
            var str = ResourceManager.GetString(key, culture);

			for(int i = 0; i < tokens.Length; i += 2)
				str = str.Replace(tokens[i], tokens[i+1]);
										
            return str;
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
		/*
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static HtmlString GetResourceHtmlString(string key, params string[] tokens)
		{
			var str = GetResourceString(key, tokens);
							
			if(str.StartsWith("HTML:"))
				str = str.Substring(5);

			return new HtmlString(str);
        }*/
		
		public static string EmptyValue { get { return GetResourceString("EmptyValue"); } }
//Resources:ComparatorResources:Property_Empty_To_Value

		public static string Property_Empty_To_Value { get { return GetResourceString("Property_Empty_To_Value"); } }
//Resources:ComparatorResources:Property_Value_To_Empty

		public static string Property_Value_To_Empty { get { return GetResourceString("Property_Value_To_Empty"); } }
//Resources:ComparatorResources:PropertyValueChanged

		public static string PropertyValueChanged { get { return GetResourceString("PropertyValueChanged"); } }
//Resources:IPCameraResources:About

		public static class Names
		{
			public const string EmptyValue = "EmptyValue";
			public const string Property_Empty_To_Value = "Property_Empty_To_Value";
			public const string Property_Value_To_Empty = "Property_Value_To_Empty";
			public const string PropertyValueChanged = "PropertyValueChanged";
		}
	}
	public class IPCameraResources
	{
        private static global::System.Resources.ResourceManager _resourceManager;
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static global::System.Resources.ResourceManager ResourceManager 
		{
            get 
			{
                if (object.ReferenceEquals(_resourceManager, null)) 
				{
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.Core.Resources.IPCameraResources", typeof(IPCameraResources).GetTypeInfo().Assembly);
                    _resourceManager = temp;
                }
                return _resourceManager;
            }
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static string GetResourceString(string key, params string[] tokens)
		{
			var culture = CultureInfo.CurrentCulture;;
            var str = ResourceManager.GetString(key, culture);

			for(int i = 0; i < tokens.Length; i += 2)
				str = str.Replace(tokens[i], tokens[i+1]);
										
            return str;
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
		/*
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static HtmlString GetResourceHtmlString(string key, params string[] tokens)
		{
			var str = GetResourceString(key, tokens);
							
			if(str.StartsWith("HTML:"))
				str = str.Substring(5);

			return new HtmlString(str);
        }*/
		
		public static string About { get { return GetResourceString("About"); } }
//Resources:IPCameraResources:Add

		public static string Add { get { return GetResourceString("Add"); } }
//Resources:IPCameraResources:AddCameraTab

		public static string AddCameraTab { get { return GetResourceString("AddCameraTab"); } }
//Resources:IPCameraResources:AddToLockScreen

		public static string AddToLockScreen { get { return GetResourceString("AddToLockScreen"); } }
//Resources:IPCameraResources:AdvancedTab

		public static string AdvancedTab { get { return GetResourceString("AdvancedTab"); } }
//Resources:IPCameraResources:AppBarMenuItemAbout

		public static string AppBarMenuItemAbout { get { return GetResourceString("AppBarMenuItemAbout"); } }
//Resources:IPCameraResources:AppBarMenuItemSettings

		public static string AppBarMenuItemSettings { get { return GetResourceString("AppBarMenuItemSettings"); } }
//Resources:IPCameraResources:ApplicationTitle

		public static string ApplicationTitle { get { return GetResourceString("ApplicationTitle"); } }
//Resources:IPCameraResources:AutomaticallyUpdateTile

		public static string AutomaticallyUpdateTile { get { return GetResourceString("AutomaticallyUpdateTile"); } }
//Resources:IPCameraResources:Brightness

		public static string Brightness { get { return GetResourceString("Brightness"); } }
//Resources:IPCameraResources:CameraAlreadyPinned

		public static string CameraAlreadyPinned { get { return GetResourceString("CameraAlreadyPinned"); } }
//Resources:IPCameraResources:CameraImageSetToLockScreen

		public static string CameraImageSetToLockScreen { get { return GetResourceString("CameraImageSetToLockScreen"); } }
//Resources:IPCameraResources:CameraMfg

		public static string CameraMfg { get { return GetResourceString("CameraMfg"); } }
//Resources:IPCameraResources:CameraModel

		public static string CameraModel { get { return GetResourceString("CameraModel"); } }
//Resources:IPCameraResources:CameraName

		public static string CameraName { get { return GetResourceString("CameraName"); } }
//Resources:IPCameraResources:CameraNameIsRequired

		public static string CameraNameIsRequired { get { return GetResourceString("CameraNameIsRequired"); } }
//Resources:IPCameraResources:CameraPatrol

		public static string CameraPatrol { get { return GetResourceString("CameraPatrol"); } }
//Resources:IPCameraResources:CameraPresets

		public static string CameraPresets { get { return GetResourceString("CameraPresets"); } }
//Resources:IPCameraResources:CameraPreviewTab

		public static string CameraPreviewTab { get { return GetResourceString("CameraPreviewTab"); } }
//Resources:IPCameraResources:CameraUrl

		public static string CameraUrl { get { return GetResourceString("CameraUrl"); } }
//Resources:IPCameraResources:CanNotConnectMessage

		public static string CanNotConnectMessage { get { return GetResourceString("CanNotConnectMessage"); } }
//Resources:IPCameraResources:ClearDefaults

		public static string ClearDefaults { get { return GetResourceString("ClearDefaults"); } }
//Resources:IPCameraResources:ConfigureLockScreen

		public static string ConfigureLockScreen { get { return GetResourceString("ConfigureLockScreen"); } }
//Resources:IPCameraResources:Connecting

		public static string Connecting { get { return GetResourceString("Connecting"); } }
//Resources:IPCameraResources:Contrast

		public static string Contrast { get { return GetResourceString("Contrast"); } }
//Resources:IPCameraResources:CouldNotConnectTitle

		public static string CouldNotConnectTitle { get { return GetResourceString("CouldNotConnectTitle"); } }
//Resources:IPCameraResources:CreatedBy

		public static string CreatedBy { get { return GetResourceString("CreatedBy"); } }
//Resources:IPCameraResources:Credits

		public static string Credits { get { return GetResourceString("Credits"); } }
//Resources:IPCameraResources:Defaults

		public static string Defaults { get { return GetResourceString("Defaults"); } }
//Resources:IPCameraResources:Done

		public static string Done { get { return GetResourceString("Done"); } }
//Resources:IPCameraResources:EditPresetNames

		public static string EditPresetNames { get { return GetResourceString("EditPresetNames"); } }
//Resources:IPCameraResources:EditPresetNamesMenu

		public static string EditPresetNamesMenu { get { return GetResourceString("EditPresetNamesMenu"); } }
//Resources:IPCameraResources:ErrorConnectingToCamera

		public static string ErrorConnectingToCamera { get { return GetResourceString("ErrorConnectingToCamera"); } }
//Resources:IPCameraResources:FinishTab

		public static string FinishTab { get { return GetResourceString("FinishTab"); } }
//Resources:IPCameraResources:Flip

		public static string Flip { get { return GetResourceString("Flip"); } }
//Resources:IPCameraResources:GotoPreset

		public static string GotoPreset { get { return GetResourceString("GotoPreset"); } }
//Resources:IPCameraResources:InvertTiltControls

		public static string InvertTiltControls { get { return GetResourceString("InvertTiltControls"); } }
//Resources:IPCameraResources:MainScreenName

		public static string MainScreenName { get { return GetResourceString("MainScreenName"); } }
//Resources:IPCameraResources:MJpegdecoder

		public static string MJpegdecoder { get { return GetResourceString("MJpegdecoder"); } }
//Resources:IPCameraResources:No

		public static string No { get { return GetResourceString("No"); } }
//Resources:IPCameraResources:NoInternetMessage

		public static string NoInternetMessage { get { return GetResourceString("NoInternetMessage"); } }
//Resources:IPCameraResources:NotNow

		public static string NotNow { get { return GetResourceString("NotNow"); } }
//Resources:IPCameraResources:OnlineHelp

		public static string OnlineHelp { get { return GetResourceString("OnlineHelp"); } }
//Resources:IPCameraResources:Optional

		public static string Optional { get { return GetResourceString("Optional"); } }
//Resources:IPCameraResources:Password

		public static string Password { get { return GetResourceString("Password"); } }
//Resources:IPCameraResources:Pin

		public static string Pin { get { return GetResourceString("Pin"); } }
//Resources:IPCameraResources:PortNumber

		public static string PortNumber { get { return GetResourceString("PortNumber"); } }
//Resources:IPCameraResources:PortNumberBetween1And100

		public static string PortNumberBetween1And100 { get { return GetResourceString("PortNumberBetween1And100"); } }
//Resources:IPCameraResources:PreparingImage

		public static string PreparingImage { get { return GetResourceString("PreparingImage"); } }
//Resources:IPCameraResources:PresetFive

		public static string PresetFive { get { return GetResourceString("PresetFive"); } }
//Resources:IPCameraResources:PresetFour

		public static string PresetFour { get { return GetResourceString("PresetFour"); } }
//Resources:IPCameraResources:PresetOne

		public static string PresetOne { get { return GetResourceString("PresetOne"); } }
//Resources:IPCameraResources:PresetSix

		public static string PresetSix { get { return GetResourceString("PresetSix"); } }
//Resources:IPCameraResources:PresetThree

		public static string PresetThree { get { return GetResourceString("PresetThree"); } }
//Resources:IPCameraResources:PresetTwo

		public static string PresetTwo { get { return GetResourceString("PresetTwo"); } }
//Resources:IPCameraResources:Privacy

		public static string Privacy { get { return GetResourceString("Privacy"); } }
//Resources:IPCameraResources:PublishDate

		public static string PublishDate { get { return GetResourceString("PublishDate"); } }
//Resources:IPCameraResources:PurchaseOptions

		public static string PurchaseOptions { get { return GetResourceString("PurchaseOptions"); } }
//Resources:IPCameraResources:RateApp

		public static string RateApp { get { return GetResourceString("RateApp"); } }
//Resources:IPCameraResources:RatingMessage

		public static string RatingMessage { get { return GetResourceString("RatingMessage"); } }
//Resources:IPCameraResources:RequestSupport

		public static string RequestSupport { get { return GetResourceString("RequestSupport"); } }
//Resources:IPCameraResources:RequestSupportHeader

		public static string RequestSupportHeader { get { return GetResourceString("RequestSupportHeader"); } }
//Resources:IPCameraResources:Resolution

		public static string Resolution { get { return GetResourceString("Resolution"); } }
//Resources:IPCameraResources:ResourceFlowDirection

		public static string ResourceFlowDirection { get { return GetResourceString("ResourceFlowDirection"); } }
//Resources:IPCameraResources:ResourceLanguage

		public static string ResourceLanguage { get { return GetResourceString("ResourceLanguage"); } }
//Resources:IPCameraResources:SaveDefaults

		public static string SaveDefaults { get { return GetResourceString("SaveDefaults"); } }
//Resources:IPCameraResources:SaveNewCamera

		public static string SaveNewCamera { get { return GetResourceString("SaveNewCamera"); } }
//Resources:IPCameraResources:SaveSettingsByDefault

		public static string SaveSettingsByDefault { get { return GetResourceString("SaveSettingsByDefault"); } }
//Resources:IPCameraResources:Saving

		public static string Saving { get { return GetResourceString("Saving"); } }
//Resources:IPCameraResources:SendEmail

		public static string SendEmail { get { return GetResourceString("SendEmail"); } }
//Resources:IPCameraResources:SendFeedback

		public static string SendFeedback { get { return GetResourceString("SendFeedback"); } }
//Resources:IPCameraResources:SetPreset

		public static string SetPreset { get { return GetResourceString("SetPreset"); } }
//Resources:IPCameraResources:Settings

		public static string Settings { get { return GetResourceString("Settings"); } }
//Resources:IPCameraResources:SettingsPageName

		public static string SettingsPageName { get { return GetResourceString("SettingsPageName"); } }
//Resources:IPCameraResources:Share

		public static string Share { get { return GetResourceString("Share"); } }
//Resources:IPCameraResources:String

		public static string String { get { return GetResourceString("String"); } }
//Resources:IPCameraResources:String1

		public static string String1 { get { return GetResourceString("String1"); } }
//Resources:IPCameraResources:Summary

		public static string Summary { get { return GetResourceString("Summary"); } }
//Resources:IPCameraResources:SupportPanTilt

		public static string SupportPanTilt { get { return GetResourceString("SupportPanTilt"); } }
//Resources:IPCameraResources:TroubleShooting

		public static string TroubleShooting { get { return GetResourceString("TroubleShooting"); } }
//Resources:IPCameraResources:TroubleShootingIntro

		public static string TroubleShootingIntro { get { return GetResourceString("TroubleShootingIntro"); } }
//Resources:IPCameraResources:TroubleShootingStep1

		public static string TroubleShootingStep1 { get { return GetResourceString("TroubleShootingStep1"); } }
//Resources:IPCameraResources:TroubleShootingStep2

		public static string TroubleShootingStep2 { get { return GetResourceString("TroubleShootingStep2"); } }
//Resources:IPCameraResources:UIControls

		public static string UIControls { get { return GetResourceString("UIControls"); } }
//Resources:IPCameraResources:Unlimited

		public static string Unlimited { get { return GetResourceString("Unlimited"); } }
//Resources:IPCameraResources:UpdateTileInBackground

		public static string UpdateTileInBackground { get { return GetResourceString("UpdateTileInBackground"); } }
//Resources:IPCameraResources:UpdatingInBackground

		public static string UpdatingInBackground { get { return GetResourceString("UpdatingInBackground"); } }
//Resources:IPCameraResources:UserMode

		public static string UserMode { get { return GetResourceString("UserMode"); } }
//Resources:IPCameraResources:UserModeAdmin

		public static string UserModeAdmin { get { return GetResourceString("UserModeAdmin"); } }
//Resources:IPCameraResources:UserModeGuest

		public static string UserModeGuest { get { return GetResourceString("UserModeGuest"); } }
//Resources:IPCameraResources:UserModeOperator

		public static string UserModeOperator { get { return GetResourceString("UserModeOperator"); } }
//Resources:IPCameraResources:UserName

		public static string UserName { get { return GetResourceString("UserName"); } }
//Resources:IPCameraResources:Version

		public static string Version { get { return GetResourceString("Version"); } }
//Resources:IPCameraResources:View3Cameras

		public static string View3Cameras { get { return GetResourceString("View3Cameras"); } }
//Resources:IPCameraResources:ViewableCameras

		public static string ViewableCameras { get { return GetResourceString("ViewableCameras"); } }
//Resources:IPCameraResources:ViewAdditionalCameras

		public static string ViewAdditionalCameras { get { return GetResourceString("ViewAdditionalCameras"); } }
//Resources:IPCameraResources:ViewMoreInstructionsOne

		public static string ViewMoreInstructionsOne { get { return GetResourceString("ViewMoreInstructionsOne"); } }
//Resources:IPCameraResources:ViewMoreInstructionsTwo

		public static string ViewMoreInstructionsTwo { get { return GetResourceString("ViewMoreInstructionsTwo"); } }
//Resources:IPCameraResources:ViewUnlimitedCameras

		public static string ViewUnlimitedCameras { get { return GetResourceString("ViewUnlimitedCameras"); } }
//Resources:IPCameraResources:Yes

		public static string Yes { get { return GetResourceString("Yes"); } }
//Resources:IPCameraResources:YouCanView

		public static string YouCanView { get { return GetResourceString("YouCanView"); } }
//Resources:LagoVistaCommonStrings:Address_Address1

		public static class Names
		{
			public const string About = "About";
			public const string Add = "Add";
			public const string AddCameraTab = "AddCameraTab";
			public const string AddToLockScreen = "AddToLockScreen";
			public const string AdvancedTab = "AdvancedTab";
			public const string AppBarMenuItemAbout = "AppBarMenuItemAbout";
			public const string AppBarMenuItemSettings = "AppBarMenuItemSettings";
			public const string ApplicationTitle = "ApplicationTitle";
			public const string AutomaticallyUpdateTile = "AutomaticallyUpdateTile";
			public const string Brightness = "Brightness";
			public const string CameraAlreadyPinned = "CameraAlreadyPinned";
			public const string CameraImageSetToLockScreen = "CameraImageSetToLockScreen";
			public const string CameraMfg = "CameraMfg";
			public const string CameraModel = "CameraModel";
			public const string CameraName = "CameraName";
			public const string CameraNameIsRequired = "CameraNameIsRequired";
			public const string CameraPatrol = "CameraPatrol";
			public const string CameraPresets = "CameraPresets";
			public const string CameraPreviewTab = "CameraPreviewTab";
			public const string CameraUrl = "CameraUrl";
			public const string CanNotConnectMessage = "CanNotConnectMessage";
			public const string ClearDefaults = "ClearDefaults";
			public const string ConfigureLockScreen = "ConfigureLockScreen";
			public const string Connecting = "Connecting";
			public const string Contrast = "Contrast";
			public const string CouldNotConnectTitle = "CouldNotConnectTitle";
			public const string CreatedBy = "CreatedBy";
			public const string Credits = "Credits";
			public const string Defaults = "Defaults";
			public const string Done = "Done";
			public const string EditPresetNames = "EditPresetNames";
			public const string EditPresetNamesMenu = "EditPresetNamesMenu";
			public const string ErrorConnectingToCamera = "ErrorConnectingToCamera";
			public const string FinishTab = "FinishTab";
			public const string Flip = "Flip";
			public const string GotoPreset = "GotoPreset";
			public const string InvertTiltControls = "InvertTiltControls";
			public const string MainScreenName = "MainScreenName";
			public const string MJpegdecoder = "MJpegdecoder";
			public const string No = "No";
			public const string NoInternetMessage = "NoInternetMessage";
			public const string NotNow = "NotNow";
			public const string OnlineHelp = "OnlineHelp";
			public const string Optional = "Optional";
			public const string Password = "Password";
			public const string Pin = "Pin";
			public const string PortNumber = "PortNumber";
			public const string PortNumberBetween1And100 = "PortNumberBetween1And100";
			public const string PreparingImage = "PreparingImage";
			public const string PresetFive = "PresetFive";
			public const string PresetFour = "PresetFour";
			public const string PresetOne = "PresetOne";
			public const string PresetSix = "PresetSix";
			public const string PresetThree = "PresetThree";
			public const string PresetTwo = "PresetTwo";
			public const string Privacy = "Privacy";
			public const string PublishDate = "PublishDate";
			public const string PurchaseOptions = "PurchaseOptions";
			public const string RateApp = "RateApp";
			public const string RatingMessage = "RatingMessage";
			public const string RequestSupport = "RequestSupport";
			public const string RequestSupportHeader = "RequestSupportHeader";
			public const string Resolution = "Resolution";
			public const string ResourceFlowDirection = "ResourceFlowDirection";
			public const string ResourceLanguage = "ResourceLanguage";
			public const string SaveDefaults = "SaveDefaults";
			public const string SaveNewCamera = "SaveNewCamera";
			public const string SaveSettingsByDefault = "SaveSettingsByDefault";
			public const string Saving = "Saving";
			public const string SendEmail = "SendEmail";
			public const string SendFeedback = "SendFeedback";
			public const string SetPreset = "SetPreset";
			public const string Settings = "Settings";
			public const string SettingsPageName = "SettingsPageName";
			public const string Share = "Share";
			public const string String = "String";
			public const string String1 = "String1";
			public const string Summary = "Summary";
			public const string SupportPanTilt = "SupportPanTilt";
			public const string TroubleShooting = "TroubleShooting";
			public const string TroubleShootingIntro = "TroubleShootingIntro";
			public const string TroubleShootingStep1 = "TroubleShootingStep1";
			public const string TroubleShootingStep2 = "TroubleShootingStep2";
			public const string UIControls = "UIControls";
			public const string Unlimited = "Unlimited";
			public const string UpdateTileInBackground = "UpdateTileInBackground";
			public const string UpdatingInBackground = "UpdatingInBackground";
			public const string UserMode = "UserMode";
			public const string UserModeAdmin = "UserModeAdmin";
			public const string UserModeGuest = "UserModeGuest";
			public const string UserModeOperator = "UserModeOperator";
			public const string UserName = "UserName";
			public const string Version = "Version";
			public const string View3Cameras = "View3Cameras";
			public const string ViewableCameras = "ViewableCameras";
			public const string ViewAdditionalCameras = "ViewAdditionalCameras";
			public const string ViewMoreInstructionsOne = "ViewMoreInstructionsOne";
			public const string ViewMoreInstructionsTwo = "ViewMoreInstructionsTwo";
			public const string ViewUnlimitedCameras = "ViewUnlimitedCameras";
			public const string Yes = "Yes";
			public const string YouCanView = "YouCanView";
		}
	}
	public class LagoVistaCommonStrings
	{
        private static global::System.Resources.ResourceManager _resourceManager;
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static global::System.Resources.ResourceManager ResourceManager 
		{
            get 
			{
                if (object.ReferenceEquals(_resourceManager, null)) 
				{
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.Core.Resources.LagoVistaCommonStrings", typeof(LagoVistaCommonStrings).GetTypeInfo().Assembly);
                    _resourceManager = temp;
                }
                return _resourceManager;
            }
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static string GetResourceString(string key, params string[] tokens)
		{
			var culture = CultureInfo.CurrentCulture;;
            var str = ResourceManager.GetString(key, culture);

			for(int i = 0; i < tokens.Length; i += 2)
				str = str.Replace(tokens[i], tokens[i+1]);
										
            return str;
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
		/*
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static HtmlString GetResourceHtmlString(string key, params string[] tokens)
		{
			var str = GetResourceString(key, tokens);
							
			if(str.StartsWith("HTML:"))
				str = str.Substring(5);

			return new HtmlString(str);
        }*/
		
		public static string Address_Address1 { get { return GetResourceString("Address_Address1"); } }
//Resources:LagoVistaCommonStrings:Address_Address2

		public static string Address_Address2 { get { return GetResourceString("Address_Address2"); } }
//Resources:LagoVistaCommonStrings:Address_AddressType

		public static string Address_AddressType { get { return GetResourceString("Address_AddressType"); } }
//Resources:LagoVistaCommonStrings:Address_AddressType_Business

		public static string Address_AddressType_Business { get { return GetResourceString("Address_AddressType_Business"); } }
//Resources:LagoVistaCommonStrings:Address_AddressType_Other

		public static string Address_AddressType_Other { get { return GetResourceString("Address_AddressType_Other"); } }
//Resources:LagoVistaCommonStrings:Address_AddressType_Residential

		public static string Address_AddressType_Residential { get { return GetResourceString("Address_AddressType_Residential"); } }
//Resources:LagoVistaCommonStrings:Address_AddressType_Select

		public static string Address_AddressType_Select { get { return GetResourceString("Address_AddressType_Select"); } }
//Resources:LagoVistaCommonStrings:Address_City

		public static string Address_City { get { return GetResourceString("Address_City"); } }
//Resources:LagoVistaCommonStrings:Address_Country

		public static string Address_Country { get { return GetResourceString("Address_Country"); } }
//Resources:LagoVistaCommonStrings:Address_Description

		public static string Address_Description { get { return GetResourceString("Address_Description"); } }
//Resources:LagoVistaCommonStrings:Address_GeoLocation

		public static string Address_GeoLocation { get { return GetResourceString("Address_GeoLocation"); } }
//Resources:LagoVistaCommonStrings:Address_Help

		public static string Address_Help { get { return GetResourceString("Address_Help"); } }
//Resources:LagoVistaCommonStrings:Address_PostalCode

		public static string Address_PostalCode { get { return GetResourceString("Address_PostalCode"); } }
//Resources:LagoVistaCommonStrings:Address_StateOrProvince

		public static string Address_StateOrProvince { get { return GetResourceString("Address_StateOrProvince"); } }
//Resources:LagoVistaCommonStrings:Address_Title

		public static string Address_Title { get { return GetResourceString("Address_Title"); } }
//Resources:LagoVistaCommonStrings:Address_UnitNumber

		public static string Address_UnitNumber { get { return GetResourceString("Address_UnitNumber"); } }
//Resources:LagoVistaCommonStrings:Address_UnitNumber_Help

		public static string Address_UnitNumber_Help { get { return GetResourceString("Address_UnitNumber_Help"); } }
//Resources:LagoVistaCommonStrings:Common_Category

		public static string Common_Category { get { return GetResourceString("Common_Category"); } }
//Resources:LagoVistaCommonStrings:Common_Category_Select

		public static string Common_Category_Select { get { return GetResourceString("Common_Category_Select"); } }
//Resources:LagoVistaCommonStrings:Common_CreatedBy

		public static string Common_CreatedBy { get { return GetResourceString("Common_CreatedBy"); } }
//Resources:LagoVistaCommonStrings:Common_CreationDate

		public static string Common_CreationDate { get { return GetResourceString("Common_CreationDate"); } }
//Resources:LagoVistaCommonStrings:Common_Description

		public static string Common_Description { get { return GetResourceString("Common_Description"); } }
//Resources:LagoVistaCommonStrings:Common_IsPublic

		public static string Common_IsPublic { get { return GetResourceString("Common_IsPublic"); } }
//Resources:LagoVistaCommonStrings:Common_Key

		public static string Common_Key { get { return GetResourceString("Common_Key"); } }
//Resources:LagoVistaCommonStrings:Common_Key_Help

		public static string Common_Key_Help { get { return GetResourceString("Common_Key_Help"); } }
//Resources:LagoVistaCommonStrings:Common_Key_Validation

		public static string Common_Key_Validation { get { return GetResourceString("Common_Key_Validation"); } }
//Resources:LagoVistaCommonStrings:Common_Language

		public static string Common_Language { get { return GetResourceString("Common_Language"); } }
//Resources:LagoVistaCommonStrings:Common_Language_Select

		public static string Common_Language_Select { get { return GetResourceString("Common_Language_Select"); } }
//Resources:LagoVistaCommonStrings:Common_LastUpdated

		public static string Common_LastUpdated { get { return GetResourceString("Common_LastUpdated"); } }
//Resources:LagoVistaCommonStrings:Common_LastUpdatedBy

		public static string Common_LastUpdatedBy { get { return GetResourceString("Common_LastUpdatedBy"); } }
//Resources:LagoVistaCommonStrings:Common_Loading

		public static string Common_Loading { get { return GetResourceString("Common_Loading"); } }
//Resources:LagoVistaCommonStrings:Common_Month

		public static string Common_Month { get { return GetResourceString("Common_Month"); } }
//Resources:LagoVistaCommonStrings:Common_Month_Select

		public static string Common_Month_Select { get { return GetResourceString("Common_Month_Select"); } }
//Resources:LagoVistaCommonStrings:Common_Name

		public static string Common_Name { get { return GetResourceString("Common_Name"); } }
//Resources:LagoVistaCommonStrings:Common_No

		public static string Common_No { get { return GetResourceString("Common_No"); } }
//Resources:LagoVistaCommonStrings:Common_Notes

		public static string Common_Notes { get { return GetResourceString("Common_Notes"); } }
//Resources:LagoVistaCommonStrings:Common_PleaseWait

		public static string Common_PleaseWait { get { return GetResourceString("Common_PleaseWait"); } }
//Resources:LagoVistaCommonStrings:Common_Select

		public static string Common_Select { get { return GetResourceString("Common_Select"); } }
//Resources:LagoVistaCommonStrings:Common_Summary

		public static string Common_Summary { get { return GetResourceString("Common_Summary"); } }
//Resources:LagoVistaCommonStrings:Common_Year

		public static string Common_Year { get { return GetResourceString("Common_Year"); } }
//Resources:LagoVistaCommonStrings:Common_Year_Select

		public static string Common_Year_Select { get { return GetResourceString("Common_Year_Select"); } }
//Resources:LagoVistaCommonStrings:Common_Yes

		public static string Common_Yes { get { return GetResourceString("Common_Yes"); } }
//Resources:LagoVistaCommonStrings:Company_Description

		public static string Company_Description { get { return GetResourceString("Company_Description"); } }
//Resources:LagoVistaCommonStrings:Company_Help

		public static string Company_Help { get { return GetResourceString("Company_Help"); } }
//Resources:LagoVistaCommonStrings:Company_Industry

		public static string Company_Industry { get { return GetResourceString("Company_Industry"); } }
//Resources:LagoVistaCommonStrings:Company_IndustryNiche

		public static string Company_IndustryNiche { get { return GetResourceString("Company_IndustryNiche"); } }
//Resources:LagoVistaCommonStrings:Company_Title

		public static string Company_Title { get { return GetResourceString("Company_Title"); } }
//Resources:LagoVistaCommonStrings:Contact_Description

		public static string Contact_Description { get { return GetResourceString("Contact_Description"); } }
//Resources:LagoVistaCommonStrings:Contact_Email

		public static string Contact_Email { get { return GetResourceString("Contact_Email"); } }
//Resources:LagoVistaCommonStrings:Contact_FirstName

		public static string Contact_FirstName { get { return GetResourceString("Contact_FirstName"); } }
//Resources:LagoVistaCommonStrings:Contact_Help

		public static string Contact_Help { get { return GetResourceString("Contact_Help"); } }
//Resources:LagoVistaCommonStrings:Contact_LastName

		public static string Contact_LastName { get { return GetResourceString("Contact_LastName"); } }
//Resources:LagoVistaCommonStrings:Contact_LinkedInPage

		public static string Contact_LinkedInPage { get { return GetResourceString("Contact_LinkedInPage"); } }
//Resources:LagoVistaCommonStrings:Contact_Notes

		public static string Contact_Notes { get { return GetResourceString("Contact_Notes"); } }
//Resources:LagoVistaCommonStrings:Contact_Phone

		public static string Contact_Phone { get { return GetResourceString("Contact_Phone"); } }
//Resources:LagoVistaCommonStrings:Contact_Title

		public static string Contact_Title { get { return GetResourceString("Contact_Title"); } }
//Resources:LagoVistaCommonStrings:Customer_Address1

		public static string Customer_Address1 { get { return GetResourceString("Customer_Address1"); } }
//Resources:LagoVistaCommonStrings:Customer_Address2

		public static string Customer_Address2 { get { return GetResourceString("Customer_Address2"); } }
//Resources:LagoVistaCommonStrings:Customer_City

		public static string Customer_City { get { return GetResourceString("Customer_City"); } }
//Resources:LagoVistaCommonStrings:Customer_Notes

		public static string Customer_Notes { get { return GetResourceString("Customer_Notes"); } }
//Resources:LagoVistaCommonStrings:Customer_State

		public static string Customer_State { get { return GetResourceString("Customer_State"); } }
//Resources:LagoVistaCommonStrings:Customer_WebSite

		public static string Customer_WebSite { get { return GetResourceString("Customer_WebSite"); } }
//Resources:LagoVistaCommonStrings:Customer_ZipCode

		public static string Customer_ZipCode { get { return GetResourceString("Customer_ZipCode"); } }
//Resources:LagoVistaCommonStrings:Discussion

		public static string Discussion { get { return GetResourceString("Discussion"); } }
//Resources:LagoVistaCommonStrings:Discussion_Help

		public static string Discussion_Help { get { return GetResourceString("Discussion_Help"); } }
//Resources:LagoVistaCommonStrings:Discussion_TimeStamp

		public static string Discussion_TimeStamp { get { return GetResourceString("Discussion_TimeStamp"); } }
//Resources:LagoVistaCommonStrings:Discussion_Title

		public static string Discussion_Title { get { return GetResourceString("Discussion_Title"); } }
//Resources:LagoVistaCommonStrings:ErrorMakingCall

		public static string ErrorMakingCall { get { return GetResourceString("ErrorMakingCall"); } }
//Resources:LagoVistaCommonStrings:ListResponse_Category

		public static string ListResponse_Category { get { return GetResourceString("ListResponse_Category"); } }
//Resources:LagoVistaCommonStrings:ListResponse_Deleted

		public static string ListResponse_Deleted { get { return GetResourceString("ListResponse_Deleted"); } }
//Resources:LagoVistaCommonStrings:Month_April

		public static string Month_April { get { return GetResourceString("Month_April"); } }
//Resources:LagoVistaCommonStrings:Month_August

		public static string Month_August { get { return GetResourceString("Month_August"); } }
//Resources:LagoVistaCommonStrings:Month_December

		public static string Month_December { get { return GetResourceString("Month_December"); } }
//Resources:LagoVistaCommonStrings:Month_February

		public static string Month_February { get { return GetResourceString("Month_February"); } }
//Resources:LagoVistaCommonStrings:Month_January

		public static string Month_January { get { return GetResourceString("Month_January"); } }
//Resources:LagoVistaCommonStrings:Month_July

		public static string Month_July { get { return GetResourceString("Month_July"); } }
//Resources:LagoVistaCommonStrings:Month_June

		public static string Month_June { get { return GetResourceString("Month_June"); } }
//Resources:LagoVistaCommonStrings:Month_March

		public static string Month_March { get { return GetResourceString("Month_March"); } }
//Resources:LagoVistaCommonStrings:Month_May

		public static string Month_May { get { return GetResourceString("Month_May"); } }
//Resources:LagoVistaCommonStrings:Month_November

		public static string Month_November { get { return GetResourceString("Month_November"); } }
//Resources:LagoVistaCommonStrings:Month_October

		public static string Month_October { get { return GetResourceString("Month_October"); } }
//Resources:LagoVistaCommonStrings:Month_September

		public static string Month_September { get { return GetResourceString("Month_September"); } }
//Resources:LagoVistaCommonStrings:NoConnection

		public static string NoConnection { get { return GetResourceString("NoConnection"); } }
//Resources:LagoVistaCommonStrings:Schedule_Active

		public static string Schedule_Active { get { return GetResourceString("Schedule_Active"); } }
//Resources:LagoVistaCommonStrings:Schedule_Biweekly

		public static string Schedule_Biweekly { get { return GetResourceString("Schedule_Biweekly"); } }
//Resources:LagoVistaCommonStrings:Schedule_Daily

		public static string Schedule_Daily { get { return GetResourceString("Schedule_Daily"); } }
//Resources:LagoVistaCommonStrings:Schedule_DayOfWeek

		public static string Schedule_DayOfWeek { get { return GetResourceString("Schedule_DayOfWeek"); } }
//Resources:LagoVistaCommonStrings:Schedule_Description

		public static string Schedule_Description { get { return GetResourceString("Schedule_Description"); } }
//Resources:LagoVistaCommonStrings:Schedule_EndsOn

		public static string Schedule_EndsOn { get { return GetResourceString("Schedule_EndsOn"); } }
//Resources:LagoVistaCommonStrings:Schedule_FirstWeek

		public static string Schedule_FirstWeek { get { return GetResourceString("Schedule_FirstWeek"); } }
//Resources:LagoVistaCommonStrings:Schedule_FourthWeek

		public static string Schedule_FourthWeek { get { return GetResourceString("Schedule_FourthWeek"); } }
//Resources:LagoVistaCommonStrings:Schedule_Friday

		public static string Schedule_Friday { get { return GetResourceString("Schedule_Friday"); } }
//Resources:LagoVistaCommonStrings:Schedule_Monday

		public static string Schedule_Monday { get { return GetResourceString("Schedule_Monday"); } }
//Resources:LagoVistaCommonStrings:Schedule_Monthly

		public static string Schedule_Monthly { get { return GetResourceString("Schedule_Monthly"); } }
//Resources:LagoVistaCommonStrings:Schedule_Saturday

		public static string Schedule_Saturday { get { return GetResourceString("Schedule_Saturday"); } }
//Resources:LagoVistaCommonStrings:Schedule_ScheduleType

		public static string Schedule_ScheduleType { get { return GetResourceString("Schedule_ScheduleType"); } }
//Resources:LagoVistaCommonStrings:Schedule_ScheduleType_Biweekly

		public static string Schedule_ScheduleType_Biweekly { get { return GetResourceString("Schedule_ScheduleType_Biweekly"); } }
//Resources:LagoVistaCommonStrings:Schedule_ScheduleType_Daily

		public static string Schedule_ScheduleType_Daily { get { return GetResourceString("Schedule_ScheduleType_Daily"); } }
//Resources:LagoVistaCommonStrings:Schedule_ScheduleType_FirstDayOfMonth

		public static string Schedule_ScheduleType_FirstDayOfMonth { get { return GetResourceString("Schedule_ScheduleType_FirstDayOfMonth"); } }
//Resources:LagoVistaCommonStrings:Schedule_ScheduleType_LastDayOfMonth

		public static string Schedule_ScheduleType_LastDayOfMonth { get { return GetResourceString("Schedule_ScheduleType_LastDayOfMonth"); } }
//Resources:LagoVistaCommonStrings:Schedule_ScheduleType_Monthly

		public static string Schedule_ScheduleType_Monthly { get { return GetResourceString("Schedule_ScheduleType_Monthly"); } }
//Resources:LagoVistaCommonStrings:Schedule_ScheduleType_Select

		public static string Schedule_ScheduleType_Select { get { return GetResourceString("Schedule_ScheduleType_Select"); } }
//Resources:LagoVistaCommonStrings:Schedule_ScheduleType_Weekly

		public static string Schedule_ScheduleType_Weekly { get { return GetResourceString("Schedule_ScheduleType_Weekly"); } }
//Resources:LagoVistaCommonStrings:Schedule_SecondWeek

		public static string Schedule_SecondWeek { get { return GetResourceString("Schedule_SecondWeek"); } }
//Resources:LagoVistaCommonStrings:Schedule_SelectWeek

		public static string Schedule_SelectWeek { get { return GetResourceString("Schedule_SelectWeek"); } }
//Resources:LagoVistaCommonStrings:Schedule_StartsOn

		public static string Schedule_StartsOn { get { return GetResourceString("Schedule_StartsOn"); } }
//Resources:LagoVistaCommonStrings:Schedule_StartTime

		public static string Schedule_StartTime { get { return GetResourceString("Schedule_StartTime"); } }
//Resources:LagoVistaCommonStrings:Schedule_Sunday

		public static string Schedule_Sunday { get { return GetResourceString("Schedule_Sunday"); } }
//Resources:LagoVistaCommonStrings:Schedule_ThirdWeek

		public static string Schedule_ThirdWeek { get { return GetResourceString("Schedule_ThirdWeek"); } }
//Resources:LagoVistaCommonStrings:Schedule_Thursday

		public static string Schedule_Thursday { get { return GetResourceString("Schedule_Thursday"); } }
//Resources:LagoVistaCommonStrings:Schedule_Title

		public static string Schedule_Title { get { return GetResourceString("Schedule_Title"); } }
//Resources:LagoVistaCommonStrings:Schedule_Tuesday

		public static string Schedule_Tuesday { get { return GetResourceString("Schedule_Tuesday"); } }
//Resources:LagoVistaCommonStrings:Schedule_Wednesday

		public static string Schedule_Wednesday { get { return GetResourceString("Schedule_Wednesday"); } }
//Resources:LagoVistaCommonStrings:Schedule_Week

		public static string Schedule_Week { get { return GetResourceString("Schedule_Week"); } }
//Resources:LagoVistaCommonStrings:Schedule_Weekly

		public static string Schedule_Weekly { get { return GetResourceString("Schedule_Weekly"); } }
//Resources:LagoVistaCommonStrings:ValidationCheck_Errors

		public static string ValidationCheck_Errors { get { return GetResourceString("ValidationCheck_Errors"); } }
//Resources:ValidationResource:Common_Key_Help

		public static class Names
		{
			public const string Address_Address1 = "Address_Address1";
			public const string Address_Address2 = "Address_Address2";
			public const string Address_AddressType = "Address_AddressType";
			public const string Address_AddressType_Business = "Address_AddressType_Business";
			public const string Address_AddressType_Other = "Address_AddressType_Other";
			public const string Address_AddressType_Residential = "Address_AddressType_Residential";
			public const string Address_AddressType_Select = "Address_AddressType_Select";
			public const string Address_City = "Address_City";
			public const string Address_Country = "Address_Country";
			public const string Address_Description = "Address_Description";
			public const string Address_GeoLocation = "Address_GeoLocation";
			public const string Address_Help = "Address_Help";
			public const string Address_PostalCode = "Address_PostalCode";
			public const string Address_StateOrProvince = "Address_StateOrProvince";
			public const string Address_Title = "Address_Title";
			public const string Address_UnitNumber = "Address_UnitNumber";
			public const string Address_UnitNumber_Help = "Address_UnitNumber_Help";
			public const string Common_Category = "Common_Category";
			public const string Common_Category_Select = "Common_Category_Select";
			public const string Common_CreatedBy = "Common_CreatedBy";
			public const string Common_CreationDate = "Common_CreationDate";
			public const string Common_Description = "Common_Description";
			public const string Common_IsPublic = "Common_IsPublic";
			public const string Common_Key = "Common_Key";
			public const string Common_Key_Help = "Common_Key_Help";
			public const string Common_Key_Validation = "Common_Key_Validation";
			public const string Common_Language = "Common_Language";
			public const string Common_Language_Select = "Common_Language_Select";
			public const string Common_LastUpdated = "Common_LastUpdated";
			public const string Common_LastUpdatedBy = "Common_LastUpdatedBy";
			public const string Common_Loading = "Common_Loading";
			public const string Common_Month = "Common_Month";
			public const string Common_Month_Select = "Common_Month_Select";
			public const string Common_Name = "Common_Name";
			public const string Common_No = "Common_No";
			public const string Common_Notes = "Common_Notes";
			public const string Common_PleaseWait = "Common_PleaseWait";
			public const string Common_Select = "Common_Select";
			public const string Common_Summary = "Common_Summary";
			public const string Common_Year = "Common_Year";
			public const string Common_Year_Select = "Common_Year_Select";
			public const string Common_Yes = "Common_Yes";
			public const string Company_Description = "Company_Description";
			public const string Company_Help = "Company_Help";
			public const string Company_Industry = "Company_Industry";
			public const string Company_IndustryNiche = "Company_IndustryNiche";
			public const string Company_Title = "Company_Title";
			public const string Contact_Description = "Contact_Description";
			public const string Contact_Email = "Contact_Email";
			public const string Contact_FirstName = "Contact_FirstName";
			public const string Contact_Help = "Contact_Help";
			public const string Contact_LastName = "Contact_LastName";
			public const string Contact_LinkedInPage = "Contact_LinkedInPage";
			public const string Contact_Notes = "Contact_Notes";
			public const string Contact_Phone = "Contact_Phone";
			public const string Contact_Title = "Contact_Title";
			public const string Customer_Address1 = "Customer_Address1";
			public const string Customer_Address2 = "Customer_Address2";
			public const string Customer_City = "Customer_City";
			public const string Customer_Notes = "Customer_Notes";
			public const string Customer_State = "Customer_State";
			public const string Customer_WebSite = "Customer_WebSite";
			public const string Customer_ZipCode = "Customer_ZipCode";
			public const string Discussion = "Discussion";
			public const string Discussion_Help = "Discussion_Help";
			public const string Discussion_TimeStamp = "Discussion_TimeStamp";
			public const string Discussion_Title = "Discussion_Title";
			public const string ErrorMakingCall = "ErrorMakingCall";
			public const string ListResponse_Category = "ListResponse_Category";
			public const string ListResponse_Deleted = "ListResponse_Deleted";
			public const string Month_April = "Month_April";
			public const string Month_August = "Month_August";
			public const string Month_December = "Month_December";
			public const string Month_February = "Month_February";
			public const string Month_January = "Month_January";
			public const string Month_July = "Month_July";
			public const string Month_June = "Month_June";
			public const string Month_March = "Month_March";
			public const string Month_May = "Month_May";
			public const string Month_November = "Month_November";
			public const string Month_October = "Month_October";
			public const string Month_September = "Month_September";
			public const string NoConnection = "NoConnection";
			public const string Schedule_Active = "Schedule_Active";
			public const string Schedule_Biweekly = "Schedule_Biweekly";
			public const string Schedule_Daily = "Schedule_Daily";
			public const string Schedule_DayOfWeek = "Schedule_DayOfWeek";
			public const string Schedule_Description = "Schedule_Description";
			public const string Schedule_EndsOn = "Schedule_EndsOn";
			public const string Schedule_FirstWeek = "Schedule_FirstWeek";
			public const string Schedule_FourthWeek = "Schedule_FourthWeek";
			public const string Schedule_Friday = "Schedule_Friday";
			public const string Schedule_Monday = "Schedule_Monday";
			public const string Schedule_Monthly = "Schedule_Monthly";
			public const string Schedule_Saturday = "Schedule_Saturday";
			public const string Schedule_ScheduleType = "Schedule_ScheduleType";
			public const string Schedule_ScheduleType_Biweekly = "Schedule_ScheduleType_Biweekly";
			public const string Schedule_ScheduleType_Daily = "Schedule_ScheduleType_Daily";
			public const string Schedule_ScheduleType_FirstDayOfMonth = "Schedule_ScheduleType_FirstDayOfMonth";
			public const string Schedule_ScheduleType_LastDayOfMonth = "Schedule_ScheduleType_LastDayOfMonth";
			public const string Schedule_ScheduleType_Monthly = "Schedule_ScheduleType_Monthly";
			public const string Schedule_ScheduleType_Select = "Schedule_ScheduleType_Select";
			public const string Schedule_ScheduleType_Weekly = "Schedule_ScheduleType_Weekly";
			public const string Schedule_SecondWeek = "Schedule_SecondWeek";
			public const string Schedule_SelectWeek = "Schedule_SelectWeek";
			public const string Schedule_StartsOn = "Schedule_StartsOn";
			public const string Schedule_StartTime = "Schedule_StartTime";
			public const string Schedule_Sunday = "Schedule_Sunday";
			public const string Schedule_ThirdWeek = "Schedule_ThirdWeek";
			public const string Schedule_Thursday = "Schedule_Thursday";
			public const string Schedule_Title = "Schedule_Title";
			public const string Schedule_Tuesday = "Schedule_Tuesday";
			public const string Schedule_Wednesday = "Schedule_Wednesday";
			public const string Schedule_Week = "Schedule_Week";
			public const string Schedule_Weekly = "Schedule_Weekly";
			public const string ValidationCheck_Errors = "ValidationCheck_Errors";
		}
	}
	public class ValidationResource
	{
        private static global::System.Resources.ResourceManager _resourceManager;
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static global::System.Resources.ResourceManager ResourceManager 
		{
            get 
			{
                if (object.ReferenceEquals(_resourceManager, null)) 
				{
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.Core.Resources.ValidationResource", typeof(ValidationResource).GetTypeInfo().Assembly);
                    _resourceManager = temp;
                }
                return _resourceManager;
            }
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static string GetResourceString(string key, params string[] tokens)
		{
			var culture = CultureInfo.CurrentCulture;;
            var str = ResourceManager.GetString(key, culture);

			for(int i = 0; i < tokens.Length; i += 2)
				str = str.Replace(tokens[i], tokens[i+1]);
										
            return str;
        }
        
        /// <summary>
        ///   Returns the formatted resource string.
        /// </summary>
		/*
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static HtmlString GetResourceHtmlString(string key, params string[] tokens)
		{
			var str = GetResourceString(key, tokens);
							
			if(str.StartsWith("HTML:"))
				str = str.Substring(5);

			return new HtmlString(str);
        }*/
		
		public static string Common_Key_Help { get { return GetResourceString("Common_Key_Help"); } }
//Resources:ValidationResource:Common_Key_Validation

		public static string Common_Key_Validation { get { return GetResourceString("Common_Key_Validation"); } }
//Resources:ValidationResource:Concurrency_Error

		public static string Concurrency_Error { get { return GetResourceString("Concurrency_Error"); } }
//Resources:ValidationResource:Concurrency_ErrorMessage

		public static string Concurrency_ErrorMessage { get { return GetResourceString("Concurrency_ErrorMessage"); } }
//Resources:ValidationResource:CreatedByIdInvalidFormat

		public static string CreatedByIdInvalidFormat { get { return GetResourceString("CreatedByIdInvalidFormat"); } }
//Resources:ValidationResource:CreatedByIdNotNullOrEmpty

		public static string CreatedByIdNotNullOrEmpty { get { return GetResourceString("CreatedByIdNotNullOrEmpty"); } }
//Resources:ValidationResource:CreatedByNotNull

		public static string CreatedByNotNull { get { return GetResourceString("CreatedByNotNull"); } }
//Resources:ValidationResource:CreatedByTextNotNullOrEmpty

		public static string CreatedByTextNotNullOrEmpty { get { return GetResourceString("CreatedByTextNotNullOrEmpty"); } }
//Resources:ValidationResource:CreationDateInvalidFormat

		public static string CreationDateInvalidFormat { get { return GetResourceString("CreationDateInvalidFormat"); } }
//Resources:ValidationResource:CreationDateRequired

		public static string CreationDateRequired { get { return GetResourceString("CreationDateRequired"); } }
//Resources:ValidationResource:Entity_Header_MissingId_System

		public static string Entity_Header_MissingId_System { get { return GetResourceString("Entity_Header_MissingId_System"); } }
//Resources:ValidationResource:Entity_Header_MissingText_System

		public static string Entity_Header_MissingText_System { get { return GetResourceString("Entity_Header_MissingText_System"); } }
//Resources:ValidationResource:Entity_Header_Null_System

		public static string Entity_Header_Null_System { get { return GetResourceString("Entity_Header_Null_System"); } }
//Resources:ValidationResource:IDIsRequired

		public static string IDIsRequired { get { return GetResourceString("IDIsRequired"); } }
//Resources:ValidationResource:IDMustNotBeEmptyGuid

		public static string IDMustNotBeEmptyGuid { get { return GetResourceString("IDMustNotBeEmptyGuid"); } }
//Resources:ValidationResource:IDMustNotBeZero

		public static string IDMustNotBeZero { get { return GetResourceString("IDMustNotBeZero"); } }
//Resources:ValidationResource:LastUpdateDateInvalidFormat

		public static string LastUpdateDateInvalidFormat { get { return GetResourceString("LastUpdateDateInvalidFormat"); } }
//Resources:ValidationResource:LastUpdatedByIdInvalidFormat

		public static string LastUpdatedByIdInvalidFormat { get { return GetResourceString("LastUpdatedByIdInvalidFormat"); } }
//Resources:ValidationResource:LastUpdatedByIdNotNullOrEmpty

		public static string LastUpdatedByIdNotNullOrEmpty { get { return GetResourceString("LastUpdatedByIdNotNullOrEmpty"); } }
//Resources:ValidationResource:LastUpdatedByNotNull

		public static string LastUpdatedByNotNull { get { return GetResourceString("LastUpdatedByNotNull"); } }
//Resources:ValidationResource:LastUpdatedByTextNotNullOrEmpty

		public static string LastUpdatedByTextNotNullOrEmpty { get { return GetResourceString("LastUpdatedByTextNotNullOrEmpty"); } }
//Resources:ValidationResource:LastUpdatedDateRequired

		public static string LastUpdatedDateRequired { get { return GetResourceString("LastUpdatedDateRequired"); } }
//Resources:ValidationResource:PropertyIsRequired

		public static string PropertyIsRequired { get { return GetResourceString("PropertyIsRequired"); } }
//Resources:ValidationResource:SystemMissingProperty

		public static string SystemMissingProperty { get { return GetResourceString("SystemMissingProperty"); } }
//Resources:ValidationResource:Validation_RegEx_Namespace

		public static string Validation_RegEx_Namespace { get { return GetResourceString("Validation_RegEx_Namespace"); } }
//Resources:ValidationResource:ValueLength_Between

		public static string ValueLength_Between { get { return GetResourceString("ValueLength_Between"); } }
//Resources:ValidationResource:ValueLength_TooLong

		public static string ValueLength_TooLong { get { return GetResourceString("ValueLength_TooLong"); } }
//Resources:ValidationResource:ValueLength_TooShort

		public static string ValueLength_TooShort { get { return GetResourceString("ValueLength_TooShort"); } }

		public static class Names
		{
			public const string Common_Key_Help = "Common_Key_Help";
			public const string Common_Key_Validation = "Common_Key_Validation";
			public const string Concurrency_Error = "Concurrency_Error";
			public const string Concurrency_ErrorMessage = "Concurrency_ErrorMessage";
			public const string CreatedByIdInvalidFormat = "CreatedByIdInvalidFormat";
			public const string CreatedByIdNotNullOrEmpty = "CreatedByIdNotNullOrEmpty";
			public const string CreatedByNotNull = "CreatedByNotNull";
			public const string CreatedByTextNotNullOrEmpty = "CreatedByTextNotNullOrEmpty";
			public const string CreationDateInvalidFormat = "CreationDateInvalidFormat";
			public const string CreationDateRequired = "CreationDateRequired";
			public const string Entity_Header_MissingId_System = "Entity_Header_MissingId_System";
			public const string Entity_Header_MissingText_System = "Entity_Header_MissingText_System";
			public const string Entity_Header_Null_System = "Entity_Header_Null_System";
			public const string IDIsRequired = "IDIsRequired";
			public const string IDMustNotBeEmptyGuid = "IDMustNotBeEmptyGuid";
			public const string IDMustNotBeZero = "IDMustNotBeZero";
			public const string LastUpdateDateInvalidFormat = "LastUpdateDateInvalidFormat";
			public const string LastUpdatedByIdInvalidFormat = "LastUpdatedByIdInvalidFormat";
			public const string LastUpdatedByIdNotNullOrEmpty = "LastUpdatedByIdNotNullOrEmpty";
			public const string LastUpdatedByNotNull = "LastUpdatedByNotNull";
			public const string LastUpdatedByTextNotNullOrEmpty = "LastUpdatedByTextNotNullOrEmpty";
			public const string LastUpdatedDateRequired = "LastUpdatedDateRequired";
			public const string PropertyIsRequired = "PropertyIsRequired";
			public const string SystemMissingProperty = "SystemMissingProperty";
			public const string Validation_RegEx_Namespace = "Validation_RegEx_Namespace";
			public const string ValueLength_Between = "ValueLength_Between";
			public const string ValueLength_TooLong = "ValueLength_TooLong";
			public const string ValueLength_TooShort = "ValueLength_TooShort";
		}
	}
}

