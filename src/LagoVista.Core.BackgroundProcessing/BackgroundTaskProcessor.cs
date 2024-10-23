using LagoVista.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace LagoVista.Core.BackgroundProcessing
{
    class BackgroundTaskProcessor : BackgroundService
    {
        private readonly PlatformSupport.ILogger _logger;
        private readonly IBackgroundServiceTaskQueue _taskQueue;

        public BackgroundTaskProcessor(IBackgroundServiceTaskQueue taskQueue,
            PlatformSupport.ILogger logger)
        {
            _taskQueue = taskQueue ?? throw new ArgumentNullException(nameof(taskQueue));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Trace(
                $"Queued Hosted Service is running.{Environment.NewLine}" +
                $"{Environment.NewLine}Tap W to add a work item to the " +
                $"background queue.{Environment.NewLine}");

            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[BackgroundTaskProcessor_Execute] - {ex.Message}");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.Trace("[BackgroundTaskProcess_StopAsync] Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
