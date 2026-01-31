// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 070c59a59a4fbef5880f8129ea0a173a256121e0437dbbd96e5b10d2dbb2f37b
// IndexVersion: 2
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class EntityDescriptionAttribute : Attribute
    {
        Type _resourceType;
        EntityTypes _entityType;

        private String _descriptionResource;
        private String _domain;
        private String _titleResource;
        private String _userHelpResource;
        private string _insertUrl;
        private string _updateUrl;
        private string _saveUrl;
        private string _factoryUrl;
        private string _getUrl;
        private string _getListUrl;
        private string _deleteUrl;
        private string _helpUrl;
        private string _icon;
        private string _listUIUrl;
        private string _editUIUrl;
        private string _previewUIUrl;
        private string _createUIUrl;
        private int? _col1WidthPct;
        private int? _col2WidthPct;

        // -----------------------------
        // New: RAG + organization knobs
        // -----------------------------
        private string _clusterKey;
        private string _entityKey;
        private ModelTypes _modelType;
        private EntityShapes _shape;
        private Lifecycles _lifecycle;
        private Sensitivities _sensitivity;

        private bool _indexInclude;
        private IndexTiers _indexTier;
        private int _indexPriority;
        private string _indexTagsCsv;

        public enum EntityTypes
        {
            SimpleModel,
            Dto,
            BusinessObject,
            ViewModel,
            Summary,
            Discussion,
            CoreIoTModel,
            OrganizationModel,
            AdminModel,
            ChildObject,
            Manufacturing,
            CircuitBoards,
            Ai
        }

        /// <summary>
        /// New axis to replace/augment EntityType for organization + RAG.
        /// Keep EntityType intact to avoid breaking changes.
        /// </summary>
        public enum ModelTypes
        {
            Unspecified = 0,
            DomainEntity = 1,
            Configuration = 2,
            RuntimeArtifact = 3,
            Document = 4,
            Taxonomy = 5,
            Integration = 6
        }

        /// <summary>
        /// Shape of the model (what role it plays in API/UI + indexing).
        /// </summary>
        public enum EntityShapes
        {
            Unspecified = 0,
            Entity = 1,
            Summary = 2,
            Dto = 3,
            ViewModel = 4,
            ChildObject = 5,
            ValueObject = 6
        }

        /// <summary>
        /// When this model primarily exists.
        /// </summary>
        public enum Lifecycles
        {
            DesignTime = 0,
            RunTime = 1,
            Audit = 2
        }

        /// <summary>
        /// Sensitivity classification for retrieval/indexing policies.
        /// </summary>
        public enum Sensitivities
        {
            Public = 0,
            Internal = 1,
            Confidential = 2,
            Restricted = 3
        }

        /// <summary>
        /// Index guidance tier. Exclude means "do not embed/store in primary RAG index".
        /// </summary>
        public enum IndexTiers
        {
            Primary = 0,
            Secondary = 1,
            Aux = 2,
            Exclude = 3
        }

        public EntityDescriptionAttribute(
            String Domain,
            String TitleResource,
            String UserHelpResource,
            String DescriptionResource,
            EntityTypes entityType,
            Type ResourceType,
            string SaveUrl = null,
            string InsertUrl = null,
            string UpdateUrl = null,
            string FactoryUrl = null,
            string GetUrl = null,
            string GetListUrl = null,
            string DeleteUrl = null,
            string HelpUrl = null,
            string Icon = null,
            bool Cloneable = false,
            bool CanExport = false,
            bool CanImport = false,
            string ListUIUrl = null,
            string EditUIUrl = null,
            string CreateUIUrl = null,
            string PreviewUIUrl = null,
            int Col1WidthPercent = -1,
            int Col2WidthPercent = -1,
            bool SaveDraft = false,
            bool AutoSave = true,
            int AutoSaveIntervalSeconds = -1,

            // -----------------------------
            // New optional args (additive)
            // -----------------------------
            string ClusterKey = null,
            ModelTypes ModelType = ModelTypes.Unspecified,
            EntityShapes Shape = EntityShapes.Unspecified,
            Lifecycles Lifecycle = Lifecycles.DesignTime,
            Sensitivities Sensitivity = Sensitivities.Internal,
            string EntityKey = null,

            bool IndexInclude = true,
            IndexTiers IndexTier = IndexTiers.Secondary,
            int IndexPriority = 50,
            string IndexTagsCsv = null
        )
        {
            _descriptionResource = DescriptionResource;
            _titleResource = TitleResource;
            _userHelpResource = UserHelpResource;
            _domain = Domain;
            _resourceType = ResourceType;
            _entityType = entityType;

            _insertUrl = InsertUrl;
            _saveUrl = SaveUrl;
            _updateUrl = UpdateUrl;
            _factoryUrl = FactoryUrl;
            _getUrl = GetUrl;
            _getListUrl = GetListUrl;
            _deleteUrl = DeleteUrl;
            _helpUrl = HelpUrl;
            _icon = Icon;

            _listUIUrl = ListUIUrl;
            _editUIUrl = EditUIUrl;
            _previewUIUrl = PreviewUIUrl;
            _createUIUrl = CreateUIUrl;

            _col1WidthPct = Col1WidthPercent == -1 ? null : (int?)Col1WidthPercent;
            _col2WidthPct = Col2WidthPercent == -1 ? null : (int?)Col2WidthPercent;

            this.Cloneable = Cloneable;
            this.CanExport = CanExport;
            this.CanImport = CanImport;
            this.AutoSave = AutoSave;
            this.SaveDraft = SaveDraft;

            if (this.AutoSave)
            {
                AutoSaveIntervalSeconds = AutoSaveIntervalSeconds <= 0 ? 30 : AutoSaveIntervalSeconds;
            }

            // New knobs
            _clusterKey = ClusterKey;
            _modelType = ModelType;
            _shape = Shape;
            _lifecycle = Lifecycle;
            _sensitivity = Sensitivity;
            _entityKey = EntityKey;

            _indexInclude = IndexInclude;
            _indexTier = IndexTier;
            _indexPriority = IndexPriority;
            _indexTagsCsv = IndexTagsCsv;
        }

        public int? Col1WidthPercent { get { return _col1WidthPct; } }
        public int? Col2WidthPercent { get { return _col2WidthPct; } }

        public String DescriptionResource { get { return _descriptionResource; } }
        public String Domain { get { return _domain; } }
        public String UserHelpResource { get { return _userHelpResource; } }
        public String TitleResource { get { return _titleResource; } }
        public EntityTypes EntityType { get { return _entityType; } }
        public Type ResourceType { get { return _resourceType; } }

        public string InsertUrl { get { return _insertUrl; } }
        public string SaveUrl { get { return _saveUrl; } }
        public string UpdateUrl { get { return _updateUrl; } }
        public string FactoryUrl { get { return _factoryUrl; } }
        public string GetUrl { get { return _getUrl; } }
        public string GetListUrl { get { return _getListUrl; } }
        public string DeleteUrl { get { return _deleteUrl; } }
        public string HelpUrl { get { return _helpUrl; } }
        public string Icon { get { return _icon; } }

        public bool Cloneable { get; private set; }
        public bool CanImport { get; private set; }
        public bool CanExport { get; private set; }

        public string ListUIUrl { get { return _listUIUrl; } }
        public string EditUIUrl { get { return _editUIUrl; } }
        public string CreateUIUrl { get { return _createUIUrl; } }
        public string PreviewUIUrl { get => _previewUIUrl; }

        public bool SaveDraft { get; }

        public bool AutoSave { get; }
        public int? AutoSaveIntervalSeconds { get; }

        // -----------------------------
        // New properties (RAG + org)
        // -----------------------------
        public string ClusterKey { get { return _clusterKey; } }
        public string EntityKey { get { return _entityKey; } }

        public ModelTypes ModelType { get { return _modelType; } }
        public EntityShapes Shape { get { return _shape; } }
        public Lifecycles Lifecycle { get { return _lifecycle; } }
        public Sensitivities Sensitivity { get { return _sensitivity; } }

        public bool IndexInclude { get { return _indexInclude; } }
        public IndexTiers IndexTier { get { return _indexTier; } }

        /// <summary>
        /// 0-100, higher means "more important to embed/retrieve"
        /// </summary>
        public int IndexPriority { get { return _indexPriority; } }

        /// <summary>
        /// Comma-separated tags to keep attribute usage simple and compile-safe.
        /// Example: "ai,agents,prompting"
        /// </summary>
        public string IndexTagsCsv { get { return _indexTagsCsv; } }
    }
}
