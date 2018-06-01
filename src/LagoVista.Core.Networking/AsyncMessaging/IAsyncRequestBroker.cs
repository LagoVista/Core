using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncRequestBroker
    {
        /// <summary>
        /// T must be an interface and must include the AsyncMessagingAttribute attribute
        /// </summary>
        /// <typeparam name="TImplementation"></typeparam>
        /// <param name="subject"></param>
        int AddRequestHandler<TInterface>(TInterface subject) where TInterface : class;

        Task<IAsyncResponse> HandleRequestAsync(IAsyncRequest request);
    }
}
