// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 4465df10328ecaf72bb5c3f857c31c4f8b321a1a0cf3b739b2563601c5e0da6f
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Rpc.Tests.Server.Utils;
using System;

namespace LagoVista.Core.Rpc.Tests.Server
{
    public class RPC_TestBase
    {
        public const string INSTANCE_ID = "2E135539BE86496691E0B233CFD63C09";
        public const string ORG_ID = "2CD887EE659846199CAE32FFA514F120";

        protected ITransceiverConnectionSettings GetSettings(string instanceId = INSTANCE_ID)
        {
            var settings = new TransceiverSettings()
            {
                RpcAdmin = new ConnectionSettings()
                {
                    AccountId = System.Environment.GetEnvironmentVariable("TEST_SB_ACCOUNT_ID"),
                    UserName = System.Environment.GetEnvironmentVariable("TEST_SB_POLICY_NAME"),
                    AccessKey = System.Environment.GetEnvironmentVariable("TEST_SB_ACCESSKEY"),
                },
                RpcClientTransmitter = new ConnectionSettings()
                {
                    AccountId = System.Environment.GetEnvironmentVariable("TEST_SB_ACCOUNT_ID"),
                    UserName = System.Environment.GetEnvironmentVariable("TEST_SB_POLICY_NAME"),
                    AccessKey = System.Environment.GetEnvironmentVariable("TEST_SB_ACCESSKEY"),
                    ResourceName = "rpc_request",
                    TimeoutInSeconds = 30,
                },
                RpcClientReceiver = new ConnectionSettings()
                {
                    AccountId = System.Environment.GetEnvironmentVariable("TEST_SB_ACCOUNT_ID"),
                    UserName = System.Environment.GetEnvironmentVariable("TEST_SB_POLICY_NAME"),
                    AccessKey = System.Environment.GetEnvironmentVariable("TEST_SB_ACCESSKEY"),
                    ResourceName = "rpc_response_iot_web_portal",
                    Uri = "application"
                },
                RpcServerTransmitter = new ConnectionSettings()
                {
                    AccountId = System.Environment.GetEnvironmentVariable("TEST_SB_ACCOUNT_ID"),
                    UserName = System.Environment.GetEnvironmentVariable("TEST_SB_POLICY_NAME"),
                    AccessKey = System.Environment.GetEnvironmentVariable("TEST_SB_ACCESSKEY"),
                },
                RpcServerReceiver = new ConnectionSettings()
                {
                    AccountId = System.Environment.GetEnvironmentVariable("TEST_SB_ACCOUNT_ID"),
                    UserName = System.Environment.GetEnvironmentVariable("TEST_SB_POLICY_NAME"),
                    AccessKey = System.Environment.GetEnvironmentVariable("TEST_SB_ACCESSKEY"),
                    ResourceName = $"rpc_request_{instanceId}/application"
                },
            };

            settings.RpcServerTransmitter.AccessKey = AzureSaSBuilder.BuildSignature(settings.RpcClientReceiver.AccountId, settings.RpcClientReceiver.ResourceName,
                settings.RpcServerReceiver.UserName, settings.RpcServerReceiver.AccessKey,
                DateTime.Now.AddMinutes(15));

            settings.RpcServerReceiver.AccessKey = AzureSaSBuilder.BuildSignature(settings.RpcClientReceiver.AccountId, settings.RpcClientReceiver.ResourceName, 
                settings.RpcServerReceiver.UserName, settings.RpcServerReceiver.AccessKey,
                DateTime.Now.AddMinutes(15));

            return settings;
        }

        protected Request GetMessage()
        {
            return new Request()
            {
                Id = Guid.NewGuid().ToId(),
                CorrelationId = Guid.NewGuid().ToId(),
                OrganizationId = ORG_ID,
                InstanceId = INSTANCE_ID
            };
        }
    }

}
