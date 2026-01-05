using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Kernel.Queries
{
    public class GetRegionListHandler : IQueryHandler<GetRegionListQuery, List<RegionDto>>
    {
        private readonly IRegionQueryRepository _repository;

        public GetRegionListHandler(IRegionQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<RegionDto>> HandleAsync(GetRegionListQuery query, CancellationToken cancellationToken)
        {
            return await _repository.QeuryResionListAsync(query.ParentId, query.RegionName);
        }
    }
}
