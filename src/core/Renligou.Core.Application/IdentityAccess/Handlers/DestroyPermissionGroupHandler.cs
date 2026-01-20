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
        IPermissionRepository _permissionRepository,
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

            // 检查该权限组或其子权限组下是否存在权限
            if (await _permissionRepository.HasPermissionsAsync(command.Id, cancellationToken))
            {
                return Result.Fail("PermissionGroup.Delete.Error", "该权限组或其子权限组下存在权限，不允许删除");
            }

            permissionGroup.Destroy();

            await _permissionGroupRepository.SaveAsync(permissionGroup);
            await _outboxRepository.AddAsync(permissionGroup.GetRegisteredEvents(), "DOMAIN", nameof(PermissionGroup), permissionGroup.Id.id.ToString());

            return Result.Ok();
        }
    }
}
