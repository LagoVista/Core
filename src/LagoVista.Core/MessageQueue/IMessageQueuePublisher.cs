using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.MessageQueue
{
    public interface IMessageQueuePublisher
    {
        Task PublishAsync<T>(T payload, CancellationToken cancellationToken = default);
    }
}
