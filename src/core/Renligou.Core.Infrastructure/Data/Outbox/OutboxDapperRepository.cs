using Renligou.Core.Infrastructure.Data.Connections;
using Dapper;
using MySqlConnector;
using System.Data;
using System.Linq;

namespace Renligou.Core.Infrastructure.Data.Outbox
{
    public class OutboxDapperRepository : IOutboxDapperRepository
    {
        private readonly IDbConnectionFactory _factory;

        public OutboxDapperRepository(IDbConnectionFactory factory)
            => _factory = factory;

        /// <summary>
        /// Asynchronously retrieves and marks a batch of new outbox events for processing.
        /// </summary>
        /// <remarks>This method selects up to <paramref name="batchSize"/> events with a status of 'NEW',
        /// marks them as 'SENDING' within a single transaction, and returns them for further processing. Events are
        /// locked during selection to prevent concurrent processing. If no events are available, an empty list is
        /// returned. This method is safe for concurrent callers and is intended for use in distributed event processing
        /// scenarios.</remarks>
        /// <param name="batchSize">The maximum number of outbox events to retrieve in the batch. Must be greater than zero.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A list of <see cref="OutboxRow"/> objects representing the events selected for processing. The list will be
        /// empty if no new events are available.</returns>
        public async Task<List<OutboxRow>> DequeueBatchAsync(int batchSize, CancellationToken ct)
        {
            await using var conn = _factory.Create();
            await conn.OpenAsync(ct);

            await using var tx = await conn.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);

            // 选一批 NEW（锁行）
            var rows = (await conn.QueryAsync<OutboxRow>(new CommandDefinition(@"
SELECT id AS Id, category AS Category, source_type AS SourceType, source_id AS AggregateId,
       event_type AS EventType, payload AS Payload,
       status AS Status, retry_count AS RetryCount,
       occurred_at AS OccurredAt, created_at AS CreatedAt, version AS Version
FROM _events
WHERE status = 'NEW'
ORDER BY created_at
LIMIT @BatchSize
FOR UPDATE SKIP LOCKED;",
                  parameters: new { BatchSize = batchSize },
                  transaction: tx,
                  cancellationToken: ct
               )
            )).ToList();

            if (rows.Count == 0)
            {
                await tx.CommitAsync(ct);
                return rows;
            }

            // 标记为 SENDING（同一事务）
            var ids = rows.Select(x => x.Id).ToArray();

            await conn.ExecuteAsync(new CommandDefinition(@"
UPDATE _events
SET status='SENDING',
    updated_at=CURRENT_TIMESTAMP(6),
    version=version+1
WHERE id IN @Ids AND status='NEW';",
                new { Ids = ids },
                transaction: tx,
                cancellationToken: ct
            ));

            await tx.CommitAsync(ct);
            return rows;
        }

        /// <summary>
        /// Marks the specified events as sent by updating their status in the data store asynchronously.
        /// </summary>
        /// <remarks>Only events with a current status of 'SENDING' will be updated. If the array is
        /// empty, no operation is performed.</remarks>
        /// <param name="ids">An array of event identifiers to be marked as sent. The array must not be empty.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task MarkSentAsync(long[] ids, CancellationToken ct)
        {
            if (ids.Length == 0) return;

            await using var conn = _factory.Create();
            await conn.OpenAsync(ct);

            await conn.ExecuteAsync(new CommandDefinition(@"
UPDATE _events
SET status='SENT',
    sent_at=CURRENT_TIMESTAMP(6),
    updated_at=CURRENT_TIMESTAMP(6),
    version=version+1
WHERE id IN @Ids AND status='SENDING';",
                new { Ids = ids },
                cancellationToken: ct
            ));
        }

        /// <summary>
        /// Marks the specified events as failed if their retry count reaches the maximum allowed, or increments their
        /// retry count otherwise.
        /// </summary>
        /// <remarks>Only events with a status of 'SENDING' are affected. If the retry count for an event
        /// reaches or exceeds <paramref name="maxRetry"/>, its status is set to 'FAILED'; otherwise, the retry count is
        /// incremented and the status is reset to 'NEW'.</remarks>
        /// <param name="ids">An array of event identifiers to update. The array must not be empty.</param>
        /// <param name="maxRetry">The maximum number of retry attempts allowed before an event is marked as failed. Must be greater than zero.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task MarkFailedAsync(long[] ids, int maxRetry, CancellationToken ct)
        {
            if (ids.Length == 0) return;

            await using var conn = _factory.Create();
            await conn.OpenAsync(ct);

            await conn.ExecuteAsync(new CommandDefinition(@"
UPDATE _events
SET retry_count = retry_count + 1,
    status = CASE
        WHEN retry_count + 1 >= @MaxRetry THEN 'FAILED'
        ELSE 'NEW'
    END,
    updated_at=CURRENT_TIMESTAMP(6),
    version=version+1
WHERE id IN @Ids AND status='SENDING';",
                new { Ids = ids, MaxRetry = maxRetry },
                cancellationToken: ct
            ));
        }
    }
}
