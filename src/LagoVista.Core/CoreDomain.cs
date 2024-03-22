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
        public static DomainDescription BillingDomain
        {
            get
            {
                return new DomainDescription()
                {
                    Description = "Contains core models and managers.",
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
                    }
                };
            }
        }
    }

}