using LagoVista.Core.Interfaces;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface ISignedServiceHttpClient
    {
        Task<InvokeResult<TResult>> GetAsync<TResult>(SignedServiceHttpTarget target, string pathAndQuery);
        Task<InvokeResult<TResult>> PostAsync<TRequest, TResult>(SignedServiceHttpTarget target, string pathAndQuery, TRequest request);
    }
}
