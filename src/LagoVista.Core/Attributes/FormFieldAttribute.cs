using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.Core.Attributes
{
    public enum FieldTypes
    {
        RowId,
        Picker,
        Hidden,
        Text,
        Key,
        MultiLineText,
        Integer,
        Decimal,
        Date,
        Time,
        LinkButton,
        DateTime,
        Phone,
        Password,
        Email,
        Bool,
        ChildList,
        ChildItem,
        ChildView,
        NodeScript,
        JsonDateTime,
        NameSpace,
        CheckBox,
        OptionsList,
        GeoLocation,
        EntityHeaderPicker,
        Byte,
        Secret,
        Icon,
        Color,
        ChildListInline,
        FileUpload,
        ChildListInlinePicker,
        UserPicker,   
        WebLink,
        Action,
        ReadonlyLabel,
        Duration,
        MediaResources,
        ProductPicker,
        PaymentMethod,
        HtmlEditor,
        Discussion,
        Category,
        Year,
        Month,
        Money,
        SiteContentPicker,
        ChildListSiteContentPicker,
        Surveys,
        DevicePicker,
        OrgLocationPicker,
        Schedule,
        ProductPickerList,
        Custom,
        Percent,
        SecureCertificate,
        CustomerPicker,
        Point2D,
        Point3D,
        Point2DSize,
        Point3DSize,
        Point2DArray,
        Signature,
        MultiLineTextAreaFixedFont,
        RawHtml
    }

    public enum NamespaceTypes
    {
        None,
        Organization,
        Location,
        DeviceGroup
    }

    [AttributeUsage( AttributeTargets.Property)]
    public class FormFieldAttribute : Attribute
    {
        Type _resourceType;
        Type _enumType;

        private string _labelDisplayResource;
        private string _columnHeaderDisplayResource;
        private string _requiredMessageResources;
        private string _regExValidation;
        private string _regExValidationMessageResource;
        private bool _isRequired;
        private bool _isUserEditable;
        private bool _isMarkDown;
        private string _helpResource;
        private string _waterMark;
        private int? _minLength;
        private int? _maxLength;
        private bool _allowAddChild;
        private string _compareTo;
        private string _pickerFor;
        private string _compareToMsgResource;
        private string _namespaceUniqueMessageResource;
        private string _childListDisplayMember;
        private string _secureIdFieldName;
        private FieldTypes _fieldType;
        private string _pickerType;
        private NamespaceTypes _namespaceType;
        private string _uploadUrl;
        private string _factoryUrl;
        private string _getUrl;
        private string _entityHeaderPickerUrl;
        private string _helpUrl;
        private string _scriptTemplateName;
        private bool _inPlaceEditing;
        private bool _sortEnums;
        private bool _addEnumSelect;
        private string _editorPath;
        private bool _isReferenceField;
        private bool _openByDefault;
        private bool _isFileUploadImage;
        private string _tags;
        private string _customFieldType;
        private string _generatedImageSize;
        private string _displayImageSize;       

        public FormFieldAttribute(String LabelResource = "",
                                  String ColHeaderResource = "",
                                  String ValidationRegEx = "",
                                  String ReqMessageResource = "",
                                  String HelpResource = "",
                                  String WaterMark = "",
                                  String CompareTo = "",
                                  String CompareToMsgResource = "",
                                  String RegExValidationMessageResource = "",
                                  String PickerType = "",
                                  String PickerFor = "",
                                  String RegExMessage = "",
                                  String NamespaceUniqueMessageResource = "",
                                  String ChildListDisplayMember = "",
                                  String SecureIdFieldName = "",
                                  NamespaceTypes NamespaceType = NamespaceTypes.None,
                                  int MinLength = -1,
                                  int MaxLength = -1,
                                  bool IsRequired = false,
                                  bool IsUserEditable = true,
                                  bool IsMarkDown = false,
                                  bool AllowAddChild = true,
                                  FieldTypes FieldType = FieldTypes.Text,
                                  Type ResourceType = null,
                                  Type EnumType = null,
                                  string UploadUrl = "/api/media/resource/upload",
                                  string FactoryUrl = null,
                                  string GetUrl = null,
                                  string EntityHeaderPickerUrl = null,
                                  string HelpUrl = null,
                                  string ScriptTemplateName = null,
                                  bool InPlaceEditing = true,
                                  bool SortEnums = false, 
                                  bool AddEnumSelect = false,
                                  string EditorPath = "",
                                  bool IsReferenceField = false, 
                                  bool OpenByDefault = false,
                                  bool IsFileUploadImage = true,
                                  string ReplaceableTags = null,
                                  string CustomFieldType = null,
                                  string AiChatPrompt = "",
                                  string GeneratedImageSize = "1024x1024",
                                  string DisplayImageSize = "",
                                  string SharedContentKey = "",
                                  bool PrivateFileUpload = false,
                                  int Rows = 6,
                                  bool ImageUpload = true,
                                  string ChildListDisplayMembers = "",
                                  string CustomCategoryType = null,
                                  bool CanAddRows = true,
                                  string TagsCSVURls = "")
        {
            _labelDisplayResource = LabelResource;
            _columnHeaderDisplayResource = ColHeaderResource;
            _regExValidation = ValidationRegEx;
            _requiredMessageResources = ReqMessageResource;
            _regExValidationMessageResource = RegExValidationMessageResource;
            _isRequired = IsRequired;
            _fieldType = FieldType;
            _isUserEditable = IsUserEditable;
            _helpResource = HelpResource;
            _resourceType = ResourceType;
            _pickerType = PickerType;
            _pickerFor = PickerFor;
            _isMarkDown = IsMarkDown;
            _waterMark = WaterMark;
            _allowAddChild = AllowAddChild;
            _minLength = MinLength < 0 ? (int?)null : MinLength;
            _maxLength = MaxLength < 0 ? (int?)null : MaxLength;
            _compareTo = CompareTo;
            _compareToMsgResource = CompareToMsgResource;
            _namespaceUniqueMessageResource = NamespaceUniqueMessageResource;
            _namespaceType = NamespaceType;
            _enumType = EnumType;
            _childListDisplayMember = ChildListDisplayMember;
            _secureIdFieldName = SecureIdFieldName;
            _uploadUrl = UploadUrl;
            _factoryUrl = FactoryUrl;
            _entityHeaderPickerUrl = EntityHeaderPickerUrl;
            _helpUrl = HelpUrl;
            _scriptTemplateName = ScriptTemplateName;
            _inPlaceEditing = InPlaceEditing;
            _sortEnums = SortEnums;
            _addEnumSelect = AddEnumSelect;
            _editorPath = EditorPath;
            _isReferenceField = IsReferenceField;
            _getUrl = GetUrl;
            _openByDefault = OpenByDefault;
            _isFileUploadImage = IsFileUploadImage;
            this.AiChatPrompt = AiChatPrompt;
            _tags = ReplaceableTags;
            _customFieldType = CustomFieldType;
            _generatedImageSize = GeneratedImageSize;
            _displayImageSize = DisplayImageSize;
            this.SharedContentKey = SharedContentKey;
            this.PrivateFileUpload = PrivateFileUpload;
            this.Rows = Rows;
            this.ImageUpload = ImageUpload;
            this.ChildListDisplayMembers = ChildListDisplayMembers;
            this.CustomCategoryType = CustomCategoryType;
            this.CanAddRows = CanAddRows;
            this.TagsCSVURls = TagsCSVURls;
        }

        public String PickerFor { get { return _pickerFor; } }
        public String PickerType { get { return _pickerType; } }
        public String HelpResource { get { return _helpResource; } }
        public String LabelDisplayResource { get { return _labelDisplayResource; } }
        public String RequiredMessageResource { get {return _requiredMessageResources; } }
        public String NamespaceUniqueMessageResource { get { return _namespaceUniqueMessageResource; } }
        public String RegExValidation { get { return _regExValidation; } }
        public String RegExValidationMessageResource { get { return _regExValidationMessageResource; } }
        public bool IsRequired { get { return _isRequired; } }
        public bool IsMarkDown { get { return _isMarkDown; } }
        public bool IsUserEditable { get { return _isUserEditable; } }
        public bool AllowAddChild { get { return _allowAddChild; } }

        public string CustomFieldType { get { return _customFieldType; } }
        public FieldTypes FieldType { get { return _fieldType; } }
        public Type ResourceType { get { return _resourceType; } }
        public string  WaterMark { get { return _waterMark; } }
        public string CompareTo { get { return _compareTo; } }
        public string CompareToMsgResource { get { return _compareToMsgResource; } }
        public int? MinLength { get { return _minLength; } }

        public bool AddEnumSelect { get { return _addEnumSelect; } }
        public bool SortEnums { get { return _sortEnums; } }
        public int? MaxLength { get { return _maxLength; } }
        public NamespaceTypes NamespaceType { get { return _namespaceType; } }
        public Type EnumType { get { return _enumType; } }
        public string ChildListDisplayMember { get { return _childListDisplayMember; } }
        public string SecureIdFieldName { get { return _secureIdFieldName; } }
        public string UploadUrl { get { return _uploadUrl; } }
        public string FactoryUrl { get { return _factoryUrl; } }
        public string GetUrl { get { return _getUrl; } }

        public bool IsReferenceField { get { return _isReferenceField; } }

		public string EntityHeaderUrl { get { return _entityHeaderPickerUrl; } }
        public string Helpurl { get { return _helpUrl; } }

        public string ScriptTemplateName { get { return _scriptTemplateName; } }

        public bool InPlaceEditing { get { return _inPlaceEditing; } }

        public string AiChatPrompt { get; }

        public string GeneratedImageSize { get { return _generatedImageSize; } }
        public string DisplayImageSize { get { return _displayImageSize; } }

        public string EditorPath { get { return _editorPath; } }
        public bool OpenByDefault { get { return _openByDefault; } }

        public bool IsFileUploadImage { get { return _isFileUploadImage; } }
        public string Tags { get { return _tags; } }

        public string SharedContentKey { get; private set; }
        public bool PrivateFileUpload { get; set; }
        public int Rows { get; set; }

        public bool CanAddRows { get; }

        public string ChildListDisplayMembers { get; private set; }
        public string CustomCategoryType { get; }
        public bool ImageUpload { get; set; }
        public string TagsCSVURls { get; }
    }

    public class SelectListItem
    {
        public String Key { get; set; }
        public String Value { get; set; }
    }

    public class ReplaceableTag
    {
        public string Tag { get; set; }
        public string Title { get; set; }
    }
}
