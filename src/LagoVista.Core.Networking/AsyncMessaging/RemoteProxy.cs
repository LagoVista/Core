using LagoVista.Core.Interfaces;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    /* Creating IProxy here isn't that terribely important, I don't think, since this is just the factory stuff */
    //public abstract class RemoteProxy : IRequestListener, IProxy
    //{
    //    protected AsyncCoupler<IResponse> AsyncCoupler { get; }
    //    protected ISender Sender { get; }

    //    public RemoteProxy(AsyncCoupler<IResponse> asyncCoupler, ISender sender)
    //    {
    //        AsyncCoupler = asyncCoupler ?? throw new ArgumentNullException("asyncCoupler");
    //        Sender = sender ?? throw new ArgumentNullException("sender");
    //    }

    //    public string Channel { get; }

    //    public async Task MessageReceived(IMessage message, CancellationToken token)
    //    {
    //        try
    //        {
    //            var response = (IResponse)message;
    //            await AsyncCoupler.CompleteAsync(response.CorrelationId, response);
    //            await CompleteAsync(message.LockToken);
    //        }
    //        catch (Exception ex)
    //        {
    //            //todo: log
    //            await DeadLetterAsync(message.LockToken, ex.GetType().FullName, ex.Message);
    //        }
    //    }

    //    public async Task<IResponse> CallRemoteAsync(IRequest message)
    //    {
    //        IResponse response = null;
    //        await Sender.SendAsync(message);
    //        //todo: get wait time from settings
    //        var invokeResult = await AsyncCoupler.WaitOnAsync(message.CorrelationId, TimeSpan.FromSeconds(30));
    //        if (!invokeResult.Successful)
    //        {
    //            //todo: handle errors
    //        }
    //        else
    //        {
    //            response = invokeResult.Result;
    //            if(!response.Success)
    //            {
    //                //todo: handle errors
    //            }
    //        }
    //        return response;
    //    }

    //    public abstract Task CompleteAsync(string lockToken);

    //    public abstract Task DeadLetterAsync(string lockToken, string reason, string description);

    //    public abstract Task HandleException(ListenerExceptionArgs e);
    //}

    /* Lifetime will be for life of the request - Instance will be created as a transient with each request, same as rest of request handler classes */
    // this is to implement DispatchProxy
    public interface IRemoteProxyFactory
    {
        /* ISender is also created as a transient, and as an input on the constructor will receive the connection settings */
        TRemoteClass Create<TRemoteClass>(IAsyncCoupler asyncCoupler, ISender sender);
    }

    /* Lifetime will be a singleton that is created on startup of the app, since it's handling OnMessage, we can't create via the transient method as above. */
    public interface IRemoteProxyResponseListener /*probably come up with a better name */
    {
        /* These two methods will be called in startup - question what happens if listener ever fails?  We can realistically only ever have one thing listening to subscriptions since we can predeict which one will get the message and clear it. */
        void Init(IAsyncCoupler asyncCoupler, IConnectionSettings connectionSettings);

        /* Kick off loop, should throw exception and log, if this fails, something really, bad happened */
         void Start();
    }
}
