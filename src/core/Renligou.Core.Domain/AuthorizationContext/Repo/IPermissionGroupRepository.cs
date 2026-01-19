using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.CommonContext.Repo;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Repo
{
    public interface IPermissionGroupRepository : IRepository, DomainRepository<PermissionGroup>
    {
        Task<bool> IsGroupNameConflictAsync(long id, string groupName);

        Task<bool> IsDisplayNameConflictAsync(long id, string displayName);
    }
}
