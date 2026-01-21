using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Model;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 创建菜单处理器
/// </summary>
public sealed class CreateMenuHandler(
    IMenuRepository _menuRepository,
    IIdGenerator _idGenerator,
    IOutboxRepository _outboxRepository
) : ICommandHandler<CreateMenuCommand, Result>
{
    public async Task<Result> HandleAsync(CreateMenuCommand command, CancellationToken cancellationToken)
    {
        var validation = command.Validate();
        if (!validation.Success) return validation;

        // 检查父级是否存在
        if (command.ParentId != 0)
        {
            if (!await _menuRepository.ExistsAsync(command.ParentId, cancellationToken))
            {
                return Result.Fail("Menu.Create.Error", $"父级菜单不存在: {command.ParentId}");
            }
        }

        // 检查名称和标识冲突
        if (await _menuRepository.IsNameTagConflictAsync(command.MenuName, command.MenuTag, null, cancellationToken))
        {
            return Result.Fail("Menu.Create.Error", "菜单名称与标识联合已存在");
        }

        var id = _idGenerator.NextId();
        var menu = new Menu(
            new AggregateId(id, true),
            command.ParentId,
            command.MenuName,
            command.MenuTag,
            command.Path,
            command.Component,
            command.Icon,
            command.Sorter,
            command.IsHidden,
            command.PermitButtons
        );

        menu.Create();

        await _menuRepository.SaveAsync(menu);
        await _outboxRepository.AddAsync(menu.GetRegisteredEvents(), "DOMAIN", menu.GetType().Name, menu.Id.id.ToString());

        return Result.Ok();
    }
}
