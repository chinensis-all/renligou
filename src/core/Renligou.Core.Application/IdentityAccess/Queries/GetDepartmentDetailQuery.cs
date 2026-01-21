using Renligou.Core.Shared.Querying;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Queries;

/// <summary>
/// 获取部门详情查询
/// </summary>
public sealed record GetDepartmentDetailQuery(long Id) : IQuery<Result<DepartmentDetailDto?>>;
