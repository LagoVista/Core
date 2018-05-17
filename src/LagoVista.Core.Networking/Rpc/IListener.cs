using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Networking.Rpc
{
    public sealed class MessageContext
    {
        public MessageContext(IRequest request)
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

    public sealed class ListenerExceptionArgs : EventArgs
    {
        public ListenerExceptionArgs(Exception exception, MessageContext requestContext)
        {
            Exception = exception ?? throw new ArgumentNullException("exception");
            MessageContext = requestContext ?? throw new ArgumentNullException("requestContext");
        }

        public Exception Exception { get; }

        public MessageContext MessageContext { get; }
    }

    public interface IListener
    {
        string Channel { get; }

        Task MessageReceived(IMessage message, CancellationToken token);

        Task CompleteAsync(string lockToken);

        Task DeadLetterAsync(string lockToken, string reason, string description);

        Task HandleException(ListenerExceptionArgs e);
    }
}
