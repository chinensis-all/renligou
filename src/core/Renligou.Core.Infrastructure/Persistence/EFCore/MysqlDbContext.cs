using Microsoft.EntityFrameworkCore;

namespace Renligou.Core.Infrastructure.Persistence.EFCore
{
    public class MysqlDbContext : DbContext
    {
        public MysqlDbContext(DbContextOptions<MysqlDbContext> options) : base(options)
        {
        }
    }
}
