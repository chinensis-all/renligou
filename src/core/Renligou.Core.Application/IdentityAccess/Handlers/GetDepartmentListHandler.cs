using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

public class GetDepartmentListHandler(IDepartmentQueryRepository _queryRepository) : IQueryHandler<GetDepartmentListQuery, Result<List<DepartmentListDto>>>
{
    public async Task<Result<List<DepartmentListDto>>> HandleAsync(GetDepartmentListQuery query, CancellationToken cancellationToken)
    {
        var result = await _queryRepository.SearchAsync(query.CompanyId, query.ParentId, query.DeptName, cancellationToken);
        return Result.Ok(result);
    }
}
