// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: aa078ecfda7a210da4afa7bbd35f3e2606980b5f02738ba5e16e5f9e082aef5a
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LagoVista.Core.Tests.UIMetaData
{
    [DomainDescriptorAttribute]
    public class Domains
    {
        public const String MetaData1 = "METADATA1";
        public const String MetaData2 = "METADATA2";
        [DomainDescription(MetaData1)]
        public static DomainDescription MetaData1Domain
        {
            get
            {
                return new DomainDescription()
                {
                    CurrentVersion = new Core.Models.VersionInfo()
                    {
                        Major = 1,
                        Minor = 1,
                        Build = 20161219,
                        Revision = 1,
                        DateStamp = new DateTime(2016, 12, 19),
                        ReleaseNotes = "Cool"
                    },
                    Description = "Manage complex, structured metadata and its related child elements across this domain.",
                    DomainType = DomainDescription.DomainTypes.BusinessObject,
                    Name = "Metadata Management",
                };
            }
        }

        [DomainDescription(MetaData2)]
        public static DomainDescription MetaData2Domain
        {
            get
            {
                return new DomainDescription()
                {
                    CurrentVersion = new Core.Models.VersionInfo()
                    {
                        Major = 1,
                        Minor = 1,
                        Build = 20161219,
                        Revision = 1,
                        DateStamp = new DateTime(2016, 12, 19),
                        ReleaseNotes = "Cool"
                    },
                    Description = "Meta Data Two Description",
                    DomainType = DomainDescription.DomainTypes.BusinessObject,
                    Name = "Meta Data Two",
                };
            }
        }
    }
}