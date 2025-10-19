// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: bd1e97e4fd1f4cf6db26ce907caea4607598a8a2bd709f57b775309e7c0cbbed
// IndexVersion: 0
// --- END CODE INDEX META ---
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

        /// <summary>
        /// Used to subscribe and receive messages from client requests
        /// should have readonly to topic that the instance subscribes to
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// SubscriptionPath - Uri
        /// </summary>
        IConnectionSettings RpcServerReceiver { get; }
    }
}