using LagoVista.Core.Attributes;
using LagoVista.Core.Models.UIMetaData;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core
{
    [DomainDescriptor]
    class Domain
    {
        public const string AuthenticationDomain = "AuthenticationDomain";

        [DomainDescription(AuthenticationDomain)]
        public static DomainDescription UserDomainDescription
        {
            get
            {
                return new DomainDescription()
                {
                    Description = "Models and Services for Authentication of remote clients.",
                    DomainType = DomainDescription.DomainTypes.BusinessObject,
                    Name = "Authentication",
                    CurrentVersion = new Core.Models.VersionInfo()
                    {
                        Major = 0,
                        Minor = 8,
                        Build = 001,
                        DateStamp = new DateTime(2016, 12, 20),
                        Revision = 1,
                        ReleaseNotes = "Initial unstable preview"
                    }
                };
            }
        }
    }
}
