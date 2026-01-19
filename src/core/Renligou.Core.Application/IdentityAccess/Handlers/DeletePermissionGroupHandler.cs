using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public class DeletePermissionGroupHandler(
        IPermissionGroupRepository _permissionGroupRepository,
        IOutboxRepository _outboxRepository
    ) : ICommandHandler<DeletePermissionGroupCommand, Result>
    {
        public async Task<Result> HandleAsync(DeletePermissionGroupCommand command, CancellationToken cancellationToken)
        {
            var permissionGroup = await _permissionGroupRepository.LoadAsync(command.Id);
            if (permissionGroup == null || permissionGroup.DeletedAt > 0)
            {
                 // Usually idempotent delete returns success even if not found, but here we might want to inform.
                 // Assuming idempotency is fine, but if business logic requires strictly existing, then return Fail.
                 // For now, let's return Fail if not found to be explicit, or just return Ok if we treat it as "ensure deleted".
                 // The skill example says "Load -> Check Null -> Call Destroy".
                 // If already deleted, we might skip.
                 return Result.Fail("PermissionGroup.NotFound", "未找到权限组");
            }

            permissionGroup.Delete();

            await _permissionGroupRepository.SaveAsync(permissionGroup);
            await _outboxRepository.AddAsync(permissionGroup.GetRegisteredEvents(), "DOMAIN", nameof(PermissionGroup), permissionGroup.Id.id.ToString());

            return Result.Ok();
        }
    }
}
