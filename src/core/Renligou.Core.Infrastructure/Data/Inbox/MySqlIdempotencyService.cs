using Dapper;
using Renligou.Core.Infrastructure.Data.Connections;

namespace Renligou.Core.Infrastructure.Data.Inbox
{
    public class MySqlIdempotencyService : IIdempotencyService
    {
        private readonly IDbConnectionFactory _factory;
        private const string ConsumerName = "hrm.employee.consumer";

        public MySqlIdempotencyService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<bool> AlreadyProcessedAsync(string messageId, CancellationToken ct)
        {
            await using var conn = _factory.Create();
            await conn.OpenAsync(ct);

            var exists = await conn.ExecuteScalarAsync<int>(new CommandDefinition(@"
SELECT EXISTS(
  SELECT 1 FROM processed_messages
  WHERE consumer_name=@Consumer AND message_id=@MsgId
);",
                new { Consumer = ConsumerName, MsgId = messageId },
                cancellationToken: ct));

            return exists == 1;
        }

        public async Task MarkProcessedAsync(string messageId, CancellationToken ct)
        {
            await using var conn = _factory.Create();
            await conn.OpenAsync(ct);

            // 利用唯一键：重复插入不报错
            await conn.ExecuteAsync(new CommandDefinition(@"
INSERT INTO processed_messages(consumer_name, message_id)
VALUES(@Consumer, @MsgId)
ON DUPLICATE KEY UPDATE processed_at = processed_at;",
                new { Consumer = ConsumerName, MsgId = messageId },
                cancellationToken: ct));
        }
    }
}
