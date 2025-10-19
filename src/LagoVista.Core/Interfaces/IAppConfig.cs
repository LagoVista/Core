// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 077fc7d910e8929889ea6467f841bc0773948b1b18a5aa58d3e649bb4212ed7b
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;

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
        Runtime,
        SingleUseToken,
        OrgAppEndUser,

    }

    public interface IAppConfig
    {
        PlatformTypes PlatformType { get; }
        Environments Environment { get; }
        AuthTypes AuthType { get; }

        EntityHeader SystemOwnerOrg { get; }

        String WebAddress { get; }

        String CompanyName { get; }

        String CompanySiteLink { get; }


        String AppName { get; }
        String AppId { get; }
        String APIToken { get; }
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

        String DefaultDeviceLabel { get; }
        String DefaultDeviceLabelPlural { get; }

        bool EmitTestingCode { get; }
        VersionInfo Version { get; }
        String AnalyticsKey { get; set;  }
    }
}
