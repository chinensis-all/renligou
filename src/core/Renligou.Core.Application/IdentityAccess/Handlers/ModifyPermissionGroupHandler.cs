using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public class ModifyPermissionGroupHandler(
        IPermissionGroupRepository _permissionGroupRepository,
        IOutboxRepository _outboxRepository
    ) : ICommandHandler<ModifyPermissionGroupCommand, Result>
    {
        public async Task<Result> HandleAsync(ModifyPermissionGroupCommand command, CancellationToken cancellationToken)
        {
            var validation = command.Validate();
            if (!validation.Success)
            {
                return validation;
            }

            var permissionGroup = await _permissionGroupRepository.LoadAsync(command.Id);
            if (permissionGroup == null || permissionGroup.DeletedAt > 0)
            {
                return Result.Fail("PermissionGroup.NotFound", "未找到权限组或权限组已删除");
            }

            // 仅在名称变化时验证冲突
            if (permissionGroup.GroupName != command.GroupName)
            {
                if (await _permissionGroupRepository.IsGroupNameConflictAsync(command.Id, command.GroupName))
                {
                    return Result.Fail("PermissionGroup.Modify.Error", "权限组名称已存在");
                }
            }

            // 仅在显示名称变化时验证冲突
            if (permissionGroup.DisplayName != command.DisplayName)
            {
                if (await _permissionGroupRepository.IsDisplayNameConflictAsync(command.Id, command.DisplayName))
                {
                    return Result.Fail("PermissionGroup.Modify.Error", "权限组显示名称已存在");
                }
            }

            // 仅在父权限组变化且大于0时验证是否存在
            if (permissionGroup.ParentId != command.ParentId && command.ParentId > 0)
            {
                if (command.ParentId == command.Id)
                {
                    return Result.Fail("PermissionGroup.Modify.Error", "父权限组不能选择自己");
                }

                var parentGroup = await _permissionGroupRepository.LoadAsync(command.ParentId);
                if (parentGroup == null || parentGroup.DeletedAt > 0)
                {
                    return Result.Fail("PermissionGroup.Modify.Error", "父权限组不存在");
                }
            }

            permissionGroup.Modify(command.GroupName, command.DisplayName, command.Description, command.ParentId, command.Sorter);

            await _permissionGroupRepository.SaveAsync(permissionGroup);
            await _outboxRepository.AddAsync(permissionGroup.GetRegisteredEvents(), "DOMAIN", nameof(PermissionGroup), permissionGroup.Id.id.ToString());

            return Result.Ok();
        }
    }
}
