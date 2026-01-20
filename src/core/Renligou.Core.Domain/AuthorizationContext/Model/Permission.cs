using System;
using Renligou.Core.Domain.AuthorizationContext.Event;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Model
{
    /// <summary>
    /// 权限聚合根
    /// </summary>
    public class Permission : AggregateBase
    {
        /// <summary>
        /// 权限组ID
        /// </summary>
        public long GroupId { get; private set; }

        /// <summary>
        /// 权限标识
        /// </summary>
        public string PermissionName { get; private set; }

        /// <summary>
        /// 权限名称
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// 权限描述
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// 删除时间(逻辑删除)
        /// </summary>
        public long DeletedAt { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Permission(
            AggregateId id,
            long groupId,
            string permissionName,
            string displayName,
            string description,
            long deletedAt = 0)
        {
            Id = id;
            GroupId = groupId;
            PermissionName = permissionName;
            DisplayName = displayName;
            Description = description;
            DeletedAt = deletedAt;
        }

        /// <summary>
        /// 创建权限
        /// </summary>
        public void Create()
        {
            RegisterEvent(new PermissionCreatedEvent
            {
                OccurredAt = DateTimeOffset.UtcNow,
                Id = Id.id,
                GroupId = GroupId,
                PermissionName = PermissionName,
                DisplayName = DisplayName,
                Description = Description
            });
        }

        /// <summary>
        /// 修改权限
        /// </summary>
        public void Modify(long groupId, string permissionName, string displayName, string description)
        {
            GroupId = groupId;
            PermissionName = permissionName;
            DisplayName = displayName;
            Description = description;

            RegisterEvent(new PermissionModifiedEvent
            {
                OccurredAt = DateTimeOffset.UtcNow,
                Id = Id.id,
                GroupId = GroupId,
                PermissionName = PermissionName,
                DisplayName = DisplayName,
                Description = Description
            });
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        public void Destroy()
        {
            DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            RegisterEvent(new PermissionDestroyedEvent
            {
                OccurredAt = DateTimeOffset.UtcNow,
                Id = Id.id
            });
        }
    }
}
