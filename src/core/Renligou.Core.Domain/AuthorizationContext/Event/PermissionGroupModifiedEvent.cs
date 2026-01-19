using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.AuthorizationContext.Event
{
    public sealed record PermissionGroupModifiedEvent : IIntegrationEvent
    {
        public DateTimeOffset OccurredAt { get; init; }

        public long Id { get; init; }

        public string GroupName { get; init; } = string.Empty;

        public string DisplayName { get; init; } = string.Empty;

        public string Description { get; init; } = string.Empty;

        public long ParentId { get; init; }

        public int Sorter { get; init; }
    }
}
