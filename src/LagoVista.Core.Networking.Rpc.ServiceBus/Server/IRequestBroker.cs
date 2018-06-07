using LagoVista.Core.Networking.Rpc.Messages;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc
{
    public interface IRequestBroker
    {
        /// <summary>
        /// T must be an interface and must include the AsyncMessagingAttribute attribute
        /// </summary>
        /// <typeparam name="TInterface">an interface - enforced at runtime</typeparam>
        /// <param name="subject">instance that implments TInterface</param>
        /// <returns>count of methods found and registered</returns>
        int AddRequestHandler<TInterface>(TInterface subject) where TInterface : class;

        Task<IResponse> InvokeAsync(IRequest request);
    }
}
