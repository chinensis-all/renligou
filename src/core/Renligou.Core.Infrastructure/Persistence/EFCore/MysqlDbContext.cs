using Microsoft.EntityFrameworkCore;
using Renligou.Core.Infrastructure.Persistence.Pos;

namespace Renligou.Core.Infrastructure.Persistence.EFCore
{
    public class MysqlDbContext : DbContext
    {
        public MysqlDbContext(DbContextOptions<MysqlDbContext> options) : base(options)
        {
        }

        public DbSet<RegionPo> regions { get; set; } // 中国行政区划表
    }
}
