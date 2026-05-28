using LagoVista.Core.Interfaces;
using LagoVista.Core.Models;
using LagoVista.Core.Rcg.Client.Interfaces;
using LagoVista.Core.Validation;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.Rcg.Client.Services
{
    public class RcgGatewayHttpClient : IRcgGatewayHttpClient
    {
        private readonly ISignedServiceHttpClient _signedServiceHttpClient;

        public RcgGatewayHttpClient(ISignedServiceHttpClient signedServiceHttpClient)
        {
            _signedServiceHttpClient = signedServiceHttpClient ?? throw new ArgumentNullException(nameof(signedServiceHttpClient));
        }

        public Task<InvokeResult<TResult>> GetAsync<TResult>(string pathAndQuery, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrWhiteSpace(pathAndQuery)) throw new ArgumentNullException(nameof(pathAndQuery));
            if (EntityHeader.IsNullOrEmpty(org)) return Task.FromResult(InvokeResult<TResult>.FromError("Organization is required."));
            if (EntityHeader.IsNullOrEmpty(user)) return Task.FromResult(InvokeResult<TResult>.FromError("User is required."));

            return _signedServiceHttpClient.GetAsync<TResult>(SignedServiceHttpTarget.RcgServer, pathAndQuery);
        }

        public Task<InvokeResult<TResult>> PostAsync<TRequest, TResult>(string pathAndQuery, TRequest request, EntityHeader org, EntityHeader user)
        {
            if (String.IsNullOrWhiteSpace(pathAndQuery)) throw new ArgumentNullException(nameof(pathAndQuery));
            if (EntityHeader.IsNullOrEmpty(org)) return Task.FromResult(InvokeResult<TResult>.FromError("Organization is required."));
            if (EntityHeader.IsNullOrEmpty(user)) return Task.FromResult(InvokeResult<TResult>.FromError("User is required."));

            return _signedServiceHttpClient.PostAsync<TRequest, TResult>(SignedServiceHttpTarget.RcgServer, pathAndQuery, request);
        }
    }
}