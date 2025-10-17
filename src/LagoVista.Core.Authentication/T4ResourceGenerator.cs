// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ead21664ae9522d03343fd38a8ca72611ee2a7411ee929e0cf79641b174d8931
// IndexVersion: 1
// --- END CODE INDEX META ---
/*5/23/2017 17:07:20*/
using System.Globalization;
using System.Reflection;

//Resources:AuthenticationResources:Common_CreatedBy
namespace LagoVista.Core.Authentication.Resources
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.Core.Authentication.Resources.AuthenticationResources", typeof(AuthenticationResources).GetTypeInfo().Assembly);
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
//Resources:AuthenticationResources:UserInfo_Description

		public static string UserInfo_Description { get { return GetResourceString("UserInfo_Description"); } }
//Resources:AuthenticationResources:UserInfo_Email

		public static string UserInfo_Email { get { return GetResourceString("UserInfo_Email"); } }
//Resources:AuthenticationResources:UserInfo_FirstName

		public static string UserInfo_FirstName { get { return GetResourceString("UserInfo_FirstName"); } }
//Resources:AuthenticationResources:UserInfo_Help

		public static string UserInfo_Help { get { return GetResourceString("UserInfo_Help"); } }
//Resources:AuthenticationResources:UserInfo_IsEmailConfirmed

		public static string UserInfo_IsEmailConfirmed { get { return GetResourceString("UserInfo_IsEmailConfirmed"); } }
//Resources:AuthenticationResources:UserInfo_IsPhoneConfirmed

		public static string UserInfo_IsPhoneConfirmed { get { return GetResourceString("UserInfo_IsPhoneConfirmed"); } }
//Resources:AuthenticationResources:UserInfo_IsSystemAdmin

		public static string UserInfo_IsSystemAdmin { get { return GetResourceString("UserInfo_IsSystemAdmin"); } }
//Resources:AuthenticationResources:UserInfo_LastName

		public static string UserInfo_LastName { get { return GetResourceString("UserInfo_LastName"); } }
//Resources:AuthenticationResources:UserInfo_PhoneNumber

		public static string UserInfo_PhoneNumber { get { return GetResourceString("UserInfo_PhoneNumber"); } }
//Resources:AuthenticationResources:UserInfo_Title

		public static string UserInfo_Title { get { return GetResourceString("UserInfo_Title"); } }

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
			public const string UserInfo_Description = "UserInfo_Description";
			public const string UserInfo_Email = "UserInfo_Email";
			public const string UserInfo_FirstName = "UserInfo_FirstName";
			public const string UserInfo_Help = "UserInfo_Help";
			public const string UserInfo_IsEmailConfirmed = "UserInfo_IsEmailConfirmed";
			public const string UserInfo_IsPhoneConfirmed = "UserInfo_IsPhoneConfirmed";
			public const string UserInfo_IsSystemAdmin = "UserInfo_IsSystemAdmin";
			public const string UserInfo_LastName = "UserInfo_LastName";
			public const string UserInfo_PhoneNumber = "UserInfo_PhoneNumber";
			public const string UserInfo_Title = "UserInfo_Title";
		}
	}
}

