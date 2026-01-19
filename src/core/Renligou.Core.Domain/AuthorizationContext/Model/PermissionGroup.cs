using Renligou.Core.Domain.AuthorizationContext.Event;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Model
{
    public class PermissionGroup : AggregateBase
    {
        public string GroupName { get; private set; }

        public string DisplayName { get; private set; }

        public string Description { get; private set; }

        public long DeletedAt { get; private set; }

        public PermissionGroup(
            AggregateId id,
            string groupName,
            string displayName,
            string description,
            long deletedAt = 0)
        {
            Id = id;
            GroupName = groupName;
            DisplayName = displayName;
            Description = description;
            DeletedAt = deletedAt;
        }

        public void Create()
        {
            var @event = new PermissionGroupCreatedEvent
            {
                OccurredAt = DateTimeOffset.UtcNow,
                Id = Id.id,
                GroupName = GroupName,
                DisplayName = DisplayName,
                Description = Description
            };
            RegisterEvent(@event);
        }

        public void Modify(string groupName, string displayName, string description)
        {
            GroupName = groupName;
            DisplayName = displayName;
            Description = description;

            var @event = new PermissionGroupModifiedEvent
            {
                OccurredAt = DateTimeOffset.UtcNow,
                Id = Id.id,
                GroupName = GroupName,
                DisplayName = DisplayName,
                Description = Description
            };
            RegisterEvent(@event);
        }

        public void Delete()
        {
            DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var @event = new PermissionGroupDeletedEvent
            {
                OccurredAt = DateTimeOffset.UtcNow,
                Id = Id.id
            };
            RegisterEvent(@event);
        }
    }
}
