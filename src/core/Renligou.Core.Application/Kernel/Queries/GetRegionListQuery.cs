using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Kernel.Queries
{
    public record GetRegionListQuery(
        long ParentId,
        string? RegionName
    ) : IQuery<List<RegionDto>>{ }
}
