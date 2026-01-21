using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Event;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.IdentityAccess.UiAccessContext.Model;

/// <summary>
/// Boss 菜单聚合根
/// </summary>
public class Menu : AggregateBase
{
    /// <summary>
    /// 父级菜单ID
    /// </summary>
    public long ParentId { get; private set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string MenuName { get; private set; } = string.Empty;

    /// <summary>
    /// 菜单标识 (用于同名区分)
    /// </summary>
    public string MenuTag { get; private set; } = string.Empty;

    /// <summary>
    /// 菜单路径
    /// </summary>
    public string Path { get; private set; } = string.Empty;

    /// <summary>
    /// 前端组件路径
    /// </summary>
    public string Component { get; private set; } = string.Empty;

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string Icon { get; private set; } = string.Empty;

    /// <summary>
    /// 菜单排序号
    /// </summary>
    public int Sorter { get; private set; }

    /// <summary>
    /// 是否隐藏
    /// </summary>
    public bool IsHidden { get; private set; }

    /// <summary>
    /// 允许的按钮权限
    /// </summary>
    public string PermitButtons { get; private set; } = string.Empty;

    /// <summary>
    /// 私有构造函数，用于仓储加载
    /// </summary>
    private Menu() { }

    /// <summary>
    /// 用于创建新菜单的构造函数
    /// </summary>
    public Menu(
        AggregateId id,
        long parentId,
        string menuName,
        string menuTag,
        string path,
        string component,
        string icon,
        int sorter,
        bool isHidden,
        string permitButtons)
    {
        Id = id;
        ParentId = parentId;
        MenuName = menuName;
        MenuTag = menuTag;
        Path = path;
        Component = component;
        Icon = icon;
        Sorter = sorter;
        IsHidden = isHidden;
        PermitButtons = permitButtons;
    }

    /// <summary>
    /// 创建菜单并注册领域事件
    /// </summary>
    public void Create()
    {
        RegisterEvent(new MenuCreatedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.id,
            ParentId = ParentId,
            MenuName = MenuName,
            MenuTag = MenuTag,
            Path = Path,
            Component = Component,
            Icon = Icon,
            Sorter = Sorter,
            IsHidden = IsHidden,
            PermitButtons = PermitButtons
        });
    }

    /// <summary>
    /// 修改菜单基本信息
    /// </summary>
    public void Modify(
        long parentId,
        string menuName,
        string menuTag,
        string path,
        string component,
        string icon,
        int sorter,
        string permitButtons)
    {
        ParentId = parentId;
        MenuName = menuName;
        MenuTag = menuTag;
        Path = path;
        Component = component;
        Icon = icon;
        Sorter = sorter;
        PermitButtons = permitButtons;

        RegisterEvent(new MenuModifiedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.id,
            ParentId = ParentId,
            MenuName = MenuName,
            MenuTag = MenuTag,
            Path = Path,
            Component = Component,
            Icon = Icon,
            Sorter = Sorter,
            PermitButtons = PermitButtons
        });
    }

    /// <summary>
    /// 更改显示状态
    /// </summary>
    /// <param name="isHidden">是否隐藏</param>
    public void ChangeVisibility(bool isHidden)
    {
        if (IsHidden == isHidden) return;

        IsHidden = isHidden;

        RegisterEvent(new MenuVisibilityChangedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.id,
            IsHidden = IsHidden
        });
    }

    /// <summary>
    /// 销毁（软删除）菜单
    /// </summary>
    public void Destroy()
    {
        RegisterEvent(new MenuDestroyedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.id
        });
    }
}
