using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using System;

namespace LagoVista.Core.Models
{
    public class CoreAppServices : ICoreAppServices
    {
        public CoreAppServices(ILogger logger, ISerialNumberManager serialNumberManager, ISystemUsers systemUsers, IAppConfig appConfig, INotificationPublisher notificationPublisher, ICoreEmailServices emailServices,
                                ISecureStorage secureStorage, IDependencyManager dependencyManager, ISecurity security)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            SystemUsers = systemUsers ?? throw new ArgumentNullException(nameof(systemUsers));
            AppConfig = appConfig ?? throw new ArgumentNullException(nameof(appConfig));
            DependencyManager = dependencyManager ?? throw new ArgumentNullException(nameof(dependencyManager));
            Security = security ?? throw new ArgumentNullException(nameof(security));
            NotificationPublisher = notificationPublisher ?? throw new ArgumentNullException(nameof(notificationPublisher));
            EmailServices = emailServices ?? throw new ArgumentNullException(nameof(emailServices));
            SecureStorage = secureStorage ?? throw new ArgumentNullException(nameof(secureStorage));
            SerialNumberManager = serialNumberManager ?? throw new ArgumentNullException(nameof(serialNumberManager));
        }

        public ISerialNumberManager SerialNumberManager { get; }
        public ISecureStorage SecureStorage { get; }
        public ICoreEmailServices EmailServices { get; }
        public INotificationPublisher NotificationPublisher { get; }
        public ISystemUsers SystemUsers { get; }
        public ILogger Logger { get; }
        public IAppConfig AppConfig { get; }
        public IDependencyManager DependencyManager { get; }
        public ISecurity Security { get; }
    }
}
