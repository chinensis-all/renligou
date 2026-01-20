using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries;

public sealed record GetDepartmentListQuery(long? CompanyId, long? ParentId, string? DeptName) : IQuery<Result<List<DepartmentListDto>>>;
