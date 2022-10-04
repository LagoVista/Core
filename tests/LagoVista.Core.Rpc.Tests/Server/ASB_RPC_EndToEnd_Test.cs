using LagoVista.Core.Diagnostics.ConsoleProxy;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Client.ServiceBus;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Server;
using LagoVista.Core.Rpc.Server.ServiceBus;
using LagoVista.Core.Rpc.Settings;
using LagoVista.Core.Rpc.Tests.Server.Utils;
using LagoVista.Core.Utils;
using LagoVista.Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Server
{
    [TestClass]
    public class ASB_RPC_EndToEnd_Test
    {
        private ITransceiverConnectionSettings GetSettings()
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
                    ResourceName = $"rpc_request_{INSTANCE_ID}/application"
                },
            };

            settings.RpcServerReceiver.Uri = $"sb://{settings.RpcClientReceiver.AccountId}.servicebus.windows.net/";
            settings.RpcServerReceiver.AccessKey = $"SharedAccessKeyName={settings.RpcClientReceiver.UserName};SharedAccessKey={settings.RpcClientReceiver.AccessKey};";
            return settings;
        }

        const string INSTANCE_ID = "2E135539BE86496691E0B233CFD63C09";
        const string ORG_ID = "2CD887EE659846199CAE32FFA514F120";


        private Request GetMessage()
        {
            return new Request()
            {
                Id = Guid.NewGuid().ToId(),
                CorrelationId = Guid.NewGuid().ToId(),
                OrganizationId = ORG_ID,
                InstanceId = INSTANCE_ID
            };
        }

        IRequestBroker _requestBroker;

        public interface IRemoteClass
        {
            int AddIt(int a, int b);
            Task<InvokeResult<List<string>>> GetItems(int count);
        }

        public class RemoteClass : IRemoteClass
        {
            public int AddIt(int a, int b) => a + b;

            public async Task<InvokeResult<List<string>>> GetItems(int count)
            {
                var items = new List<string>();
                for (var idx = 0; idx < count; idx++)
                    items.Add(idx.ToString());
                await Task.Delay(5);
                return InvokeResult<List<string>>.Create(items);
            }
        }



        [TestMethod]
        public async Task EndToEnd()
        {
            var logger = new TestLogger();

            _requestBroker = new LagoVista.Core.Rpc.Server.RequestBroker();
            _requestBroker.AddService<IRemoteClass>(new RemoteClass());

            var coupler = new AsyncCoupler<IMessage>(logger, new TestUsageMetrics("rpc", "rpc", "rpc") { Version = "N/A" });
            var server = new ServiceBusRequestServer(_requestBroker, logger);            
            var client = new ServiceBusProxyClient(coupler, logger);

            var consoleProxyFactory = new ConsoleProxyFactory(new ConsoleWriter(), true);
            var asyncCoupler = consoleProxyFactory.Create<IAsyncCoupler<IMessage>>(coupler);
            var rpcTransceiver = consoleProxyFactory.Create<ITransceiver>(new ServiceBusProxyClient(asyncCoupler, logger));

            var proxyFactory = new LagoVista.Core.Rpc.Client.ProxyFactory(GetSettings(), client, coupler, logger);

            var proxy = proxyFactory.Create<IRemoteClass>(new ProxySettings()
            {
                 InstanceId = INSTANCE_ID,
                 OrganizationId = ORG_ID
            });

            await client.StartAsync(GetSettings());
            await server.StartAsync(GetSettings());

            var result = proxy.AddIt(1, 1);
            Assert.AreEqual(2, result);

            var results = await proxy.GetItems(5);
            foreach(var item in results.Result)
            {
                Console.WriteLine(item);
            }
        }
    }
}
