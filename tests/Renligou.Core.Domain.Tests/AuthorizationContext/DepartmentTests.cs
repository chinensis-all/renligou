using NUnit.Framework;
using Renligou.Core.Domain.AuthorizationContext.Event;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Value;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.Tests.AuthorizationContext;

[TestFixture]
public class DepartmentTests
{
    [Test]
    public void Create_ShouldRegisterDepartmentCreatedEvent()
    {
        // Arrange
        var id = new AggregateId(100, true);
        var department = new Department(id, 0, 1, "IT Dept", "IT", "Information Tech", 1);

        // Act
        department.Create();

        // Assert
        var events = department.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<DepartmentCreatedEvent>());
        
        var @event = (DepartmentCreatedEvent)events[0];
        Assert.That(@event.Id, Is.EqualTo(100));
        Assert.That(@event.DeptName, Is.EqualTo("IT Dept"));
        Assert.That(@event.Status, Is.EqualTo("ACTIVE"));
    }

    [Test]
    public void ModifyBasic_ShouldUpdatePropertiesAndRegisterDepartmentModifiedEvent()
    {
        // Arrange
        var id = new AggregateId(100, false);
        var department = new Department(id, 0, 1, "IT Dept", "IT", "Information Tech", 1);

        // Act
        department.ModifyBasic(10, "CS Dept", "CS", "Computer Science", 2);

        // Assert
        Assert.That(department.ParentId, Is.EqualTo(10));
        Assert.That(department.DeptName, Is.EqualTo("CS Dept"));
        Assert.That(department.DeptCode, Is.EqualTo("CS"));

        var events = department.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<DepartmentModifiedEvent>());
    }

    [Test]
    public void Inactive_ShouldUpdateStatusAndRegisterEvent()
    {
        // Arrange
        var id = new AggregateId(100, false);
        var department = new Department(id, 0, 1, "IT Dept", "IT", "Information Tech", 1);

        // Act
        department.Inactive();

        // Assert
        Assert.That(department.Status, Is.EqualTo(DepartmentStatus.Inactive));
        
        var events = department.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<DepartmentStatusModifiedEvent>());
        var @event = (DepartmentStatusModifiedEvent)events[0];
        Assert.That(@event.Status, Is.EqualTo("INACTIVE"));
    }

    [Test]
    public void Activate_ShouldUpdateStatusAndRegisterEvent()
    {
        // Arrange
        var id = new AggregateId(100, false);
        var department = new Department(id, 0, 1, "IT Dept", "IT", "Information Tech", 1);
        department.Inactive();
        department.ClearRegisteredEvents();

        // Act
        department.Activate();

        // Assert
        Assert.That(department.Status, Is.EqualTo(DepartmentStatus.Active));
        
        var events = department.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<DepartmentStatusModifiedEvent>());
        var @event = (DepartmentStatusModifiedEvent)events[0];
        Assert.That(@event.Status, Is.EqualTo("ACTIVE"));
    }
}
