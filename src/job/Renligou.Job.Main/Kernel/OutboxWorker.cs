using Renligou.Core.Infrastructure.Data.Outbox;
using Renligou.Core.Infrastructure.Event;

namespace Renligou.Job.Main.Kernel
{
    public class OutboxWorker(
        ILogger<OutboxWorker> _logger,
        IServiceScopeFactory _scopeFactory
    ) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessAsync(stoppingToken);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public async Task ProcessAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IOutboxDapperRepository>();
            var publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

            var batch = await repo.DequeueBatchAsync(batchSize: 50, stoppingToken);
            if (batch.Count == 0)
            {
                return;
            }

            var sent = new List<long>();
            var failed = new List<long>();

            foreach (var row in batch)
            {
                try
                {
                    await publisher.PublishAsync(row, stoppingToken);
                    sent.Add(row.Id);
                }
                catch (Exception ex)
                {
                    failed.Add(row.Id);
                    _logger.LogError(ex, "Failed to publish outbox event {OutboxId}", row.Id);
                }
            }

            await repo.MarkSentAsync(sent.ToArray(), stoppingToken);
            await repo.MarkFailedAsync(failed.ToArray(), maxRetry: 5, stoppingToken);
        }
    }
}
