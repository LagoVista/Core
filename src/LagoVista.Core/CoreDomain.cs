// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 081307cf3bc817fa33d2bbc8715b7087b0773c3c86342adf33db4495316f02fc
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using System;
using System.Collections.Generic;
using LagoVista.Core.Models.UIMetaData;

namespace LagoVista.Core
{
    [DomainDescriptor]
    public class Domains
    {
        public const string CoreDomainName = "CoreDomain";

        [DomainDescription(CoreDomainName)]
        public static DomainDescription CoreDomain
        {
            get
            {
                return new DomainDescription()
                {
                    Description = "Contains core models and related management features.",
                    DomainType = DomainDescription.DomainTypes.BusinessObject,
                    Name = "Core",
                    CurrentVersion = new Core.Models.VersionInfo()
                    {
                        Major = 0,
                        Minor = 4,
                        Build = 001,
                        DateStamp = new DateTime(2024, 3, 21),
                        Revision = 1,
                        ReleaseNotes = ""
                    },

                    // New defaults
                    DefaultSensitivity = LagoVista.Core.Attributes.EntityDescriptionAttribute.Sensitivities.Internal,
                    DefaultLifecycle = LagoVista.Core.Attributes.EntityDescriptionAttribute.Lifecycles.DesignTime,
                    DefaultModelType = LagoVista.Core.Attributes.EntityDescriptionAttribute.ModelTypes.DomainEntity,
                    DefaultIndexTier = LagoVista.Core.Attributes.EntityDescriptionAttribute.IndexTiers.Secondary,
                    DefaultIndexInclude = true,
                    DefaultIndexPriority = 60,

                    // Optional: hint clusters you expect in CoreDomain
                    Clusters = new List<Cluster>()
                };
            }
        }
    }

}