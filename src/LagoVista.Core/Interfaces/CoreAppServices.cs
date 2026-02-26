using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.Text;

namespace LagoVista.Core.Interfaces
{
    public interface ICoreAppServices
    {
        ISecureStorage SecureStorage { get; }
        INotificationPublisher NotificationPublisher { get; }
        ICoreEmailServices EmailServices { get; }
        ISystemUsers SystemUsers { get; }
        ILogger Logger { get; }
        IAppConfig AppConfig { get; }
        IDependencyManager DependencyManager { get; }
        ISecurity Security { get; }
        ISerialNumberManager SerialNumberManager { get; }
    }
}
