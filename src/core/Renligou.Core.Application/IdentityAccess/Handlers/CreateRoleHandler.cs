using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 创建角色处理器
/// </summary>
public class CreateRoleHandler(
    IRoleRepository _roleRepository,
    IOutboxRepository _outboxRepository,
    IIdGenerator _idGenerator
) : ICommandHandler<CreateRoleCommand, Result>
{
    public async Task<Result> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        // 1. 基础验证
        var validation = command.Validate();
        if (!validation.Success) return validation;

        // 2. 检查唯一性冲突
        if (await _roleRepository.IsRoleNameConflictAsync(0, command.RoleName))
        {
            return Result.Fail("Role.Create.Error", "角色名称已存在");
        }

        if (await _roleRepository.IsDisplayNameConflictAsync(0, command.DisplayName))
        {
            return Result.Fail("Role.Create.Error", "显示名称已存在");
        }

        // 3. 生成 ID 并构造聚合根
        var id = _idGenerator.NextId();
        var role = new Role(new AggregateId(id, true), command.RoleName, command.DisplayName, command.Description);

        // 4. 执行创建逻辑 (内部注册领域事件)
        role.Create();

        // 5. 持久化
        await _roleRepository.SaveAsync(role);

        // 6. 保存到 Outbox
        await _outboxRepository.AddAsync(role.GetRegisteredEvents(), "DOMAIN", role.GetType().Name, role.Id.id.ToString());

        return Result.Ok();
    }
}
