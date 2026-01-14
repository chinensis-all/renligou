using Renligou.Core.Application.Kernel.Queries;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Kernel.Handlers
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
