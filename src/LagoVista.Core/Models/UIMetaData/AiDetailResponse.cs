// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: TBD
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LagoVista.Core.Models.AIMetaData
{
    public enum AiFieldValueType
    {
        Unknown,
        Text,
        Boolean,
        Integer,
        Decimal,
        Date,
        DateTime,
        StringList,
        EntityReference,
        EntityReferenceList,
        Object,
        ObjectList,
        File,
        Media
    }


    public enum AiFieldInteractionType
    {
        Unknown,
        TextInput,
        MultiLineText,
        RichText,
        Toggle,
        SingleSelect,
        MultiSelect,
        EntityPicker,
        EntityMultiPicker,
        StructuredObject,
        StructuredObjectList,
        FileUpload,
        ReadOnly
    }

    /// <summary>
    /// Model-facing projection of DetailResponse<TModel>.
    /// Purpose: Provide only the metadata a model needs to conduct a guided conversation
    /// and produce structured updates, without UI wiring noise (URLs, editor paths, etc.).
    /// </summary>
    public class AiDetailResponse<TModel> : InvokeResult, IAiDetailResponse where TModel : new()
    {
        public string ModelTitle { get; set; }
        public string ModelHelp { get; set; }
        public string ModelName { get; set; }
        public string FullClassName { get; set; }
        public string AssemblyName { get; set; }

        List<string> IAiDetailResponse.FieldOrder => FieldOrder;

        IDictionary<string, AiFieldDescriptor> IAiDetailResponse.Fields => Fields;

        object IAiDetailResponse.ModelInstance => Model;

        /// <summary>
        /// Conversation-level instructions (e.g., SOP/playbook text).
        /// </summary>
        public string AiPromptInstructions { get; set; }

        /// <summary>
        /// Optional: default AI mode (e.g., "interview", "enrich", etc.).
        /// </summary>
        public string AiDefaultMode { get; set; }

        /// <summary>
        /// Suggested ordering for the interview. Derived from form field lists.
        /// Contains camelCase field names.
        /// </summary>
        public List<string> FieldOrder { get; set; } = new List<string>();

        /// <summary>
        /// Field descriptors keyed by camelCase field name.
        /// </summary>
        public IDictionary<string, AiFieldDescriptor> Fields { get; set; } = new Dictionary<string, AiFieldDescriptor>();

        public TModel Model { get; set; }

        public ValidationResult ValidationResult { get; set; } 

        /// <summary>
        /// Convenience factory: build AI projection directly from the UI-oriented DetailResponse.
        /// </summary>
        public static AiDetailResponse<TModel> CreateFromDetailResponse(
            DetailResponse<TModel> detail,
            bool includeCurrentValues = true,
            bool includeAdvancedOrdering = false)
        {
            if (detail == null) throw new ArgumentNullException(nameof(detail));

            var ai = new AiDetailResponse<TModel>
            {
                ModelTitle = detail.ModelTitle,
                ModelHelp = detail.ModelHelp,
                FullClassName = detail.FullClassName,
                AssemblyName = detail.AssemblyName,
                AiPromptInstructions = detail.AiPromptInstructions,
                AiDefaultMode = detail.AiDefaultMode,
                Model = detail.Model
            };

            ai.FieldOrder = BuildOrder(detail, includeAdvancedOrdering);

            if(detail.Model != null && detail.Model is IValidateable)
            {
                ai.ValidationResult = Validator.Validate(detail.Model as IValidateable, Actions.Update);
            }

            if (detail.View != null)
            {
                foreach (var kvp in detail.View)
                {
                    var fieldName = kvp.Key;
                    var field = kvp.Value;
                    ai.Fields[fieldName] = AiFieldDescriptor.FromFormField(field);
                }
            }

            return ai;
        }

        /// <summary>
        /// Convenience factory: build AI projection from a model instance using existing DetailResponse.Create pipeline.
        /// </summary>
        public static AiDetailResponse<TModel> Create(
            TModel model,
            bool isEditing = true,
            bool quickCreate = false,
            bool includeCurrentValues = true,
            bool includeAdvancedOrdering = false)
        {
            var detail = DetailResponse<TModel>.Create(model, isEditing: isEditing, quickCreate: quickCreate);
            return CreateFromDetailResponse(detail, includeCurrentValues: includeCurrentValues, includeAdvancedOrdering: includeAdvancedOrdering);
        }

        /// <summary>
        /// Convenience factory: build AI projection for a new model instance.
        /// </summary>
        public static AiDetailResponse<TModel> CreateNew(
            bool quickCreate = false,
            bool includeCurrentValues = false,
            bool includeAdvancedOrdering = false)
        {
            var detail = DetailResponse<TModel>.Create(quickCreate: quickCreate);
            return CreateFromDetailResponse(detail, includeCurrentValues: includeCurrentValues, includeAdvancedOrdering: includeAdvancedOrdering);
        }

        private static List<string> BuildOrder(DetailResponse<TModel> detail, bool includeAdvancedOrdering)
        {
            var order = new List<string>();

            void AddRangeDistinct(IEnumerable<string> fields)
            {
                if (fields == null) return;
                foreach (var f in fields)
                {
                    if (string.IsNullOrWhiteSpace(f)) continue;
                    if (!order.Contains(f)) order.Add(f);
                }
            }

            // Primary ordering follows the UI descriptors (since you already curated these lists).
            AddRangeDistinct(detail.FormFields);
            AddRangeDistinct(detail.FormFieldsCol2);

            AddRangeDistinct(detail.FormInlineFields);
            AddRangeDistinct(detail.FormFieldsBottom);
            AddRangeDistinct(detail.FormFieldsTabs);
            AddRangeDistinct(detail.FormMobileFields);
            AddRangeDistinct(detail.FormFieldsSimple);

            if (includeAdvancedOrdering)
            {
                AddRangeDistinct(detail.FormFieldsAdvanced);
                AddRangeDistinct(detail.FormFieldsAdvancedCol2);
            }

            // If we have View keys that weren’t included in any list, append them at the end.
            if (detail.View != null)
            {
                foreach (var key in detail.View.Keys)
                {
                    if (!order.Contains(key))
                        order.Add(key);
                }
            }

            return order;
        }
    }

    public sealed class AiFieldDescriptor
    {
        private static AiFieldValueType ResolveValueType(FormField field)
        {
            if (field == null)
            {
                return AiFieldValueType.Unknown;
            }

            switch (field.FieldType)
            {
                case nameof(FieldTypes.RowId):
                case nameof(FieldTypes.Hidden):
                case nameof(FieldTypes.Text):
                case nameof(FieldTypes.Key):
                case nameof(FieldTypes.MultiLineText):
                case nameof(FieldTypes.Phone):
                case nameof(FieldTypes.Password):
                case nameof(FieldTypes.Email):
                case nameof(FieldTypes.NodeScript):
                case nameof(FieldTypes.NameSpace):
                case nameof(FieldTypes.Secret):
                case nameof(FieldTypes.Icon):
                case nameof(FieldTypes.Color):
                case nameof(FieldTypes.WebLink):
                case nameof(FieldTypes.ReadonlyLabel):
                case nameof(FieldTypes.HtmlEditor):
                case nameof(FieldTypes.Discussion):
                case nameof(FieldTypes.Category):
                case nameof(FieldTypes.Custom):
                case nameof(FieldTypes.SecureCertificate):
                case nameof(FieldTypes.Signature):
                case nameof(FieldTypes.MultiLineTextAreaFixedFont):
                case nameof(FieldTypes.RawHtml):
                case nameof(FieldTypes.CronBuilder):
                    return AiFieldValueType.Text;

                case nameof(FieldTypes.Bool):
                case nameof(FieldTypes.CheckBox):
                    return AiFieldValueType.Boolean;

                case nameof(FieldTypes.Integer):
                case nameof(FieldTypes.Byte):
                case nameof(FieldTypes.Year):
                case nameof(FieldTypes.Month):
                case nameof(FieldTypes.Duration):
                    return AiFieldValueType.Integer;

                case nameof(FieldTypes.Decimal):
                case nameof(FieldTypes.Money):
                case nameof(FieldTypes.Percent):
                case nameof(FieldTypes.Currency):
                    return AiFieldValueType.Decimal;

                case nameof(FieldTypes.Date):
                    return AiFieldValueType.Date;

                case nameof(FieldTypes.Time):
                case nameof(FieldTypes.DateTime):
                case nameof(FieldTypes.JsonDateTime):
                case nameof(FieldTypes.Schedule):
                    return AiFieldValueType.DateTime;

                case nameof(FieldTypes.StringList):
                    return AiFieldValueType.StringList;

                case nameof(FieldTypes.Picker):
                case nameof(FieldTypes.OptionsList):
                    return ResolveOptionValueType(field);

                case nameof(FieldTypes.EntityHeaderPicker):
                case nameof(FieldTypes.EntithHeaderPickerDropDown):
                case nameof(FieldTypes.UserPicker):
                case nameof(FieldTypes.ProductPicker):
                case nameof(FieldTypes.PaymentMethod):
                case nameof(FieldTypes.SiteContentPicker):
                case nameof(FieldTypes.Surveys):
                case nameof(FieldTypes.DevicePicker):
                case nameof(FieldTypes.OrgLocationPicker):
                case nameof(FieldTypes.CustomerPicker):
                case nameof(FieldTypes.ContactPicker):
                    return AiFieldValueType.EntityReference;

                case nameof(FieldTypes.ProductPickerList):
                case nameof(FieldTypes.ChildListInlinePicker):
                case nameof(FieldTypes.ChildListSiteContentPicker):
                    return AiFieldValueType.EntityReferenceList;

                case nameof(FieldTypes.ChildItem):
                case nameof(FieldTypes.ChildView):
                case nameof(FieldTypes.GeoLocation):
                case nameof(FieldTypes.Point2D):
                case nameof(FieldTypes.Point3D):
                case nameof(FieldTypes.Point2DSize):
                case nameof(FieldTypes.Point3DSize):
                    return AiFieldValueType.Object;

                case nameof(FieldTypes.ChildList):
                case nameof(FieldTypes.ChildListInline):
                case nameof(FieldTypes.Point2DArray):
                    return AiFieldValueType.ObjectList;

                case nameof(FieldTypes.FileUpload):
                case nameof(FieldTypes.FileUploads):
                    return AiFieldValueType.File;

                case nameof(FieldTypes.MediaResources):
                    return AiFieldValueType.Media;

                case nameof(FieldTypes.LinkButton):
                case nameof(FieldTypes.Action):
                case nameof(FieldTypes.FontAwesomeIconPicker):
                default:
                    return AiFieldValueType.Unknown;
            }
        }

        private static AiFieldValueType ResolveOptionValueType(FormField field)
        {
            if (field?.Options == null || field.Options.Count == 0)
            {
                return AiFieldValueType.Text;
            }

            return AiFieldValueType.EntityReference;
        }

        private static AiFieldInteractionType ResolveInteractionType(FormField field)
        {
            if (field == null)
            {
                return AiFieldInteractionType.Unknown;
            }

            switch (field.FieldType)
            {
                case nameof(FieldTypes.RowId):
                case nameof(FieldTypes.Hidden):
                case nameof(FieldTypes.ReadonlyLabel):
                    return AiFieldInteractionType.ReadOnly;

                case nameof(FieldTypes.Text):
                case nameof(FieldTypes.Key):
                case nameof(FieldTypes.Phone):
                case nameof(FieldTypes.Password):
                case nameof(FieldTypes.Email):
                case nameof(FieldTypes.NameSpace):
                case nameof(FieldTypes.Secret):
                case nameof(FieldTypes.Icon):
                case nameof(FieldTypes.Color):
                case nameof(FieldTypes.WebLink):
                case nameof(FieldTypes.Integer):
                case nameof(FieldTypes.Decimal):
                case nameof(FieldTypes.Byte):
                case nameof(FieldTypes.Duration):
                case nameof(FieldTypes.Money):
                case nameof(FieldTypes.Percent):
                case nameof(FieldTypes.Currency):
                case nameof(FieldTypes.Date):
                case nameof(FieldTypes.Time):
                case nameof(FieldTypes.DateTime):
                case nameof(FieldTypes.JsonDateTime):
                case nameof(FieldTypes.Year):
                case nameof(FieldTypes.Month):
                case nameof(FieldTypes.CronBuilder):
                    return AiFieldInteractionType.TextInput;

                case nameof(FieldTypes.MultiLineText):
                case nameof(FieldTypes.NodeScript):
                case nameof(FieldTypes.MultiLineTextAreaFixedFont):
                    return AiFieldInteractionType.MultiLineText;

                case nameof(FieldTypes.HtmlEditor):
                case nameof(FieldTypes.Discussion):
                case nameof(FieldTypes.RawHtml):
                    return AiFieldInteractionType.RichText;

                case nameof(FieldTypes.Bool):
                case nameof(FieldTypes.CheckBox):
                    return AiFieldInteractionType.Toggle;

                case nameof(FieldTypes.Picker):
                case nameof(FieldTypes.OptionsList):
                case nameof(FieldTypes.Category):
                    return AiFieldInteractionType.SingleSelect;

                case nameof(FieldTypes.StringList):
                    return AiFieldInteractionType.MultiSelect;

                case nameof(FieldTypes.EntityHeaderPicker):
                case nameof(FieldTypes.EntithHeaderPickerDropDown):
                case nameof(FieldTypes.UserPicker):
                case nameof(FieldTypes.ProductPicker):
                case nameof(FieldTypes.PaymentMethod):
                case nameof(FieldTypes.SiteContentPicker):
                case nameof(FieldTypes.Surveys):
                case nameof(FieldTypes.DevicePicker):
                case nameof(FieldTypes.OrgLocationPicker):
                case nameof(FieldTypes.CustomerPicker):
                case nameof(FieldTypes.ContactPicker):
                    return AiFieldInteractionType.EntityPicker;

                case nameof(FieldTypes.ProductPickerList):
                case nameof(FieldTypes.ChildListInlinePicker):
                case nameof(FieldTypes.ChildListSiteContentPicker):
                    return AiFieldInteractionType.EntityMultiPicker;

                case nameof(FieldTypes.ChildItem):
                case nameof(FieldTypes.ChildView):
                case nameof(FieldTypes.GeoLocation):
                case nameof(FieldTypes.Point2D):
                case nameof(FieldTypes.Point3D):
                case nameof(FieldTypes.Point2DSize):
                case nameof(FieldTypes.Point3DSize):
                    return AiFieldInteractionType.StructuredObject;

                case nameof(FieldTypes.ChildList):
                case nameof(FieldTypes.ChildListInline):
                case nameof(FieldTypes.Point2DArray):
                    return AiFieldInteractionType.StructuredObjectList;

                case nameof(FieldTypes.FileUpload):
                case nameof(FieldTypes.FileUploads):
                case nameof(FieldTypes.MediaResources):
                case nameof(FieldTypes.Signature):
                case nameof(FieldTypes.SecureCertificate):
                    return AiFieldInteractionType.FileUpload;

                case nameof(FieldTypes.Custom):
                case nameof(FieldTypes.FontAwesomeIconPicker):
                case nameof(FieldTypes.Schedule):
                case nameof(FieldTypes.LinkButton):
                case nameof(FieldTypes.Action):
                default:
                    return AiFieldInteractionType.Unknown;
            }
        }
        public string Name { get; set; }             // camelCase
        public string Label { get; set; }
        public string Help { get; set; }
        public string FieldType { get; set; }        // e.g., "Text", "Picker", "ChildListInline"
        public bool IsRequired { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string GetEntityHeaderOptionsUrl { get; set; }

        public AiFieldValueType ValueType { get; set; }

        public AiFieldInteractionType InteractionType { get; set; }

        public string AiChatPrompt { get; set; }

        /// <summary>
        /// Enum/Picker options (if applicable).
        /// </summary>
        public List<AiOption> Options { get; set; } = new List<AiOption>();

        /// <summary>
        /// Child object schema (if applicable).
        /// </summary>
        public AiChildSchema Child { get; set; }

        public static AiFieldDescriptor FromFormField(FormField field)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            var desc = new AiFieldDescriptor
            {
                Name = field.Name,
                Label = field.Label,
                Help = field.Help,
                FieldType = field.FieldType,
                IsRequired = field.IsRequired,
                MinLength = field.MinLength,
                ValueType = ResolveValueType(field),
                InteractionType = ResolveInteractionType(field),
                MaxLength = field.MaxLength,
                AiChatPrompt = field.AiChatPrompt,
                GetEntityHeaderOptionsUrl = field.EntityHeaderPickerUrl
            };


            if (field.Options != null && field.Options.Count > 0)
            {
                desc.AiChatPrompt += "When assigning this property on the entity, MUST use format {\"id\":<id>,\"key\":<key>,\"text\":<text>}."; ;
                desc.Options = field.Options.Select(opt => new AiOption
                {
                    Id = opt.Id,
                    Key = opt.Key,
                    Label = opt.Label,
                    Text = opt.Text,
                    SortOrder = opt.SortOrder
                }).ToList();
            }

            // Child list/view schema
            // Your FormField.Create populates these for ChildListInline/Picker, and for ChildView it uses View too.
            var hasChildView = field.View != null && field.View.Count > 0;
            var hasChildFormFields = field.FormFields != null && field.FormFields.Count > 0;

            if (hasChildView || hasChildFormFields)
            {
                desc.Child = new AiChildSchema
                {
                    ModelTitle = field.ModelTitle,
                    ModelHelp = field.ModelHelp,
                    FieldOrder = BuildChildOrder(field),
                    Fields = BuildChildFields(field.View)
                };
            }

            return desc;
        }

        private static List<string> BuildChildOrder(FormField field)
        {
            var order = new List<string>();

            void AddRangeDistinct(IEnumerable<string> fields)
            {
                if (fields == null) return;
                foreach (var f in fields)
                {
                    if (string.IsNullOrWhiteSpace(f)) continue;
                    if (!order.Contains(f)) order.Add(f);
                }
            }

            AddRangeDistinct(field.FormFields);
            AddRangeDistinct(field.FormFieldsCol2);

            AddRangeDistinct(field.FormFieldAdvanced);
            AddRangeDistinct(field.FormFieldAdvancedCol2);

            if (field.View != null)
            {
                foreach (var key in field.View.Keys)
                {
                    if (!order.Contains(key))
                        order.Add(key);
                }
            }

            return order;
        }

        private static IDictionary<string, AiFieldDescriptor> BuildChildFields(IDictionary<string, FormField> view)
        {
            var dict = new Dictionary<string, AiFieldDescriptor>();
            if (view == null) return dict;

            foreach (var kvp in view)
            {
                dict[kvp.Key] = FromFormField(kvp.Value);
            }

            return dict;
        }
    }

    public interface IAiDetailResponseFactory
    {
        InvokeResult<IAiDetailResponse> Create(Type modelType, object model = null, bool isEditing = true, bool quickCreate = false, bool includeCurrentValues = true, bool includeAdvancedOrdering = false);
    }

    public interface IAiDetailResponse
    {
        string ModelTitle { get; }

        string ModelHelp { get; }

        string ModelName { get; }

        string FullClassName { get; }

        string AssemblyName { get; }

        string AiPromptInstructions { get; }

        string AiDefaultMode { get; }

        List<string> FieldOrder { get; }

        IDictionary<string, AiFieldDescriptor> Fields { get; }

        object ModelInstance { get; }

        ValidationResult ValidationResult { get; }
    }

    public sealed class AiChildSchema
    {
        public string ModelTitle { get; set; }
        public string ModelHelp { get; set; }

        public List<string> FieldOrder { get; set; } = new List<string>();
        public IDictionary<string, AiFieldDescriptor> Fields { get; set; } = new Dictionary<string, AiFieldDescriptor>();
    }

    public sealed class AiOption
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
        public string Text { get; set; }
        public int SortOrder { get; set; }
    }

    public static class AiDetailResponseFactory
    {
        private const string CreateMethodName = "Create";
        private const string CreateNewMethodName = "CreateNew";

        public static InvokeResult<IAiDetailResponse> Create(Type modelType, object model = null, bool isEditing = true, bool quickCreate = false, bool includeCurrentValues = true, bool includeAdvancedOrdering = false)
        {
            if (modelType == null) return InvokeResult<IAiDetailResponse>.FromError("Model type is required.");

            if (model != null && !modelType.IsInstanceOfType(model))
            {
                return InvokeResult<IAiDetailResponse>.FromError($"Model instance type '{model.GetType().FullName}' is not assignable to '{modelType.FullName}'.");
            }

            try
            {
                var responseType = typeof(AiDetailResponse<>).MakeGenericType(modelType);
                var method = model == null ? FindCreateNewMethod(responseType) : FindCreateMethod(responseType);

                if (method == null)
                {
                    return InvokeResult<IAiDetailResponse>.FromError($"Could not locate an AiDetailResponse factory method for '{modelType.FullName}'.");
                }

                var arguments = model == null ? new object[] { quickCreate, includeCurrentValues, includeAdvancedOrdering } : new object[] { model, isEditing, quickCreate, includeCurrentValues, includeAdvancedOrdering };
                var response = method.Invoke(null, arguments) as IAiDetailResponse;

                if (response == null)
                {
                    return InvokeResult<IAiDetailResponse>.FromError($"AiDetailResponse creation returned no result for '{modelType.FullName}'.");
                }

                return InvokeResult<IAiDetailResponse>.Create(response);
            }
            catch (TargetInvocationException ex)
            {
                var message = ex.InnerException?.Message ?? ex.Message;
                return InvokeResult<IAiDetailResponse>.FromError($"Could not create AI detail metadata for '{modelType.FullName}': {message}");
            }
            catch (Exception ex)
            {
                return InvokeResult<IAiDetailResponse>.FromError($"Could not create AI detail metadata for '{modelType.FullName}': {ex.Message}");
            }
        }

        private static MethodInfo FindCreateMethod(Type responseType)
        {
            return responseType.GetMethods(BindingFlags.Public | BindingFlags.Static).SingleOrDefault(method => method.Name == CreateMethodName && method.GetParameters().Length == 5);
        }

        private static MethodInfo FindCreateNewMethod(Type responseType)
        {
            return responseType.GetMethods(BindingFlags.Public | BindingFlags.Static).SingleOrDefault(method => method.Name == CreateNewMethodName && method.GetParameters().Length == 3);
        }
    }
}
