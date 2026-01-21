using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 获取部门详情处理器
/// </summary>
public sealed class GetDepartmentDetailHandler(
    IDepartmentQueryRepository _departmentQueryRepository
) : IQueryHandler<GetDepartmentDetailQuery, Result<DepartmentDetailDto?>>
{
    public async Task<Result<DepartmentDetailDto?>> HandleAsync(GetDepartmentDetailQuery query, CancellationToken cancellationToken)
    {
        var dto = await _departmentQueryRepository.QueryDetailAsync(query.Id, cancellationToken);
        if (dto == null)
        {
            return Result<DepartmentDetailDto?>.Fail("Department.NotFound", $"没有找到部门: {query.Id}");
        }

        return Result<DepartmentDetailDto?>.Ok(dto);
    }
}
