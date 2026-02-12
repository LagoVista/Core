// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: TBD
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Validation;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LagoVista.Core.Models.AIMetaData
{
    /// <summary>
    /// Model-facing projection of DetailResponse<TModel>.
    /// Purpose: Provide only the metadata a model needs to conduct a guided conversation
    /// and produce structured updates, without UI wiring noise (URLs, editor paths, etc.).
    /// </summary>
    public class AiDetailResponse<TModel> : InvokeResult where TModel : new()
    {
        public string ModelTitle { get; set; }
        public string ModelHelp { get; set; }
        public string ModelName { get; set; }
        public string FullClassName { get; set; }
        public string AssemblyName { get; set; }

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
        public string Name { get; set; }             // camelCase
        public string Label { get; set; }
        public string Help { get; set; }
        public string FieldType { get; set; }        // e.g., "Text", "Picker", "ChildListInline"
        public bool IsRequired { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string GetEntityHeaderOptionsUrl { get; set; }

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
                MaxLength = field.MaxLength,
                AiChatPrompt = field.AiChatPrompt,
                GetEntityHeaderOptionsUrl = field.EntityHeaderPickerUrl
            };


            if (field.Options != null && field.Options.Count > 0)
            {
                desc.AiChatPrompt += "When assinging property on entity, MUST use format {\"id\":<id>,\"key\":<key>,\"text\":<text>}.";
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
}
