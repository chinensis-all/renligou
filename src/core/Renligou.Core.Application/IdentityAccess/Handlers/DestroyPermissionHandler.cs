using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public class DestroyPermissionHandler(
        IPermissionRepository _permissionRepository,
        IOutboxRepository _outboxRepository
    ) : ICommandHandler<DestroyPermissionCommand, Result>
    {
        public async Task<Result> HandleAsync(DestroyPermissionCommand command, CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.LoadAsync(command.Id);
            if (permission == null || permission.DeletedAt > 0)
            {
                return Result.Fail("Permission.NotFound", "未找到权限");
            }

            permission.Destroy();

            await _permissionRepository.SaveAsync(permission);
            await _outboxRepository.AddAsync(permission.GetRegisteredEvents(), "DOMAIN", nameof(Permission), permission.Id.id.ToString());

            return Result.Ok();
        }
    }
}
