// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: b8cd406a9045ebd2274e46a84aed75d22986e20172c036e36d0db62324e05fe9
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public enum Channels
    {
        Org,
        Team,
        User,

        Host,
        Instance,
        DeviceRepository,
        DeviceGroup,
        Device,
        DeviceLog,
        PipelineModule,
        Route,
        DeviceConfiguration,
        MessageType,

        Dependency,

        Simulator,

        Jobs,

        DFU,

        Customer,
        ToDo,
        WorkTask,
        Inbox,

        Entity,
    }

    public enum NotificationVerbosity
    {
        Diagnostics,
        Normal,
        Errors,
    }

    public enum Targets
    {
        Sms,
        WebSocket,
        Push,
        WebSocketAndPush
    }

    public interface INotificationPublisher
    {
        Task PublishAsync(Targets target, Notification notification, NotificationVerbosity verbosity = NotificationVerbosity.Normal);
        Task PublishAsync<TPayload>(Targets target, Channels channel, string channelId, TPayload message, NotificationVerbosity verbosity = NotificationVerbosity.Normal);
        Task PublishAsync<TPayload>(Targets target, Channels channel, string channelId, String text, TPayload message, NotificationVerbosity verbosity = NotificationVerbosity.Normal);
        Task PublishAsync<TEntity>(Targets target, TEntity entity, NotificationVerbosity verbosity = NotificationVerbosity.Normal) where TEntity : IIDEntity;
        Task PublishTextAsync(Targets target, Channels channel, string channelId, String text, NotificationVerbosity verbosity = NotificationVerbosity.Normal);
    }
}
