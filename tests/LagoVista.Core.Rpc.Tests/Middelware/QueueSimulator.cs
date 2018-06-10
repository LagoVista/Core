using LagoVista.Core.Rpc.Messages;
using LagoVista.Core.Rpc.Middleware;
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

        public async Task Send(IRequest request)
        {
            foreach (var receiver in _requestReceivers)
            {
                await receiver.ReceiveAsync(request);
            }
        }

        public async Task Send(IResponse response)
        {
            foreach (var receiver in _responseReceivers)
            {
                await receiver.ReceiveAsync(response);
            }
        }
    }
}
