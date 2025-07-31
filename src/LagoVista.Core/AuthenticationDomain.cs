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
                    Description = "Models and Services for Authentication of remote clients.",
                    DomainType = DomainDescription.DomainTypes.BusinessObject,
                    Name = "Authentication",
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
}
