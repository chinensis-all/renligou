using Renligou.Core.Application.Kernel.Handlers;
using Renligou.Core.Application.Kernel.Queries;

namespace Renligou.Core.Application.Kernel
{
    public class RegionFacade
    {
        private readonly GetRegionListHandler getRegionListHandler;

        public RegionFacade(GetRegionListHandler getRegionListHandler)
        {
            this.getRegionListHandler = getRegionListHandler;
        }

        public Task<List<RegionDto>> GetRegionListAsync(long parentId, string? regionName, CancellationToken cancellationToken = default)
        {
            var query = new GetRegionListQuery(parentId, regionName);
            return getRegionListHandler.HandleAsync(query, cancellationToken);
        }
    }
}
