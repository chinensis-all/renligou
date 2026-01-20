using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

public class GetDepartmentPageHandler(IDepartmentQueryRepository _queryRepository) : IQueryHandler<GetDepartmentPageQuery, Result<Pagination<DepartmentDetailDto>>>
{
    public async Task<Result<Pagination<DepartmentDetailDto>>> HandleAsync(GetDepartmentPageQuery query, CancellationToken cancellationToken)
    {
        var result = await _queryRepository.PaginateAsync(query.PageIndex, query.PageSize, query.CompanyId, query.ParentId, query.DeptName, cancellationToken);
        return Result.Ok(result);
    }
}
