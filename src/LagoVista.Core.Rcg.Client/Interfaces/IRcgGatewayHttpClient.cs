using LagoVista.Core.Models;
using LagoVista.Core.Validation;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Interfaces
{
    public interface IRcgGatewayHttpClient
    {
        Task<InvokeResult<TResult>> GetAsync<TResult>(string pathAndQuery, EntityHeader org, EntityHeader user);
        Task<InvokeResult<TResult>> PostAsync<TRequest, TResult>(string pathAndQuery, TRequest request, EntityHeader org, EntityHeader user);
    }
}
