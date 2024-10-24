using LagoVista.Core.Interfaces;
using System.Threading.Channels;

namespace LagoVista.Core.BackgroundProcessing
{
    public class BackgroundTaskQueue : IBackgroundServiceTaskQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue;

        private string Id = Guid.NewGuid().ToString();

        public BackgroundTaskQueue(int capacity)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);

            Console.WriteLine($"[BackgroundTaskQueue__Constructor] Capacity {capacity}  ID: {Id}");
        }

        public async Task QueueBackgroundWorkItemAsync(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            await _queue.Writer.WriteAsync(workItem);
        }

        public bool TryQueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            //Task.Run(async () =>
            //{
            //    await _queue.Writer.WriteAsync(workItem);
            //    Console.WriteLine($"[Queue]  queued item {Id}");
            //});

            var addQueueResult = _queue.Writer.TryWrite(workItem);
            if (!addQueueResult)
            {
                Console.WriteLine("Could not queue background task");
            }
            return addQueueResult;
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}
