using LagoVista.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace LagoVista.Core.BackgroundProcessing
{
    class BackgroundTaskProcessor : BackgroundService
    {
        private readonly IBackgroundServiceTaskQueue _taskQueue;

        public BackgroundTaskProcessor(IBackgroundServiceTaskQueue taskQueue)
        {
            _taskQueue = taskQueue ?? throw new ArgumentNullException(nameof(taskQueue));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            Console.WriteLine("[BackgroundTaskProcessor__Execute] Start Processing");

            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    var sw = Stopwatch.StartNew();
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BackgroundTaskProcessor_Execute] - Exception {ex.Message}");
                }
            }

            Console.WriteLine("[BackgroundTaskProcessor__Execute] Done Processing - Cancel was reqquested");

        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("[BackgroundTaskProcessor__StopAsync]");

            await base.StopAsync(stoppingToken);
        }
    }

}
