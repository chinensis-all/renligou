using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Model;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Repo;
using Renligou.Core.Infrastructure.Persistence.Pos;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Infrastructure.Persistence.Repos;

/// <summary>
/// 菜单仓储实现
/// </summary>
public class MenuRepository(DbContext _db) : IMenuRepository, IMenuQueryRepository
{
    private readonly Dictionary<long, MenuPo> _tracked = new();

    // --- IMenuRepository (Domain) ---

    public async Task<Menu?> LoadAsync(long id)
    {
        var po = await _db.Set<MenuPo>()
            .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == 0);

        if (po == null) return null;

        if (!_tracked.ContainsKey(id))
        {
            _tracked.Add(id, po);
        }

        return MapToAggregate(po);
    }

    public async Task SaveAsync(Menu aggregate)
    {
        if (_tracked.TryGetValue(aggregate.Id.id, out var po))
        {
            ApplyAggregateToPo(aggregate, po);
            po.UpdatedAt = DateTimeOffset.UtcNow;
            
            // 处理软删除
            if (aggregate.GetRegisteredEvents().Any(e => e is Renligou.Core.Domain.IdentityAccess.UiAccessContext.Event.MenuDestroyedEvent))
            {
                po.DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            
            _db.Update(po);
        }
        else
        {
            po = new MenuPo
            {
                Id = aggregate.Id.id,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                DeletedAt = 0
            };
            ApplyAggregateToPo(aggregate, po);
            await _db.AddAsync(po);
            _tracked.Add(po.Id, po);
        }
    }

    public async Task<bool> IsNameTagConflictAsync(string menuName, string menuTag, long? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Set<MenuPo>().AsNoTracking()
            .Where(x => x.MenuName == menuName && x.MenuTag == menuTag && x.DeletedAt == 0);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Set<MenuPo>().AsNoTracking()
            .AnyAsync(x => x.Id == id && x.DeletedAt == 0, cancellationToken);
    }

    // --- IMenuQueryRepository (Query) ---

    public async Task<MenuDetailDto?> QueryDetailAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Set<MenuPo>().AsNoTracking()
            .Where(x => x.Id == id && x.DeletedAt == 0)
            .Select(x => new MenuDetailDto
            {
                Id = x.Id,
                ParentId = x.ParentId,
                MenuName = x.MenuName,
                MenuTag = x.MenuTag,
                Path = x.Path,
                Component = x.Component,
                Icon = x.Icon,
                Sorter = x.Sorter,
                IsHidden = x.IsHidden,
                PermitButtons = x.PermitButtons,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<MenuTreeNodeDto>> GetMenuTreeAsync(long parentId = 0, CancellationToken cancellationToken = default)
    {
        // 加载所有非删除菜单以构建树
        var allPos = await _db.Set<MenuPo>().AsNoTracking()
            .Where(x => x.DeletedAt == 0)
            .OrderBy(x => x.Sorter)
            .ToListAsync(cancellationToken);

        var allNodes = allPos.Select(x => new MenuTreeNodeDto
        {
            Id = x.Id,
            ParentId = x.ParentId,
            Name = x.MenuName,
            Tag = x.MenuTag,
            Icon = x.Icon,
            Path = x.Path,
            IsHidden = x.IsHidden
        }).ToList();

        return BuildTree(allNodes, parentId);
    }

    private List<MenuTreeNodeDto> BuildTree(List<MenuTreeNodeDto> nodes, long parentId)
    {
        var result = new List<MenuTreeNodeDto>();
        var children = nodes.Where(x => x.ParentId == parentId).ToList();
        foreach (var child in children)
        {
            var node = child with { Children = BuildTree(nodes, child.Id) };
            result.Add(node);
        }
        return result;
    }

    // --- Helper Methods ---

    private Menu MapToAggregate(MenuPo po)
    {
        var menu = (Menu)Activator.CreateInstance(typeof(Menu), true)!;
        
        typeof(Menu).GetProperty(nameof(Menu.Id))!.SetValue(menu, new AggregateId(po.Id, false));
        typeof(Menu).GetProperty(nameof(Menu.ParentId))!.SetValue(menu, po.ParentId);
        typeof(Menu).GetProperty(nameof(Menu.MenuName))!.SetValue(menu, po.MenuName);
        typeof(Menu).GetProperty(nameof(Menu.MenuTag))!.SetValue(menu, po.MenuTag);
        typeof(Menu).GetProperty(nameof(Menu.Path))!.SetValue(menu, po.Path);
        typeof(Menu).GetProperty(nameof(Menu.Component))!.SetValue(menu, po.Component);
        typeof(Menu).GetProperty(nameof(Menu.Icon))!.SetValue(menu, po.Icon);
        typeof(Menu).GetProperty(nameof(Menu.Sorter))!.SetValue(menu, po.Sorter);
        typeof(Menu).GetProperty(nameof(Menu.IsHidden))!.SetValue(menu, po.IsHidden);
        typeof(Menu).GetProperty(nameof(Menu.PermitButtons))!.SetValue(menu, po.PermitButtons);

        return menu;
    }

    private static void ApplyAggregateToPo(Menu aggregate, MenuPo po)
    {
        po.ParentId = aggregate.ParentId;
        po.MenuName = aggregate.MenuName;
        po.MenuTag = aggregate.MenuTag;
        po.Path = aggregate.Path;
        po.Component = aggregate.Component;
        po.Icon = aggregate.Icon;
        po.Sorter = aggregate.Sorter;
        po.IsHidden = aggregate.IsHidden;
        po.PermitButtons = aggregate.PermitButtons;
    }
}
