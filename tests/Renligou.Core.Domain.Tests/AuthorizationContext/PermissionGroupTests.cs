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
            var group = new PermissionGroup(id, "Admin", "管理员", "系统管理员");

            // Act
            group.Create();

            // Assert
            var events = group.GetRegisteredEvents();
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<PermissionGroupCreatedEvent>());
            
            var @event = (PermissionGroupCreatedEvent)events[0];
            Assert.That(@event.GroupName, Is.EqualTo("Admin"));
            Assert.That(@event.Id, Is.EqualTo(1));
        }

        [Test]
        public void Modify_ShouldRegisterPermissionGroupModifiedEvent()
        {
            // Arrange
            var id = new AggregateId(1, false);
            var group = new PermissionGroup(id, "Admin", "管理员", "系统管理员");
            group.ClearRegisteredEvents();

            // Act
            group.Modify("SuperAdmin", "超级管理员", "超级系统管理员");

            // Assert
            Assert.That(group.GroupName, Is.EqualTo("SuperAdmin"));
            Assert.That(group.DisplayName, Is.EqualTo("超级管理员"));
            Assert.That(group.Description, Is.EqualTo("超级系统管理员"));

            var events = group.GetRegisteredEvents();
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<PermissionGroupModifiedEvent>());
        }

        [Test]
        public void Delete_ShouldRegisterPermissionGroupDeletedEvent_AndSetDeletedAt()
        {
             // Arrange
            var id = new AggregateId(1, false);
            var group = new PermissionGroup(id, "Admin", "管理员", "系统管理员");
            group.ClearRegisteredEvents();

            // Act
            group.Delete();

            // Assert
            Assert.That(group.DeletedAt, Is.GreaterThan(0));

            var events = group.GetRegisteredEvents();
            Assert.That(events, Has.Count.EqualTo(1));
            Assert.That(events[0], Is.TypeOf<PermissionGroupDeletedEvent>());
        }
    }
}
