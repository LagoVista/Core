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
                        Major = 1,
                        Minor = 3,
                        Build = 002,
                        DateStamp = new DateTime(2018, 4, 27),
                        Revision = 1,
                        ReleaseNotes = "Beta Quality"
                    }
                };
            }
        }
    }
}
