using System.Data.Common;

namespace Renligou.Core.Infrastructure.Data.Connections
{
    public interface IDbConnectionFactory
    {
        DbConnection Create();
    }
}
