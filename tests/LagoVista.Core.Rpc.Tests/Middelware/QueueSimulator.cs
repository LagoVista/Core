// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: c66ba66ae0e031add05701be827e293ed975b84f066bd30c7a4739a8f36f003a
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Tests.Middelware
{
    /// <summary>
    /// for end-to-end testing
    /// </summary>
    public sealed class QueueSimulator
    {
        private readonly List<IReceiver> _responseReceivers = new List<IReceiver>();
        private readonly List<IReceiver> _requestReceivers = new List<IReceiver>();

        public enum ListenerType
        {
            Request,
            Response
        }

        public void RegisterListener(IReceiver receiver, ListenerType listenerType)
        {
            switch (listenerType)
            {
                case ListenerType.Request:
                    _requestReceivers.Add(receiver);
                    break;
                case ListenerType.Response:
                    _responseReceivers.Add(receiver);
                    break;
            }
        }

        public int RequestCount { get; private set; } = 0;
        public int ResponseCount { get; private set; } = 0;

        public Task SendAsync(IRequest request)
        {
            ++RequestCount;
            Send(request);
            return Task.FromResult<object>(null);
        }

        private void Send(IRequest request)
        {
            // simulate network delay
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                foreach (var receiver in _requestReceivers)
                {
                    await receiver.ReceiveAsync(request);
                }
            });
        }

        public Task SendAsync(IResponse response)
        {
            ++ResponseCount;
            return Task.FromResult<object>(null);
        }

        private void Send(IResponse response)
        {
            // simulate network delay
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                await Task.Delay(TimeSpan.FromSeconds(1));
                foreach (var receiver in _responseReceivers)
                {
                    await receiver.ReceiveAsync(response);
                }
            });
        }
    }
}

