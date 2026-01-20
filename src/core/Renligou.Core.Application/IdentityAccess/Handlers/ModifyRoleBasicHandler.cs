using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 修改角色基础信息处理器
/// </summary>
public class ModifyRoleBasicHandler(
    IRoleRepository _roleRepository,
    IOutboxRepository _outboxRepository
) : ICommandHandler<ModifyRoleBasicCommand, Result>
{
    public async Task<Result> HandleAsync(ModifyRoleBasicCommand command, CancellationToken cancellationToken)
    {
        // 1. 基础验证
        var validation = command.Validate();
        if (!validation.Success) return validation;

        // 2. 加载聚合根
        var role = await _roleRepository.LoadAsync(command.RoleId);
        if (role == null)
        {
            return Result.Fail("Role.NotFound", $"未找到角色: {command.RoleId}");
        }

        // 3. 检查冲突
        if (await _roleRepository.IsRoleNameConflictAsync(command.RoleId, command.RoleName))
        {
            return Result.Fail("Role.Modify.Error", "角色名称与其他角色冲突");
        }

        if (await _roleRepository.IsDisplayNameConflictAsync(command.RoleId, command.DisplayName))
        {
            return Result.Fail("Role.Modify.Error", "显示名称与其他角色冲突");
        }

        // 4. 修改领域行为
        role.ModifyBasic(command.RoleName, command.DisplayName, command.Description);

        // 5. 保存
        await _roleRepository.SaveAsync(role);

        // 6. Outbox
        await _outboxRepository.AddAsync(role.GetRegisteredEvents(), "DOMAIN", role.GetType().Name, role.Id.id.ToString());

        return Result.Ok();
    }
}
