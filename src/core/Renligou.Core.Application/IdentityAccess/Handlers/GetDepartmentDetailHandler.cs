using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

public class GetDepartmentDetailHandler(IDepartmentQueryRepository _queryRepository) : IQueryHandler<GetDepartmentDetailQuery, Result<DepartmentDetailDto?>>
{
    public async Task<Result<DepartmentDetailDto?>> HandleAsync(GetDepartmentDetailQuery query, CancellationToken cancellationToken)
    {
        var result = await _queryRepository.QueryDetailAsync(query.Id, cancellationToken);
        if (result == null)
            return Result.Fail<DepartmentDetailDto?>("Department.NotFound", "部门不存在");

        return Result.Ok<DepartmentDetailDto?>(result);
    }
}
