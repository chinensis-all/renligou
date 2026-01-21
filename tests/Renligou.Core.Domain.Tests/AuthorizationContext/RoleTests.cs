using NUnit.Framework;
using Renligou.Core.AuthorizationContext.Event;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.Tests.AuthorizationContext;

[TestFixture]
public class RoleTests
{
    [Test]
    public void Create_ShouldRegisterRoleCreatedEvent()
    {
        // Arrange
        var id = new AggregateId(12345, true);
        var role = new Role(id, "Admin", "管理员", "描述");

        // Act
        role.Create();

        // Assert
        var events = role.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<RoleCreatedEvent>());
        
        var @event = (RoleCreatedEvent)events[0];
        Assert.That(@event.Id, Is.EqualTo(12345));
        Assert.That(@event.RoleName, Is.EqualTo("Admin"));
        Assert.That(@event.DisplayName, Is.EqualTo("管理员"));
    }

    [Test]
    public void ModifyBasic_ShouldUpdatePropertiesAndRegisterRoleModifiedEvent()
    {
        // Arrange
        var id = new AggregateId(12345, false);
        var role = new Role(id, "Admin", "管理员", "描述");

        // Act
        role.ModifyBasic("SuperAdmin", "超级管理员", "新描述");

        // Assert
        Assert.That(role.RoleName, Is.EqualTo("SuperAdmin"));
        Assert.That(role.DisplayName, Is.EqualTo("超级管理员"));

        var events = role.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<RoleModifiedEvent>());

        var @event = (RoleModifiedEvent)events[0];
        Assert.That(@event.RoleName, Is.EqualTo("SuperAdmin"));
        Assert.That(@event.DisplayName, Is.EqualTo("超级管理员"));
    }

    [Test]
    public void Destroy_ShouldRegisterRoleDestroyedEvent()
    {
        // Arrange
        var id = new AggregateId(12345, false);
        var role = new Role(id, "Admin", "管理员", "描述");

        // Act
        role.Destroy();

        // Assert
        var events = role.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<RoleDestroyedEvent>());

        var @event = (RoleDestroyedEvent)events[0];
        Assert.That(@event.Id, Is.EqualTo(12345));
    }
}
