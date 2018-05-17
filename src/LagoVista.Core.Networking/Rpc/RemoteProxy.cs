using LagoVista.Core.PlatformSupport;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc
{
    public abstract class RemoteProxy : IListener, IProxy
    {
        protected AsyncCoupler<IResponse> AsyncCoupler { get; }
        protected ISender Sender { get; }

        public RemoteProxy(AsyncCoupler<IResponse> asyncCoupler, ISender sender)
        {
            AsyncCoupler = asyncCoupler ?? throw new ArgumentNullException("asyncCoupler");
            Sender = sender ?? throw new ArgumentNullException("sender");
        }

        public string Channel { get; }

        public async Task MessageReceived(IMessage message, CancellationToken token)
        {
            try
            {
                var response = (IResponse)message;
                await AsyncCoupler.CompleteAsync(response.CorrelationId, response);
                await CompleteAsync(message.LockToken);
            }
            catch (Exception ex)
            {
                //todo: log
                await DeadLetterAsync(message.LockToken, ex.GetType().FullName, ex.Message);
            }
        }

        public async Task<IResponse> CallRemoteAsync(IRequest message)
        {
            IResponse response = null;
            await Sender.SendAsync(message);
            //todo: get wait time from settings
            var invokeResult = await AsyncCoupler.WaitOnAsync(message.CorrelationId, TimeSpan.FromSeconds(30));
            if (!invokeResult.Successful)
            {
                //todo: handle errors
            }
            else
            {
                response = invokeResult.Result;
                if(!response.Success)
                {
                    //todo: handle errors
                }
            }
            return response;
        }

        public abstract Task CompleteAsync(string lockToken);

        public abstract Task DeadLetterAsync(string lockToken, string reason, string description);

        public abstract Task HandleException(ListenerExceptionArgs e);
    }
}
