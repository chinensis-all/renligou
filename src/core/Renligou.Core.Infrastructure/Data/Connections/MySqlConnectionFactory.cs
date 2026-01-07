using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Data.Common;

namespace Renligou.Core.Infrastructure.Data.Connections
{
    public sealed class MySqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connStr;

        public MySqlConnectionFactory(IConfiguration cfg)
            => _connStr = cfg.GetConnectionString("MySql")!;

        public DbConnection Create() => new MySqlConnection(_connStr);
    }
}
