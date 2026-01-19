using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public class DestroyPermissionGroupHandler(
        IPermissionGroupRepository _permissionGroupRepository,
        IOutboxRepository _outboxRepository
    ) : ICommandHandler<DestroyPermissionGroupCommand, Result>
    {
        public async Task<Result> HandleAsync(DestroyPermissionGroupCommand command, CancellationToken cancellationToken)
        {
            var permissionGroup = await _permissionGroupRepository.LoadAsync(command.Id);
            if (permissionGroup == null || permissionGroup.DeletedAt > 0)
            {
                 return Result.Fail("PermissionGroup.NotFound", "未找到权限组");
            }

            permissionGroup.Destroy();

            await _permissionGroupRepository.SaveAsync(permissionGroup);
            await _outboxRepository.AddAsync(permissionGroup.GetRegisteredEvents(), "DOMAIN", nameof(PermissionGroup), permissionGroup.Id.id.ToString());

            return Result.Ok();
        }
    }
}
