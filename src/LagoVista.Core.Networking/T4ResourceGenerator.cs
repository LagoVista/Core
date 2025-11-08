// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 817827d84b5f486ff9b5b2ba5a05b8059faef0c647b8a7d8792351fbb0d54c89
// IndexVersion: 2
// --- END CODE INDEX META ---
/*9/25/2025 10:09:46 AM*/
using System.Globalization;
using System.Reflection;

//Resources:NetworkingResources:MQTT_QOS0
namespace LagoVista.Core.Networking.Resources
{
	public class NetworkingResources
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.Core.Networking.Resources.NetworkingResources", typeof(NetworkingResources).GetTypeInfo().Assembly);
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
		
		public static string MQTT_QOS0 { get { return GetResourceString("MQTT_QOS0"); } }
//Resources:NetworkingResources:MQTT_QOS1

		public static string MQTT_QOS1 { get { return GetResourceString("MQTT_QOS1"); } }
//Resources:NetworkingResources:MQTT_QOS2

		public static string MQTT_QOS2 { get { return GetResourceString("MQTT_QOS2"); } }
//Resources:NetworkingResources:Subscription_Description

		public static string Subscription_Description { get { return GetResourceString("Subscription_Description"); } }
//Resources:NetworkingResources:Subscription_QOS

		public static string Subscription_QOS { get { return GetResourceString("Subscription_QOS"); } }
//Resources:NetworkingResources:Subscription_QOS_Help

		public static string Subscription_QOS_Help { get { return GetResourceString("Subscription_QOS_Help"); } }
//Resources:NetworkingResources:Subscription_QOS_Select

		public static string Subscription_QOS_Select { get { return GetResourceString("Subscription_QOS_Select"); } }
//Resources:NetworkingResources:Subscription_Title

		public static string Subscription_Title { get { return GetResourceString("Subscription_Title"); } }
//Resources:NetworkingResources:Subscription_Topic

		public static string Subscription_Topic { get { return GetResourceString("Subscription_Topic"); } }
//Resources:NetworkingResources:Subscription_Topic_Help

		public static string Subscription_Topic_Help { get { return GetResourceString("Subscription_Topic_Help"); } }

		public static class Names
		{
			public const string MQTT_QOS0 = "MQTT_QOS0";
			public const string MQTT_QOS1 = "MQTT_QOS1";
			public const string MQTT_QOS2 = "MQTT_QOS2";
			public const string Subscription_Description = "Subscription_Description";
			public const string Subscription_QOS = "Subscription_QOS";
			public const string Subscription_QOS_Help = "Subscription_QOS_Help";
			public const string Subscription_QOS_Select = "Subscription_QOS_Select";
			public const string Subscription_Title = "Subscription_Title";
			public const string Subscription_Topic = "Subscription_Topic";
			public const string Subscription_Topic_Help = "Subscription_Topic_Help";
		}
	}
}

