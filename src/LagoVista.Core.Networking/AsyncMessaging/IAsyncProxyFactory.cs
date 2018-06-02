using System;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncProxyFactory
    {
        TProxy Create<TProxy>(
            string organizationId,
            string instanceId,
            TimeSpan timeout);
    }
}
