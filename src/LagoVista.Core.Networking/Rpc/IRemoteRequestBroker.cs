using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc
{
    public sealed class RequestContext
    {
        public RequestContext(IRemoteRequest request)
        {
            Id = request.Id;
            CorrelationId = request.CorrelationId;
            Name = request.Name;
            MarshalledData = request.MarshalledData;
        }
        public string Id { get; }
        public string CorrelationId { get; }
        public string Name { get; }
        public byte[] MarshalledData { get; }
        /// <summary>
        /// a good place to put the Azure ExceptionReceivedContext data as json
        /// </summary>
        public string Other { get; set; }
    }

    public sealed class RequestExceptionArgs : EventArgs
    {
        public RequestExceptionArgs(Exception exception, RequestContext requestContext)
        {
            Exception = exception ?? throw new ArgumentNullException("exception");
            RequestContext = requestContext ?? throw new ArgumentNullException("requestContext");
        }

        public Exception Exception { get; }

        public RequestContext RequestContext { get; }
    }

    public interface IRemoteRequestBroker
    {
        void RegisterSubject<T>(T subject) where T : class;

        Task StartAsync();

        Task StopAsync();

        Task OnRequest(IRemoteRequest request, CancellationToken token);

        Task OnException(RequestExceptionArgs e);
    }
}
