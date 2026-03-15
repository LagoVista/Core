using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.MessageQueue
{
    public interface IMessageQueueHandler<T>
    {
        Task HandleAsync(MessageQueueContext<T> context, CancellationToken cancellationToken = default);
    }
}
