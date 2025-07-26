using System;
using System.Threading;
using System.Threading.Tasks;

namespace LagoVista.Core.Interfaces
{
    public interface IBackgroundServiceTaskQueue
    {
        Task QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem);
        bool TryQueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);


        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }

    public static class BackgroundServiceTaskQueueProvider
    {
        public static IBackgroundServiceTaskQueue Instance { get; set; }
    }
}
