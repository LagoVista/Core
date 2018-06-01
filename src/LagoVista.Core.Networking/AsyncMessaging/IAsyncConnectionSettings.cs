using LagoVista.Core.Interfaces;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    /// <summary>
    /// request side service bus toplic client settings - request sender
    /// </summary>
    public interface IServiceBusAsyncRequestSenderConnectionSettings
    {
        /// <summary>
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// DestinationEntityPath - ResourceName - [instance id]
        /// </summary>
        IConnectionSettings ServiceBusAsyncRequestSender { get; }
        
        /// <summary>
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// </summary>
        //IConnectionSettings ServiceBusTopicManager { get; }
    }

    /// <summary>
    /// response side service bus topic client settings - response sender
    /// </summary>
    public interface IServiceBusAsyncResponseSenderConnectionSettings
    {
        /// <summary>
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// DestinationEntityPath - ResourceName
        /// </summary>
        IConnectionSettings ServiceBusAsyncResponseSender { get; }

        /// <summary>
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// </summary>
        //IConnectionSettings ServiceBusTopicManager { get; }
    }

    /// <summary>
    /// request side service bus subscription client settings - response listener
    /// </summary>
    public interface IServiceBusAsyncResponseListenerConnectionSettings
    {
        /// <summary>
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// SourceEntityPath - ResourceName
        /// SubscriptionPath - Uri
        /// </summary>
        IConnectionSettings ServiceBusAsyncResponseListener { get; }

        /// <summary>
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// </summary>
        //IConnectionSettings ServiceBusTopicManager { get; }
    }

    /// <summary>
    /// response side service bus subscription client settings - request listener 
    /// </summary>
    public interface IServiceBusAsyncRequestModeratorConnectionSettings
    {
        /// <summary>
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// SourceEntityPath - ResourceName
        /// SubscriptionPath - Uri
        /// </summary>
        IConnectionSettings ServiceBusAsyncRequestModerator { get; }

        /// <summary>
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// </summary>
        //IConnectionSettings ServiceBusTopicManager { get; }
    }
}
