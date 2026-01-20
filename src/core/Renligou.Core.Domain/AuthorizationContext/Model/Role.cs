using Renligou.Core.AuthorizationContext.Event;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Model;

/// <summary>
/// 角色聚合根
/// </summary>
public class Role : AggregateBase
{
    /// <summary>
    /// 角色名称 (唯一)
    /// </summary>
    public string RoleName { get; private set; } = string.Empty;

    /// <summary>
    /// 角色显示名称 (唯一)
    /// </summary>
    public string DisplayName { get; private set; } = string.Empty;

    /// <summary>
    /// 私有构造函数，用于仓储加载
    /// </summary>
    private Role() { }

    /// <summary>
    /// 用于创建新角色的构造函数
    /// </summary>
    public Role(AggregateId id, string roleName, string displayName)
    {
        Id = id;
        RoleName = roleName;
        DisplayName = displayName;
    }

    /// <summary>
    /// 创建角色并注册领域事件
    /// </summary>
    public void Create()
    {
        AddDomainEvent(new RoleCreatedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.Id,
            RoleName = RoleName,
            DisplayName = DisplayName
        });
    }

    /// <summary>
    /// 修改角色基础信息
    /// </summary>
    /// <param name="roleName">角色名称</param>
    /// <param name="displayName">显示名称</param>
    public void ModifyBasic(string roleName, string displayName)
    {
        RoleName = roleName;
        DisplayName = displayName;

        AddDomainEvent(new RoleModifiedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.Id,
            RoleName = RoleName,
            DisplayName = DisplayName
        });
    }

    /// <summary>
    /// 软删除角色
    /// </summary>
    public void Destroy()
    {
        AddDomainEvent(new RoleDestroyedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = Id.Id
        });
    }
}
