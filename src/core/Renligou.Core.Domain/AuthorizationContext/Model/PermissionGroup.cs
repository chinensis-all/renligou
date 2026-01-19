using Renligou.Core.Domain.AuthorizationContext.Event;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.AuthorizationContext.Model
{
    public class PermissionGroup : AggregateBase
    {
        public string GroupName { get; private set; }

        public string DisplayName { get; private set; }

        public string Description { get; private set; }

        public long ParentId { get; private set; }

        public int Sorter { get; private set; }

        public long DeletedAt { get; private set; }

        public PermissionGroup(
            AggregateId id,
            string groupName,
            string displayName,
            string description,
            long parentId,
            int sorter,
            long deletedAt = 0)
        {
            Id = id;
            GroupName = groupName;
            DisplayName = displayName;
            Description = description;
            ParentId = parentId;
            Sorter = sorter;
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
                Description = Description,
                ParentId = ParentId,
                Sorter = Sorter
            };
            RegisterEvent(@event);
        }

        public void Modify(string groupName, string displayName, string description, long parentId, int sorter)
        {
            GroupName = groupName;
            DisplayName = displayName;
            Description = description;
            ParentId = parentId;
            Sorter = sorter;

            var @event = new PermissionGroupModifiedEvent
            {
                OccurredAt = DateTimeOffset.UtcNow,
                Id = Id.id,
                GroupName = GroupName,
                DisplayName = DisplayName,
                Description = Description,
                ParentId = ParentId,
                Sorter = Sorter
            };
            RegisterEvent(@event);
        }

        public void Destroy()
        {
            DeletedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var @event = new PermissionGroupDestroyedEvent
            {
                OccurredAt = DateTimeOffset.UtcNow,
                Id = Id.id
            };
            RegisterEvent(@event);
        }
    }
}
