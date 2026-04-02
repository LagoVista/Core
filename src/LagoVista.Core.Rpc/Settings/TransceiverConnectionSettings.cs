// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: bd1e97e4fd1f4cf6db26ce907caea4607598a8a2bd709f57b775309e7c0cbbed
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using Microsoft.Extensions.Configuration;
using System;

namespace LagoVista.Core.Rpc.Settings
{
    /// <summary>
    /// request side service bus topic client settings - request sender
    /// </summary>
    public class TransceiverConnectionSettings : ITransceiverConnectionSettings
    {
        /// <summary>
        /// Used to create topics, used to generate SAS tokens for on premise
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// </summary>
        public IConnectionSettings RpcAdmin { get; }

        /// <summary>
        /// Used to request data from an remote server.
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// TopicPrefix - ResourceName
        /// client request timeout - TimeoutInSeconds
        /// </summary>
        public IConnectionSettings RpcClientTransmitter { get; }

        /// <summary>
        /// Used to subscribe to messages from a remote server 
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// Topic - ResourceName
        /// SubscriptionPath - Uri
        /// </summary>
        public IConnectionSettings RpcClientReceiver { get; }

        /// <summary>
        /// Used to send messages to client/requester
        /// should have write-only permissions to send to SB
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// </summary>
        public IConnectionSettings RpcServerTransmitter { get; }

        /// <summary>
        /// Used to subscribe and receive messages from client requests
        /// should have readonly to topic that the instance subscribes to
        /// Endpoint - AccountId
        /// SharedAccessKeyName - UserName
        /// SharedAccessKey - AccessKey
        /// SubscriptionPath - Uri
        /// </summary>
        public IConnectionSettings RpcServerReceiver { get; }


        public TransceiverConnectionSettings(IConfiguration configuration)
        {
            var appKey = configuration.Require("AppKey");

            var rpcAdminSection = configuration.GetSection("RpcAdmin");
            RpcAdmin = new LagoVista.Core.Models.ConnectionSettings()
            {
                AccountId = rpcAdminSection.Require("Endpoint"),
                UserName = rpcAdminSection.Require("SharedAccessKeyName"),
                AccessKey = rpcAdminSection.Require("SharedAccessKey"),
            };

            var rpcClientTransmitterSection = configuration.GetSection("RpcClientTransmitter");
            RpcClientTransmitter = new LagoVista.Core.Models.ConnectionSettings()
            {
                AccountId = rpcClientTransmitterSection.Require("Endpoint"),
                UserName = rpcClientTransmitterSection.Require("SharedAccessKeyName"),
                AccessKey = rpcClientTransmitterSection.Require("SharedAccessKey"),
                ResourceName = rpcClientTransmitterSection.Require("TopicPrefix"),
                TimeoutInSeconds = Convert.ToInt32(rpcClientTransmitterSection.Require("TimeoutInSeconds"))
            };

            var rpcClientReceiverSection = configuration.GetSection("RpcClientReceiver");
            var appReceiverSection = rpcClientReceiverSection.GetSection(appKey);
            RpcClientReceiver = new LagoVista.Core.Models.ConnectionSettings()
            {
                AccountId = appReceiverSection.Require("Endpoint"),
                UserName = appReceiverSection.Require("SharedAccessKeyName"),
                AccessKey = appReceiverSection.Require("SharedAccessKey"),
                ResourceName = appReceiverSection.Require("SourceEntityPath"),
                Uri = appReceiverSection.Require("SubscriptionPath"),
            };

            var rpcServerTransmitterSection = configuration.GetSection("RpcServerTransmitter");
            RpcServerTransmitter = new LagoVista.Core.Models.ConnectionSettings()
            {
                AccountId = rpcServerTransmitterSection.Require("Endpoint"),
                UserName = rpcServerTransmitterSection.Require("SharedAccessKeyName"),
                AccessKey = rpcServerTransmitterSection.Require("SharedAccessKey"),
            };

            var rpcServerReceiverSection = configuration.GetSection("RpcServerReceiver");
            RpcServerReceiver = new LagoVista.Core.Models.ConnectionSettings()
            {
                AccountId = rpcServerReceiverSection.Require("Endpoint"),
                UserName = rpcServerReceiverSection.Require("SharedAccessKeyName"),
                AccessKey = rpcServerReceiverSection.Require("SharedAccessKey"),
                Uri = rpcServerReceiverSection.Require("SubscriptionPath"),
            };
        }
    }
}