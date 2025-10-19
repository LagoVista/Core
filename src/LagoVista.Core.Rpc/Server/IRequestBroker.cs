// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2ec3636abc5630e20fff6bc7923221337122eadb1c60154a357fac59c04fe275
// IndexVersion: 0
// --- END CODE INDEX META ---
using LagoVista.Core.Rpc.Messages;
using System.Threading.Tasks;

namespace LagoVista.Core.Rpc.Server
{
    public interface IRequestBroker
    {
        /// <summary>
        /// T must be an interface 
        /// </summary>
        /// <typeparam name="TInterface">an interface - enforced at runtime</typeparam>
        /// <param name="subject">instance that implments TInterface</param>
        /// <returns>count of methods found and registered</returns>
        int AddService<TInterface>(TInterface subject) where TInterface : class;

        Task<IResponse> InvokeAsync(IRequest request);
    }
}
