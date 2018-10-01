using LagoVista.Core.Interfaces;

namespace LagoVista.Core.Rpc.Settings
{
    /// <summary>
    /// request side service bus toplic client settings - request sender
    /// </summary>
    public interface ITransceiverConnectionSettings
    {
        /// <summary>
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// DestinationEntityPath - ResourceName
        /// client request timeout - TimeoutInSeconds
        /// </summary>
        IConnectionSettings RpcTopicConstructor { get; }

        /// <summary>
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// DestinationEntityPath - ResourceName
        /// client request timeout - TimeoutInSeconds
        /// </summary>
        IConnectionSettings RpcTransmitter { get; }

        /// <summary>
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// SourceEntityPath - ResourceName
        /// SubscriptionPath - Uri
        /// </summary>
        IConnectionSettings RpcReceiver { get; }
    }
}