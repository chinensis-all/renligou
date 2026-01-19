using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.AuthorizationContext.Event
{
    public sealed record PermissionGroupDeletedEvent : IIntegrationEvent
    {
        public DateTimeOffset OccurredAt { get; init; }

        public long Id { get; init; }
    }
}
