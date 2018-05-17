using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.AsyncMessaging
{
    public interface IRequestBroker
    {
        /// <summary>
        /// T must be an interface and must include the IsRemotableAttribute attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subject"></param>
        void RegisterSubject<T>(T subject) where T : class;

        Task<InvokeResult> HandleMessageAsync(IAsyncRequest message);
    }

    public class QueueRequestBroker: IRequestBroker
    {
        public QueueRequestBroker(ISender sender)
        {
        }

        public Task<InvokeResult> HandleMessageAsync(IAsyncRequest message)
        {
            throw new System.NotImplementedException();
        }

        public void RegisterSubject<T>(T subject) where T : class
        {
            throw new System.NotImplementedException();
        }
    }
}
