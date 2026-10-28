using LagoVista.Core.Models;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    /// <summary>
    /// Additive delivery-target flags. The legacy Targets enum and
    /// INotificationPublisher signatures remain unchanged.
    /// </summary>
    [Flags]
    public enum NotificationTargets
    {
        None = 0,
        WebSocket = 1 << 0,
        Push = 1 << 1,
        Browser = 1 << 2,
        Sms = 1 << 3
    }

    /// <summary>
    /// Optional publisher capability for callers that explicitly select
    /// multiple delivery transports. Existing publishers may continue to
    /// implement INotificationPublisher only.
    /// </summary>
    public interface INotificationPublisherV2 : INotificationPublisher
    {
        Task PublishAsync(NotificationTargets targets, Notification notification,
            NotificationVerbosity verbosity = NotificationVerbosity.Normal);

        Task PublishAsync<TPayload>(NotificationTargets targets, Channels channel,
            string channelId, TPayload message,
            NotificationVerbosity verbosity = NotificationVerbosity.Normal);

        Task PublishAsync<TPayload>(NotificationTargets targets, Channels channel,
            string channelId, string text, TPayload message,
            NotificationVerbosity verbosity = NotificationVerbosity.Normal);

        Task PublishAsync<TEntity>(NotificationTargets targets, TEntity entity,
            NotificationVerbosity verbosity = NotificationVerbosity.Normal)
            where TEntity : IIDEntity;

        Task PublishTextAsync(NotificationTargets targets, Channels channel,
            string channelId, string text,
            NotificationVerbosity verbosity = NotificationVerbosity.Normal);
    }
}
