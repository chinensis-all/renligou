using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries;

public sealed record GetDepartmentPageQuery(int PageIndex, int PageSize, long? CompanyId, long? ParentId, string? DeptName) : IQuery<Result<Pagination<DepartmentDetailDto>>>;
