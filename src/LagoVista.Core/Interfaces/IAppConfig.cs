using LagoVista.Core.Models;
using System;

namespace LagoVista.Core.Interfaces
{
    public enum PlatformTypes
    {
        WebApp,
        WebAPI,
        WindowsUWP,
        iPhone,
        Android,
        Runtime
    }

    public enum Environments
    {
        Production,
        Staging,
        Beta,
        Testing,
        Development,
        Local,
        LocalDevelopment
    }

    public enum AuthTypes
    {
        User,
        DeviceUser,
        ClientApp,
        Runtime
    }

    public interface IAppConfig
    {
        PlatformTypes PlatformType { get; }
        Environments Environment { get; }
        AuthTypes AuthType { get; }

        String WebAddress { get; }

        String CompanyName { get; }

        String CompanySiteLink { get; }


        String AppName { get; }
        String AppId { get; }
        String AppDescription { get; }
        String TermsAndConditionsLink { get; }
        String PrivacyStatementLink { get; }
        String ClientType { get; }

        String AppLogo { get; }
        String CompanyLogo { get; }

        string InstanceId { get; set; }
        string InstanceAuthKey { get; set; }

        String DeviceId { get; set; }
        String DeviceRepoId { get; set; }


        bool EmitTestingCode { get; }
        VersionInfo Version { get; }
    }
}
