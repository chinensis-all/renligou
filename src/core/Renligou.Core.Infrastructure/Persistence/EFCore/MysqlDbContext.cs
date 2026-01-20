using Microsoft.EntityFrameworkCore;
using Renligou.Core.Infrastructure.Persistence.Pos;

namespace Renligou.Core.Infrastructure.Persistence.EFCore
{
    public class MysqlDbContext : DbContext
    {
        public MysqlDbContext(DbContextOptions<MysqlDbContext> options) : base(options)
        {
        }

        public DbSet<RegionPo> Regions { get; set; } = null!;                                       // 中国行政区划表
        public DbSet<OutboxPo> Outboxes { get; set; } = null!;                                      // 可靠事件表
        public DbSet<CompanyPo> Companies { get; set; } = null!;                                    // 企业信息表
        public DbSet<PermissionGroupPo> PermissionGroups { get; set; } = null!;                     // 权限组表
        public DbSet<PermissionPo> Permissions { get; set; } = null!;                               // 权限表
    }
}
