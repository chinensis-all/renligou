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

            if (await _permissionGroupRepository.IsGroupNameConflictAsync(command.Id, command.GroupName))
            {
                return Result.Fail("PermissionGroup.Modify.Error", "权限组名称已存在");
            }

            if (await _permissionGroupRepository.IsDisplayNameConflictAsync(command.Id, command.DisplayName))
            {
                return Result.Fail("PermissionGroup.Modify.Error", "权限组显示名称已存在");
            }

            permissionGroup.Modify(command.GroupName, command.DisplayName, command.Description);

            await _permissionGroupRepository.SaveAsync(permissionGroup);
             await _outboxRepository.AddAsync(permissionGroup.GetRegisteredEvents(), "DOMAIN", nameof(PermissionGroup), permissionGroup.Id.id.ToString());

            return Result.Ok();
        }
    }
}
