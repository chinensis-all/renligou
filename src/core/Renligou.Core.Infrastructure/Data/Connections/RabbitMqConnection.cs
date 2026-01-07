using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace Renligou.Core.Infrastructure.Data.Connections
{
    public class RabbitMqConnection : IAsyncDisposable
    {
        private readonly IConnection _connection;

        public RabbitMqConnection(IConfiguration cfg)
        {
            var factory = new ConnectionFactory
            {
                HostName = cfg["RabbitMQ:Host"] ?? "localhost",
                UserName = cfg["RabbitMQ:User"] ?? "guest",
                Password = cfg["RabbitMQ:Password"] ?? "guest",
                VirtualHost = cfg["RabbitMQ:VirtualHost"] ?? "/"
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }

        public async Task<IChannel> CreateChannelAsync()
        {
            return await _connection.CreateChannelAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
    }
}
