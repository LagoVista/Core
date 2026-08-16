// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: e1255a2033199156d65c73676a4268f0921327f42fcbab251d8428841e741815
// IndexVersion: 2
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

    /// <summary>
    /// Optional support used by graceful shutdown to drain work that was already accepted before
    /// shutdown, including continuations created by an active admitted operation.
    /// </summary>
    public interface IBackgroundServiceTaskQueueDrainSupport
    {
        int PendingCount { get; }
        bool TryDequeue(out Func<CancellationToken, Task> workItem);
    }

    public static class BackgroundServiceTaskQueueProvider
    {
        public static IBackgroundServiceTaskQueue Instance { get; set; }
    }
}
