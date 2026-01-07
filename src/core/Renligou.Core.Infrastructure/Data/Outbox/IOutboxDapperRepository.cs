using Renligou.Core.Infrastructure.Data.Outbox;

namespace Renligou.Core.Infrastructure.Data.Outbox
{
    public interface IOutboxDapperRepository
    {
        /// <summary>
        /// Asynchronously retrieves and marks a batch of new outbox events for processing.
        /// </summary>
        Task<List<OutboxRow>> DequeueBatchAsync(int batchSize, CancellationToken ct);

        /// <summary>
        /// Marks the specified events as sent by updating their status in the data store asynchronously.
        /// </summary>
        Task MarkSentAsync(long[] ids, CancellationToken ct);

        /// <summary>
        /// Marks the specified events as failed if their retry count reaches the maximum allowed, or increments their
        /// retry count otherwise.
        /// </summary>
        Task MarkFailedAsync(long[] ids, int maxRetry, CancellationToken ct);
    }
}
