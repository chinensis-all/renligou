using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 销毁角色处理器
/// </summary>
public class DestroyRoleHandler(
    IRoleRepository _roleRepository,
    IOutboxRepository _outboxRepository
) : ICommandHandler<DestroyRoleCommand, Result>
{
    public async Task<Result> HandleAsync(DestroyRoleCommand command, CancellationToken cancellationToken)
    {
        // 1. 验证
        var validation = command.Validate();
        if (!validation.Success) return validation;

        // 2. 加载
        var role = await _roleRepository.LoadAsync(command.RoleId);
        if (role == null)
        {
            return Result.Fail("Role.NotFound", $"未找到角色: {command.RoleId}");
        }

        // 3. 销毁逻辑
        role.Destroy();

        // 4. 保存
        await _roleRepository.SaveAsync(role);

        // 5. Outbox
        await _outboxRepository.AddAsync(role.GetRegisteredEvents(), "DOMAIN", role.GetType().Name, role.Id.id.ToString());

        return Result.Ok();
    }
}
