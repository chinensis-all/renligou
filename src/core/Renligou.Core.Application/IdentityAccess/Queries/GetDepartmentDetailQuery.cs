using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.IdentityAccess.Queries;

public sealed record GetDepartmentDetailQuery(long Id) : IQuery<Result<DepartmentDetailDto?>>;
