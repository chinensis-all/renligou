using NUnit.Framework;
using Renligou.Core.Domain.AuthorizationContext.Event;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.Tests.AuthorizationContext
{
    [TestFixture]
    public class PermissionGroupTests
    {
        [Test]
        public void Create_ShouldRegisterPermissionGroupCreatedEvent()
        {
            // Arrange
            var id = new AggregateId(1, true);
            var parentId = 100L;
            var sorter = 10;
            var group = new PermissionGroup(id, "Admin", "管理员", "系统管理员", parentId, sorter);

            // Act
            group.Create();

            // Assert
            var events = group.GetRegisteredEvents();
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<PermissionGroupCreatedEvent>());
            
            var @event = (PermissionGroupCreatedEvent)events[0];
            Assert.That(@event.GroupName, Is.EqualTo("Admin"));
            Assert.That(@event.Id, Is.EqualTo(1));
            Assert.That(@event.ParentId, Is.EqualTo(parentId));
            Assert.That(@event.Sorter, Is.EqualTo(sorter));
        }

        [Test]
        public void Modify_ShouldRegisterPermissionGroupModifiedEvent()
        {
            // Arrange
            var id = new AggregateId(1, false);
            var parentId = 100L;
            var sorter = 10;
            var group = new PermissionGroup(id, "Admin", "管理员", "系统管理员", parentId, sorter);
            group.ClearRegisteredEvents();

            var newParentId = 200L;
            var newSorter = 20;

            // Act
            group.Modify("SuperAdmin", "超级管理员", "超级系统管理员", newParentId, newSorter);

            // Assert
            Assert.That(group.GroupName, Is.EqualTo("SuperAdmin"));
            Assert.That(group.DisplayName, Is.EqualTo("超级管理员"));
            Assert.That(group.Description, Is.EqualTo("超级系统管理员"));
            Assert.That(group.ParentId, Is.EqualTo(newParentId));
            Assert.That(group.Sorter, Is.EqualTo(newSorter));

            var events = group.GetRegisteredEvents();
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<PermissionGroupModifiedEvent>());
            var @event = (PermissionGroupModifiedEvent)events[0];
            Assert.That(@event.ParentId, Is.EqualTo(newParentId));
            Assert.That(@event.Sorter, Is.EqualTo(newSorter));
        }

        [Test]
        public void Destroy_ShouldRegisterPermissionGroupDestroyedEvent_AndSetDeletedAt()
        {
             // Arrange
            var id = new AggregateId(1, false);
            var group = new PermissionGroup(id, "Admin", "管理员", "系统管理员", 0, 0);
            group.ClearRegisteredEvents();

            // Act
            group.Destroy();

            // Assert
            Assert.That(group.DeletedAt, Is.GreaterThan(0));

            var events = group.GetRegisteredEvents();
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<PermissionGroupDestroyedEvent>());
        }
    }
}
