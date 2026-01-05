using Microsoft.EntityFrameworkCore;
using Renligou.Core.Application.Kernel.Queries;
using Renligou.Core.Infrastructure.Persistence.EFCore;
using Renligou.Core.Infrastructure.Persistence.Pos;

namespace Renligou.Core.Infrastructure.Persistence.Repos
{
    public class RegionRepository : IRegionQueryRepository
    {
        private readonly MysqlDbContext _context;
        public RegionRepository(MysqlDbContext context)
        {
            _context = context;
        }

        public async Task<List<RegionDto>> QeuryResionListAsync(long parentId, string? regionName)
        {
            List<RegionDto> regions = _context.Set<RegionPo>()
                .Where(r => r.ParentId == parentId &&
                            (string.IsNullOrEmpty(regionName) || r.RegionName.Contains(regionName) || r.NamePinyin.Contains(regionName)))
                .Select(r => new RegionDto
                {
                    Id = r.Id.ToString(),
                    PostalCode = r.PostalCode,
                    AreaCode = r.AreaCode,
                    RegionName = r.RegionName,
                    MergeName = r.MergeName,
                    Longitude = r.Longitude,

                    Latitude = r.Latitude
                })
                .ToList();

            return await Task.FromResult(regions);
        }
    }
}
