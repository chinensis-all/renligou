using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public class ModifyPermissionHandler(
        IPermissionRepository _permissionRepository,
        IPermissionGroupRepository _permissionGroupRepository,
        IOutboxRepository _outboxRepository
    ) : ICommandHandler<ModifyPermissionCommand, Result>
    {
        public async Task<Result> HandleAsync(ModifyPermissionCommand command, CancellationToken cancellationToken)
        {
            var validation = command.Validate();
            if (!validation.Success) return validation;

            var permission = await _permissionRepository.LoadAsync(command.Id);
            if (permission == null || permission.DeletedAt > 0)
            {
                return Result.Fail("Permission.NotFound", "未找到权限");
            }

            // 如果权限组发生了变化，检查新权限组是否存在
            if (permission.GroupId != command.GroupId)
            {
                if (!await _permissionGroupRepository.ExistsAsync(command.GroupId, cancellationToken))
                {
                    return Result.Fail("Permission.Modify.Error", $"权限组 {command.GroupId} 不存在");
                }
            }

            if (await _permissionRepository.IsPermissionNameConflictAsync(command.Id, command.PermissionName))
            {
                return Result.Fail("Permission.Modify.Error", "权限标识已存在");
            }

            if (await _permissionRepository.IsDisplayNameConflictAsync(command.Id, command.DisplayName))
            {
                return Result.Fail("Permission.Modify.Error", "权限名称已存在");
            }

            permission.Modify(command.GroupId, command.PermissionName, command.DisplayName, command.Description);

            await _permissionRepository.SaveAsync(permission);
            await _outboxRepository.AddAsync(permission.GetRegisteredEvents(), "DOMAIN", nameof(Permission), permission.Id.id.ToString());

            return Result.Ok();
        }
    }
}
