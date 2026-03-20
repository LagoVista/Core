// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 7f795fdbcd98374ad52f24746ea51139a48858f7907d384c9d45175408b204f2
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    [DomainDescriptor]
    public class AuthDomain
    {
        public const string AuthenticationDomain = "AuthenticationDomain";
        [DomainDescription(AuthenticationDomain)]
        public static DomainDescription UserDomainDescription
        {
            get
            {
                return new DomainDescription()
                {
                    Description = "Shared models used throughout the LagoVista system.",
                    DomainType = DomainDescription.DomainTypes.BusinessObject,
                    Name = "Common",
                    CurrentVersion = new Core.Models.VersionInfo()
                    {
                        Major = 1,
                        Minor = 3,
                        Build = 006,
                        DateStamp = new DateTime(2018, 4, 27),
                        Revision = 1,
                        ReleaseNotes = "Beta Quality"
                    }
                };
            }
        }
    }

    [DomainDescriptor]
    public class LGVCommonDomain
    {
        public const string CommonDomain = "CommonDomain";
        [DomainDescription(CommonDomain)]
        public static DomainDescription UserDomainDescription
        {
            get
            {
                return new DomainDescription()
                {
                    Description = "Common Models across LagoVista System.",
                    DomainType = DomainDescription.DomainTypes.BusinessObject,
                    Name = "Common",
                    CurrentVersion = new Core.Models.VersionInfo()
                    {
                        Major = 1,
                        Minor = 3,
                        Build = 006,
                        DateStamp = new DateTime(2018, 4, 27),
                        Revision = 1,
                        ReleaseNotes = "Beta Quality"
                    }
                };
            }
        }
    }

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