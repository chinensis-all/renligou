using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using Renligou.Core.Infrastructure.Data.Connections;
using Renligou.Core.Infrastructure.Data.Outbox;
using System.Text;

namespace Renligou.Core.Infrastructure.Event
{
    public class RabbitMqEventPublisher : IEventPublisher
    {
        private readonly RabbitMqConnection _conn;

        private readonly string _exchange;

        public RabbitMqEventPublisher(RabbitMqConnection conn, IConfiguration cfg)
        {
            _conn = conn;
            _exchange = cfg["RabbitMQ:Exchange"] ?? "renligou.events";
        }

        public async Task PublishAsync(OutboxRow row, CancellationToken ct)
        {
            await using var ch = await _conn.CreateChannelAsync();

            await ch.ExchangeDeclareAsync(
                exchange: _exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: ct
            );

            var props = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent,
                ContentType = "application/json",
                Type = row.EventType,
                MessageId = row.Id.ToString()
            };

#pragma warning disable CS8619 
            props.Headers = new Dictionary<string, object>
            {
                ["x-event-id"] = Encoding.UTF8.GetBytes(row.Id.ToString()),
                ["x-source-type"] = Encoding.UTF8.GetBytes(row.SourceType),
                ["x-source-id"] = Encoding.UTF8.GetBytes(row.SourceId)
            };
#pragma warning restore CS8619 

            var body = Encoding.UTF8.GetBytes(row.Payload);
            var routingKey = row.EventType;

            await ch.BasicPublishAsync(
                exchange: _exchange,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: ct
            );
        }
    }
}
