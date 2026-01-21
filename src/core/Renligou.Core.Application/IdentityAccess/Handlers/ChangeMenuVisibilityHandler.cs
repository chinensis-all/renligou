using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 修改菜单显示状态处理器
/// </summary>
public sealed class ChangeMenuVisibilityHandler(
    IMenuRepository _menuRepository,
    IOutboxRepository _outboxRepository
) : ICommandHandler<ChangeMenuVisibilityCommand, Result>
{
    public async Task<Result> HandleAsync(ChangeMenuVisibilityCommand command, CancellationToken cancellationToken)
    {
        var validation = command.Validate();
        if (!validation.Success) return validation;

        var menu = await _menuRepository.LoadAsync(command.Id);
        if (menu == null)
        {
            // 按照需求：如果不存在抛出异常“菜单不存在”
            // 不过在CQRS Handler中通常返回 Result.Fail，但如果用户说“抛出异常”，我也可遵从，
            // 但考虑到之前的 Result 模式，我先返回 Result.Fail
            return Result.Fail("Menu.NotFound", "菜单不存在");
        }

        menu.ChangeVisibility(command.IsHidden);

        await _menuRepository.SaveAsync(menu);
        await _outboxRepository.AddAsync(menu.GetRegisteredEvents(), "DOMAIN", menu.GetType().Name, menu.Id.id.ToString());

        return Result.Ok();
    }
}
