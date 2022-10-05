using LagoVista.Core.Diagnostics.ConsoleProxy;
using LagoVista.Core.Interfaces;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Client.ServiceBus;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using LagoVista.Core.Rpc.Server;
using LagoVista.Core.Rpc.Server.ServiceBus;
using LagoVista.Core.Rpc.Tests.Server.Utils;
using LagoVista.Core.Utils;
using LagoVista.Core.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Server
{
    [TestClass]
    public class ASB_RPC_EndToEnd_Test : RPC_TestBase
    {
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

            var requestBroker = new LagoVista.Core.Rpc.Server.RequestBroker();
            requestBroker.AddService<IRemoteClass>(new RemoteClass());

            var coupler = new AsyncCoupler<IMessage>(logger, new TestUsageMetrics("rpc", "rpc", "rpc") { Version = "N/A" });
            var server = new ServiceBusRequestServer(requestBroker, logger);            
            var client = new ServiceBusProxyClient(coupler, logger);

            var consoleProxyFactory = new ConsoleProxyFactory(new ConsoleWriter(), true);
            var asyncCoupler = consoleProxyFactory.Create<IAsyncCoupler<IMessage>>(coupler);
            var rpcClientTransceiver = consoleProxyFactory.Create<ITransceiver>(client);

            var proxyFactory = new LagoVista.Core.Rpc.Client.ProxyFactory(GetSettings(), client, coupler, logger);

            var proxy = proxyFactory.Create<IRemoteClass>(new ProxySettings()
            {
                 InstanceId = INSTANCE_ID,
                 OrganizationId = ORG_ID
            });

            // 
            await rpcClientTransceiver.StartAsync(GetSettings());

            // Remote Server (will execute methods);
            await server.StartAsync(GetSettings());


            var result = proxy.AddIt(1, 1);
            Assert.AreEqual(2, result);

            var results = await proxy.GetItems(5);
            Assert.AreEqual(5, results.Result.Count);
            foreach(var item in results.Result)
            {
                Console.WriteLine(item);
            }
        }
    }
}
