using LagoVista.Core.Interfaces;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    /// <summary>
    /// request side service bus toplic client settings - request sender
    /// </summary>
    public interface IServiceBusAsyncRequestSenderConnectionSettings
    {
        /// <summary>
        /// Endpoint - Name
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// DestinationEntityPath - ResourceName
        /// </summary>
        IConnectionSettings ServiceBusAsyncRequestSender { get; set; }
    }

    /// <summary>
    /// response side service bus topic client settings - response sender
    /// </summary>
    public interface IServiceBusAsyncResponseSenderConnectionSettings
    {
        /// <summary>
        /// Endpoint - Name
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// DestinationEntityPath - ResourceName
        /// </summary>
        IConnectionSettings ServiceBusAsyncResponseSender { get; set; }
    }

    /// <summary>
    /// request side service bus subscription client settings - response listener
    /// </summary>
    public interface IServiceBusAsyncResponseListenerConnectionSettings
    {
        /// <summary>
        /// Endpoint - Name
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// SourceEntityPath - ResourceName
        /// SubscriptionPath - Uri
        /// </summary>
        IConnectionSettings ServiceBusAsyncResponseListener { get; set; }
    }

    /// <summary>
    /// response side service bus subscription client settings - request listener 
    /// </summary>
    public interface IServiceBusAsyncRequestModeratorConnectionSettings
    {
        /// <summary>
        /// Endpoint - Name
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// SourceEntityPath - ResourceName
        /// SubscriptionPath - Uri
        /// </summary>
        IConnectionSettings ServiceBusAsyncRequestModerator { get; set; }
    }
}
