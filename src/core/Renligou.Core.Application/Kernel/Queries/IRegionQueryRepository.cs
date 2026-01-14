using Renligou.Core.Shared.Common;

namespace Renligou.Core.Application.Kernel.Queries
{
    public interface IRegionQueryRepository : IRepository
    {
        Task<List<RegionDto>> QeuryResionListAsync(long parentId, string? regionName);

        Task<Dictionary<long, string>> QueryRegionNamesByIdsAsync(IEnumerable<long> regionIds);
    }
}
