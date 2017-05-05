using System.Globalization;
using System.Reflection;

//Models:ValidationResources:CustomValidationMessage
namespace LagoVista.Core.Tests.Resources.Models
{
	public class ValidationResources
	{
        private static global::System.Resources.ResourceManager resourceMan;
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static global::System.Resources.ResourceManager ResourceManager 
		{
            get 
			{
                if (object.ReferenceEquals(resourceMan, null)) 
				{
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.Core.Tests.Models.ValidationResources", typeof(ValidationResources).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
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
		
		public static string CustomValidationMessage { get { return GetResourceString("CustomValidationMessage"); } }
//Models:ValidationResources:EnumFive

		public static string EnumFive { get { return GetResourceString("EnumFive"); } }
//Models:ValidationResources:EnumFour

		public static string EnumFour { get { return GetResourceString("EnumFour"); } }
//Models:ValidationResources:EnumOne

		public static string EnumOne { get { return GetResourceString("EnumOne"); } }
//Models:ValidationResources:EnumThree

		public static string EnumThree { get { return GetResourceString("EnumThree"); } }
//Models:ValidationResources:EnumTwo

		public static string EnumTwo { get { return GetResourceString("EnumTwo"); } }
//Models:ValidationResources:FirstNameLabel

		public static string FirstNameLabel { get { return GetResourceString("FirstNameLabel"); } }
//UIMetaData:MetaDataResources:BaseField1

		public static class Names
		{
			public const string CustomValidationMessage = "CustomValidationMessage";
			public const string EnumFive = "EnumFive";
			public const string EnumFour = "EnumFour";
			public const string EnumOne = "EnumOne";
			public const string EnumThree = "EnumThree";
			public const string EnumTwo = "EnumTwo";
			public const string FirstNameLabel = "FirstNameLabel";
		}
	}
}
namespace LagoVista.Core.Tests.Resources.UIMetaData
{
	public class MetaDataResources
	{
        private static global::System.Resources.ResourceManager resourceMan;
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        private static global::System.Resources.ResourceManager ResourceManager 
		{
            get 
			{
                if (object.ReferenceEquals(resourceMan, null)) 
				{
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("LagoVista.Core.Tests.UIMetaData.MetaDataResources", typeof(MetaDataResources).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
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
		
		public static string BaseField1 { get { return GetResourceString("BaseField1"); } }
//UIMetaData:MetaDataResources:BaseField1_Help

		public static string BaseField1_Help { get { return GetResourceString("BaseField1_Help"); } }
//UIMetaData:MetaDataResources:BaseField1_Requied

		public static string BaseField1_Requied { get { return GetResourceString("BaseField1_Requied"); } }
//UIMetaData:MetaDataResources:BaseField1_WM

		public static string BaseField1_WM { get { return GetResourceString("BaseField1_WM"); } }
//UIMetaData:MetaDataResources:Enum1_Help

		public static string Enum1_Help { get { return GetResourceString("Enum1_Help"); } }
//UIMetaData:MetaDataResources:Enum1_Label

		public static string Enum1_Label { get { return GetResourceString("Enum1_Label"); } }
//UIMetaData:MetaDataResources:Enum2_Help

		public static string Enum2_Help { get { return GetResourceString("Enum2_Help"); } }
//UIMetaData:MetaDataResources:Enum2_Label

		public static string Enum2_Label { get { return GetResourceString("Enum2_Label"); } }
//UIMetaData:MetaDataResources:Enum3_Help

		public static string Enum3_Help { get { return GetResourceString("Enum3_Help"); } }
//UIMetaData:MetaDataResources:Enum3_Label

		public static string Enum3_Label { get { return GetResourceString("Enum3_Label"); } }
//UIMetaData:MetaDataResources:Field1_Help

		public static string Field1_Help { get { return GetResourceString("Field1_Help"); } }
//UIMetaData:MetaDataResources:Field1_Label

		public static string Field1_Label { get { return GetResourceString("Field1_Label"); } }
//UIMetaData:MetaDataResources:Field1_RequiredMessage1

		public static string Field1_RequiredMessage1 { get { return GetResourceString("Field1_RequiredMessage1"); } }
//UIMetaData:MetaDataResources:Field1_Summary

		public static string Field1_Summary { get { return GetResourceString("Field1_Summary"); } }
//UIMetaData:MetaDataResources:Field1_WaterMark

		public static string Field1_WaterMark { get { return GetResourceString("Field1_WaterMark"); } }
//UIMetaData:MetaDataResources:Field2_Help

		public static string Field2_Help { get { return GetResourceString("Field2_Help"); } }
//UIMetaData:MetaDataResources:Field2_Label

		public static string Field2_Label { get { return GetResourceString("Field2_Label"); } }
//UIMetaData:MetaDataResources:Field2_RegExMessage

		public static string Field2_RegExMessage { get { return GetResourceString("Field2_RegExMessage"); } }
//UIMetaData:MetaDataResources:Field2_Summary

		public static string Field2_Summary { get { return GetResourceString("Field2_Summary"); } }
//UIMetaData:MetaDataResources:Field3_Column_Header

		public static string Field3_Column_Header { get { return GetResourceString("Field3_Column_Header"); } }
//UIMetaData:MetaDataResources:Field3_Label

		public static string Field3_Label { get { return GetResourceString("Field3_Label"); } }
//UIMetaData:MetaDataResources:Field3_Summary

		public static string Field3_Summary { get { return GetResourceString("Field3_Summary"); } }
//UIMetaData:MetaDataResources:Field4_CompareResource

		public static string Field4_CompareResource { get { return GetResourceString("Field4_CompareResource"); } }
//UIMetaData:MetaDataResources:Field4_Label

		public static string Field4_Label { get { return GetResourceString("Field4_Label"); } }
//UIMetaData:MetaDataResources:Field5_Label

		public static string Field5_Label { get { return GetResourceString("Field5_Label"); } }
//UIMetaData:MetaDataResources:Model1_Description

		public static string Model1_Description { get { return GetResourceString("Model1_Description"); } }
//UIMetaData:MetaDataResources:Model1_Help

		public static string Model1_Help { get { return GetResourceString("Model1_Help"); } }
//UIMetaData:MetaDataResources:Model1_Title

		public static string Model1_Title { get { return GetResourceString("Model1_Title"); } }
//UIMetaData:MetaDataResources:Model2_Description

		public static string Model2_Description { get { return GetResourceString("Model2_Description"); } }
//UIMetaData:MetaDataResources:Model2_Help

		public static string Model2_Help { get { return GetResourceString("Model2_Help"); } }
//UIMetaData:MetaDataResources:Model2_Title

		public static string Model2_Title { get { return GetResourceString("Model2_Title"); } }
//UIMetaData:MetaDataResources:Model3_Description

		public static string Model3_Description { get { return GetResourceString("Model3_Description"); } }
//UIMetaData:MetaDataResources:Model3_Help

		public static string Model3_Help { get { return GetResourceString("Model3_Help"); } }
//UIMetaData:MetaDataResources:Model3_Title

		public static string Model3_Title { get { return GetResourceString("Model3_Title"); } }

		public static class Names
		{
			public const string BaseField1 = "BaseField1";
			public const string BaseField1_Help = "BaseField1_Help";
			public const string BaseField1_Requied = "BaseField1_Requied";
			public const string BaseField1_WM = "BaseField1_WM";
			public const string Enum1_Help = "Enum1_Help";
			public const string Enum1_Label = "Enum1_Label";
			public const string Enum2_Help = "Enum2_Help";
			public const string Enum2_Label = "Enum2_Label";
			public const string Enum3_Help = "Enum3_Help";
			public const string Enum3_Label = "Enum3_Label";
			public const string Field1_Help = "Field1_Help";
			public const string Field1_Label = "Field1_Label";
			public const string Field1_RequiredMessage1 = "Field1_RequiredMessage1";
			public const string Field1_Summary = "Field1_Summary";
			public const string Field1_WaterMark = "Field1_WaterMark";
			public const string Field2_Help = "Field2_Help";
			public const string Field2_Label = "Field2_Label";
			public const string Field2_RegExMessage = "Field2_RegExMessage";
			public const string Field2_Summary = "Field2_Summary";
			public const string Field3_Column_Header = "Field3_Column_Header";
			public const string Field3_Label = "Field3_Label";
			public const string Field3_Summary = "Field3_Summary";
			public const string Field4_CompareResource = "Field4_CompareResource";
			public const string Field4_Label = "Field4_Label";
			public const string Field5_Label = "Field5_Label";
			public const string Model1_Description = "Model1_Description";
			public const string Model1_Help = "Model1_Help";
			public const string Model1_Title = "Model1_Title";
			public const string Model2_Description = "Model2_Description";
			public const string Model2_Help = "Model2_Help";
			public const string Model2_Title = "Model2_Title";
			public const string Model3_Description = "Model3_Description";
			public const string Model3_Help = "Model3_Help";
			public const string Model3_Title = "Model3_Title";
		}
	}
}
