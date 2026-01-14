using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Infrastructure.Persistence.Pos;
using Renligou.Core.Shared.Events;
using System.Text.Json;

namespace Renligou.Core.Infrastructure.Persistence.Repos
{
    public class OutboxRepository(
        DbContext _context,
        ILogger<OutboxRepository> _logger
    ) : IOutboxRepository
    {

        public async Task AddAsync(IIntegrationEvent @event, String category, String aggregateType, String aggregateId)
        {
            OutboxPo po = new OutboxPo();
            po.Category = category;
            po.SourceType = aggregateType;
            po.SourceId = aggregateId;
            po.EventType = @event.GetType().FullName ?? string.Empty;
            po.Payload = JsonSerializer.Serialize(@event, @event.GetType());
            po.Status = "NEW";
            po.OccurredAt = (@event as IIntegrationEvent)?.OccurredAt ?? default;
            po.CreatedAt = DateTimeOffset.UtcNow;
            po.UpdatedAt = DateTimeOffset.UtcNow;

            _context.Set<OutboxPo>().Add(po);
            _context.Entry(po).State = EntityState.Added;
        }

        public async Task AddAsync(IEnumerable<IIntegrationEvent> @events, String category, String aggregateType, String aggregateId)
        {
            foreach (var @event in @events)
            {
                await this.AddAsync(@event, category, aggregateType, aggregateId);
            }
        }
    }
}
