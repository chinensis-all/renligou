using System;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.AuthorizationContext.Event
{
    /// <summary>
    /// 权限创建事件
    /// </summary>
    public sealed record PermissionCreatedEvent : IIntegrationEvent
    {
        public DateTimeOffset OccurredAt { get; init; }

        public long Id { get; init; }

        public long GroupId { get; init; }

        public string PermissionName { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;
    }

    /// <summary>
    /// 权限修改事件
    /// </summary>
    public sealed record PermissionModifiedEvent : IIntegrationEvent
    {
        public DateTimeOffset OccurredAt { get; init; }

        public long Id { get; init; }

        public long GroupId { get; init; }

        public string PermissionName { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;
    }

    /// <summary>
    /// 权限删除事件
    /// </summary>
    public sealed record PermissionDestroyedEvent : IIntegrationEvent
    {
        public DateTimeOffset OccurredAt { get; init; }

        public long Id { get; init; }
    }
}
