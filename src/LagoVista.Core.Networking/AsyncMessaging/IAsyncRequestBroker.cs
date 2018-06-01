using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IAsyncRequestBroker
    {
        /// <summary>
        /// T must be an interface and must include the AsyncMessagingAttribute attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        int RegisterSubject<T>(T subject) where T : class;

        Task<IAsyncResponse> HandleRequestAsync(IAsyncRequest request);
    }
}
