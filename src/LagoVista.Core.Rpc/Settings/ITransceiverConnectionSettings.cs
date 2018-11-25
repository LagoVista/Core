using LagoVista.Core.Interfaces;

namespace LagoVista.Core.Rpc.Settings
{
    /// <summary>
    /// request side service bus topic client settings - request sender
    /// </summary>
    public interface ITransceiverConnectionSettings
    {
        /// <summary>
        /// Used to create topics, used to generate SAS tokens for on premise
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// </summary>
        IConnectionSettings RpcAdmin { get; }

        /// <summary>
        /// Used to request data from an remote server.
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// TopicPrefix - ResourceName
        /// client request timeout - TimeoutInSeconds
        /// </summary>
        IConnectionSettings RpcClientTransmitter { get; }

        /// <summary>
        /// Used to subscribe to messages from a remote server 
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// Topic - ResourceName
        /// SubscriptionPath - Uri
        /// </summary>
        IConnectionSettings RpcClientReceiver { get; }

        /// <summary>
        /// Used to send messages to client/requester
        /// should have write-only permissions to send to SB
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// </summary>
        IConnectionSettings RpcServerTransmitter { get; }
    }
}