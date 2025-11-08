// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e1255a2033199156d65c73676a4268f0921327f42fcbab251d8428841e741815
// IndexVersion: 1
// --- END CODE INDEX META ---
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
