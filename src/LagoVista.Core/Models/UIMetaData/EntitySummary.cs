using LagoVista.Core.Attributes;
using System;
using System.Reflection;

public class EntitySummary
{
    public String Name { get; set; }
    public String DomainKey { get; set; }
    public String ClassName { get; set; }

    public bool Cloneable { get; set; }
    public String ShortClassName { get; set; }

    public string UiCreateUrl { get; set; }
    public string UiGetListUrl { get; set; }
    public string UiEditUrl { get; set; }
    public string UiPreviewUrl { get; set; }

    public String Icon { get; set; }
    public String Description { get; set; }
    public String UserHelp { get; set; }

    // Existing (keep)
    public EntityDescriptionAttribute.EntityTypes EntityType { get; set; }

    // New knobs
    public string ClusterKey { get; set; }
    public string EntityKey { get; set; }

    public EntityDescriptionAttribute.ModelTypes ModelType { get; set; }
    public EntityDescriptionAttribute.EntityShapes Shape { get; set; }
    public EntityDescriptionAttribute.Lifecycles Lifecycle { get; set; }
    public EntityDescriptionAttribute.Sensitivities Sensitivity { get; set; }

    public IndexingInfo Index { get; set; }

    public class IndexingInfo
    {
        public bool Include { get; set; }
        public EntityDescriptionAttribute.IndexTiers Tier { get; set; }
        public int Priority { get; set; }
        public string TagsCsv { get; set; }
    }

    public static EntitySummary CreateFromAttribute(Type entityType, EntityDescriptionAttribute attr)
    {
        if (entityType == null) throw new ArgumentNullException(nameof(entityType));
        if (attr == null) throw new ArgumentNullException(nameof(attr));

        var title = ResolveResourceString(attr.ResourceType, attr.TitleResource) ?? entityType.Name;
        var description = ResolveResourceString(attr.ResourceType, attr.DescriptionResource);
        var userHelp = ResolveResourceString(attr.ResourceType, attr.UserHelpResource);

        return new EntitySummary()
        {
            // Identity
            ClassName = entityType.FullName,
            ShortClassName = entityType.Name,

            // Human meaning
            Name = title,
            Description = description,
            UserHelp = userHelp,

            // Domain + UI
            DomainKey = attr.Domain,
            Icon = attr.Icon,

            UiEditUrl = attr.EditUIUrl,
            UiCreateUrl = attr.CreateUIUrl,
            UiGetListUrl = attr.ListUIUrl,
            UiPreviewUrl = attr.PreviewUIUrl,

            // Existing capabilities
            Cloneable = attr.Cloneable,
            EntityType = attr.EntityType,

            // New knobs
            ClusterKey = attr.ClusterKey,
            EntityKey = attr.EntityKey,
            ModelType = attr.ModelType,
            Shape = attr.Shape,
            Lifecycle = attr.Lifecycle,
            Sensitivity = attr.Sensitivity,

            Index = new IndexingInfo()
            {
                Include = attr.IndexInclude,
                Tier = attr.IndexTier,
                Priority = attr.IndexPriority,
                TagsCsv = attr.IndexTagsCsv
            }
        };
    }

    private static string ResolveResourceString(Type resourceType, string resourceKey)
    {
        if (resourceType == null) return null;
        if (String.IsNullOrEmpty(resourceKey)) return null;

        // Matches your snippet, but packaged as a helper.
        var prop = resourceType.GetTypeInfo().GetDeclaredProperty(resourceKey);
        if (prop == null) return null;

        return prop.GetValue(prop.DeclaringType, null) as string;
    }
}
