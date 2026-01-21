using NUnit.Framework;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Event;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Model;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.Tests.IdentityAccess;

[TestFixture]
public class MenuTests
{
    [Test]
    public void Create_ShouldRegisterMenuCreatedEvent()
    {
        // Arrange
        var id = new AggregateId(1, true);
        var menu = new Menu(id, 0, "TestMenu", "Tag", "/path", "Comp", "icon", 1, false, "btn");

        // Act
        menu.Create();

        // Assert
        var events = menu.GetRegisteredEvents();
        Assert.That(events.Count, Is.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<MenuCreatedEvent>());
        var createdEvent = (MenuCreatedEvent)events[0];
        Assert.That(createdEvent.Id, Is.EqualTo(1));
        Assert.That(createdEvent.MenuName, Is.EqualTo("TestMenu"));
    }

    [Test]
    public void Modify_ShouldUpdatePropertiesAndRegisterEvent()
    {
        // Arrange
        var id = new AggregateId(1, false);
        var menu = new Menu(id, 0, "Old", "OldTag", "/old", "OldComp", "oldIcon", 1, false, "oldBtn");

        // Act
        menu.Modify(1, "New", "NewTag", "/new", "NewComp", "newIcon", 2, "newBtn");

        // Assert
        Assert.That(menu.MenuName, Is.EqualTo("New"));
        Assert.That(menu.ParentId, Is.EqualTo(1));
        var events = menu.GetRegisteredEvents();
        Assert.That(events.Any(e => e is MenuModifiedEvent), Is.True);
    }

    [Test]
    public void ChangeVisibility_ShouldUpdateAndRegisterEvent_WhenValueChanges()
    {
        // Arrange
        var menu = new Menu(new AggregateId(1, false), 0, "N", "T", "P", "C", "I", 1, false, "B");

        // Act
        menu.ChangeVisibility(true);

        // Assert
        Assert.That(menu.IsHidden, Is.True);
        Assert.That(menu.GetRegisteredEvents().Any(e => e is MenuVisibilityChangedEvent), Is.True);
    }

    [Test]
    public void ChangeVisibility_ShouldNotRegisterEvent_WhenValueIsSame()
    {
        // Arrange
        var menu = new Menu(new AggregateId(1, false), 0, "N", "T", "P", "C", "I", 1, false, "B");

        // Act
        menu.ChangeVisibility(false);

        // Assert
        Assert.That(menu.GetRegisteredEvents().Any(e => e is MenuVisibilityChangedEvent), Is.False);
    }

    [Test]
    public void Destroy_ShouldRegisterMenuDestroyedEvent()
    {
        // Arrange
        var menu = new Menu(new AggregateId(1, false), 0, "N", "T", "P", "C", "I", 1, false, "B");

        // Act
        menu.Destroy();

        // Assert
        Assert.That(menu.GetRegisteredEvents().Any(e => e is MenuDestroyedEvent), Is.True);
    }
}
