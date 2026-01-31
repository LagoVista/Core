using System;
using System.Collections.Generic;

namespace LagoVista.Core.Models.UIMetaData
{
    public class DomainDescription
    {
        public DomainDescription()
        {
        }

        public String Key { get; set; }

        public String Description { get; set; }
        public String Name { get; set; }
        public string SourceAssembly { get; set; }

        public VersionInfo CurrentVersion { get; set; }
        public List<VersionInfo> VersionHistory { get; set; }

        public enum DomainTypes
        {
            Dto,
            BusinessObject,
            Storage,
            UserInterface,
            Service,
            Manager
        }

        public DomainTypes DomainType { get; set; }

        // -----------------------------
        // New: domain-level defaults
        // -----------------------------

        /// <summary>
        /// Default model classification for entities in this domain, unless overridden at the entity level.
        /// </summary>
        public LagoVista.Core.Attributes.EntityDescriptionAttribute.ModelTypes? DefaultModelType { get; set; }

        /// <summary>
        /// Default lifecycle for entities in this domain, unless overridden at the entity level.
        /// </summary>
        public LagoVista.Core.Attributes.EntityDescriptionAttribute.Lifecycles? DefaultLifecycle { get; set; }

        /// <summary>
        /// Default sensitivity for entities in this domain, unless overridden at the entity level.
        /// </summary>
        public LagoVista.Core.Attributes.EntityDescriptionAttribute.Sensitivities? DefaultSensitivity { get; set; }

        /// <summary>
        /// Default indexing tier for entities in this domain, unless overridden at the entity level.
        /// </summary>
        public LagoVista.Core.Attributes.EntityDescriptionAttribute.IndexTiers? DefaultIndexTier { get; set; }

        /// <summary>
        /// Default inclusion in indexing for entities in this domain, unless overridden at the entity level.
        /// </summary>
        public bool? DefaultIndexInclude { get; set; }

        /// <summary>
        /// Default index priority (0-100) for entities in this domain, unless overridden at the entity level.
        /// </summary>
        public int? DefaultIndexPriority { get; set; }

        /// <summary>
        /// Optional comma-separated list of cluster keys expected within this domain.
        /// Purely organizational/navigation hint.
        /// </summary>
        public List<Cluster> Clusters { get; set; } = new List<Cluster>();
    }

    public class Cluster
    {
          public string Key { get; set; }
          public string Name { get; set; }
          public string Description { get; set; } 
    }
}
