using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 销毁菜单处理器
/// </summary>
public sealed class DestroyMenuHandler(
    IMenuRepository _menuRepository,
    IOutboxRepository _outboxRepository
) : ICommandHandler<DestroyMenuCommand, Result>
{
    public async Task<Result> HandleAsync(DestroyMenuCommand command, CancellationToken cancellationToken)
    {
        var menu = await _menuRepository.LoadAsync(command.Id);
        if (menu == null)
        {
            return Result.Fail("Menu.NotFound", "菜单不存在");
        }

        menu.Destroy();

        await _menuRepository.SaveAsync(menu);
        await _outboxRepository.AddAsync(menu.GetRegisteredEvents(), "DOMAIN", menu.GetType().Name, menu.Id.id.ToString());

        return Result.Ok();
    }
}
