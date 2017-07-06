using System;

namespace LagoVista.Core.Interfaces
{
    public enum PlatformTypes
    {
        WebApp,
        WebAPI,
        WindowsUWP,
        iPhone,
        Android
    }

    public enum Environments
    {
        Production,
        Staging,
        Beta,
        Testing,
        Development,
        Local
    }

    public interface IAppConfig
    {
        PlatformTypes PlatformType { get; }
        Environments Environment { get; }
        String WebAddress { get; }
        String AppName { get; }
        String AppId { get;  }
        String InstallationId { get; }
        String AppLogo { get; }
        String CompanyLogo { get; }
        bool EmitTestingCode { get; }
    }
}
