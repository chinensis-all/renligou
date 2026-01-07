using RabbitMQ.Client;
using Renligou.Core.Infrastructure.Data.Outbox;
using System.Text;

namespace Renligou.Core.Infrastructure.Event
{
    public interface IEventPublisher
    {
        Task PublishAsync(OutboxRow row, CancellationToken ct);
    }
}
