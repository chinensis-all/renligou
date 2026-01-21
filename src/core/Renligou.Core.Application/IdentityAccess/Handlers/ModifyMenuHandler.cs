using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 修改菜单处理器
/// </summary>
public sealed class ModifyMenuHandler(
    IMenuRepository _menuRepository,
    IOutboxRepository _outboxRepository
) : ICommandHandler<ModifyMenuCommand, Result>
{
    public async Task<Result> HandleAsync(ModifyMenuCommand command, CancellationToken cancellationToken)
    {
        var validation = command.Validate();
        if (!validation.Success) return validation;

        var menu = await _menuRepository.LoadAsync(command.Id);
        if (menu == null)
        {
            return Result.Fail("Menu.NotFound", "菜单不存在");
        }

        // 检查父级是否存在
        if (command.ParentId != 0 && command.ParentId != menu.ParentId)
        {
            if (!await _menuRepository.ExistsAsync(command.ParentId, cancellationToken))
            {
                return Result.Fail("Menu.Modify.Error", $"父级菜单不存在: {command.ParentId}");
            }
        }

        // 检查名称和标识冲突
        if (await _menuRepository.IsNameTagConflictAsync(command.MenuName, command.MenuTag, command.Id, cancellationToken))
        {
            return Result.Fail("Menu.Modify.Error", "菜单名称与标识联合已存在");
        }

        menu.Modify(
            command.ParentId,
            command.MenuName,
            command.MenuTag,
            command.Path,
            command.Component,
            command.Icon,
            command.Sorter,
            command.PermitButtons
        );

        await _menuRepository.SaveAsync(menu);
        await _outboxRepository.AddAsync(menu.GetRegisteredEvents(), "DOMAIN", menu.GetType().Name, menu.Id.id.ToString());

        return Result.Ok();
    }
}
