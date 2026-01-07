using Dapper;
using MySqlConnector;

namespace Renligou.Core.Infrastructure.Data.Inbox
{
    public interface IIdempotencyService
    {
        Task<bool> AlreadyProcessedAsync(string messageId, CancellationToken ct);
        Task MarkProcessedAsync(string messageId, CancellationToken ct);
    }
}
