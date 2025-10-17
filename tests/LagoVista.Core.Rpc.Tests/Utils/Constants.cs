// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 278aa35aca37482a557064849f52f63286ff1df7801e9f43740401002b561f48
// IndexVersion: 1
// --- END CODE INDEX META ---
using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Rpc.Client;
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Tests.Middelware;
using LagoVista.Core.Rpc.Tests.Models;
using LagoVista.Core.Utils;
using Moq;
using System;
using System.Reflection;

namespace LagoVista.Core.Rpc.Tests.Utils
{
    public static class Constants
    {
        #region Messages
        public static readonly string MessageDestination = "Test.Destination.Path";
        public static readonly string MessageId = Guid.Parse("{C4CE5957-F9D7-4727-A20D-4C51AB5C6745}").ToString();
        public static readonly string MessageCorrelationId = Guid.Parse("{1C2FC03B-3D21-42A3-97F3-1756177DE2CB}").ToString();
        public static readonly string MessageReplyPath = "Test.ReplyTo.Path";
        public static readonly string MessageInBox = "Test.InBox";
        public static readonly DateTime MessageTimeStamp = new DateTime(2018, 1, 1, 13, 30, 30);
        #endregion

        #region Common
        public static readonly int TimeoutInSeconds = 5;
        public static readonly string OrganizationId = Guid.Parse("{8AF59E47-E473-41D1-AA86-8B557813EEFB}").ToString();
        public static readonly string InstanceId = Guid.Parse("{EC0E2AE5-7B17-4C0D-9355-1903E3284FBE}").ToString();
        public static readonly SimulatedConnectionSettings ConnectionSettings = new SimulatedConnectionSettings();
        #endregion

        #region Echo
        public static readonly MethodInfo EchoMethodInfo = typeof(IProxySubject).GetMethod(nameof(IProxySubject.Echo));
        public static readonly MethodInfo VoidTaskMethodInfo = typeof(IProxySubject).GetMethod(nameof(IProxySubject.VoidTaskMethod));
        public static readonly MethodInfo VoidMethodInfo = typeof(IProxySubject).GetMethod(nameof(IProxySubject.VoidMethod));
        public static readonly object[] EchoArgs = new object[1] { ProxySubject.EchoValueConst };
        public static readonly string EchoMethodParamValue = ProxySubject.EchoValueConst;
        public static readonly string EchoMethodParamName = "value";
        #endregion

        #region Proxy
        public static readonly IAsyncCoupler<IMessage> AsyncCoupler = new AsyncCoupler<IMessage>(new Mock<ILogger>().Object, new SimulatedUsageMetrics("rpc", "rpc", "rpc") { Version = "N/A" });
        public static readonly ProxySettings ProxySettings = new ProxySettings { InstanceId = Constants.InstanceId, OrganizationId = Constants.OrganizationId };
        #endregion

        #region Middleware
        public static readonly QueueSimulator QueueSimulator = new QueueSimulator();
        #endregion

    }
}
