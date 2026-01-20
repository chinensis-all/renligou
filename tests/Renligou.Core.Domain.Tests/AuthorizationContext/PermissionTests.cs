using NUnit.Framework;
using Renligou.Core.Domain.AuthorizationContext.Event;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.Tests.AuthorizationContext
{
    [TestFixture]
    public class PermissionTests
    {
        [Test]
        public void Create_ShouldRegisterPermissionCreatedEvent()
        {
            // Arrange
            var id = 123L;
            var groupId = 456L;
            var name = "sys:user:add";
            var displayName = "添加用户";
            var description = "允许添加新用户";

            var permission = new Permission(
                new AggregateId(id, true),
                groupId,
                name,
                displayName,
                description
            );

            // Act
            permission.Create();

            // Assert
            var events = permission.GetRegisteredEvents();
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<PermissionCreatedEvent>());
            
            var @event = (PermissionCreatedEvent)events[0];
            Assert.That(@event.Id, Is.EqualTo(id));
            Assert.That(@event.GroupId, Is.EqualTo(groupId));
            Assert.That(@event.PermissionName, Is.EqualTo(name));
            Assert.That(@event.DisplayName, Is.EqualTo(displayName));
            Assert.That(@event.Description, Is.EqualTo(description));
        }

        [Test]
        public void Modify_ShouldUpdatePropertiesAndRegisterPermissionModifiedEvent()
        {
            // Arrange
            var permission = new Permission(
                new AggregateId(123, false),
                456,
                "old:name",
                "Old Name",
                "Old Desc"
            );

            var newGroupId = 789L;
            var newName = "new:name";
            var newDisplayName = "New Name";
            var newDescription = "New Desc";

            // Act
            permission.Modify(newGroupId, newName, newDisplayName, newDescription);

            // Assert
            Assert.That(permission.GroupId, Is.EqualTo(newGroupId));
            Assert.That(permission.PermissionName, Is.EqualTo(newName));
            Assert.That(permission.DisplayName, Is.EqualTo(newDisplayName));
            Assert.That(permission.Description, Is.EqualTo(newDescription));

            var events = permission.GetRegisteredEvents();
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<PermissionModifiedEvent>());
        }

        [Test]
        public void Destroy_ShouldSetDeletedAtAndRegisterPermissionDestroyedEvent()
        {
            // Arrange
            var permission = new Permission(
                new AggregateId(123, false),
                456,
                "name",
                "Name",
                "Desc"
            );

            // Act
            permission.Destroy();

            // Assert
            Assert.That(permission.DeletedAt, Is.GreaterThan(0));

            var events = permission.GetRegisteredEvents();
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<PermissionDestroyedEvent>());
        }
    }
}
