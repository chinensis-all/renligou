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
    public void Create_ShouldRegisterCreatedEvent()
    {
        var id = new AggregateId(1, true);
        var dept = new Department(id, 0, 1, "TestDept", "TD", "Desc", 1, DepartmentStatus.Active);

        dept.Create();

        var events = dept.GetRegisteredEvents();
        Assert.That(events.Count, Is.EqualTo(1));
        Assert.IsInstanceOf<DepartmentCreatedEvent>(events[0]);
        var e = (DepartmentCreatedEvent)events[0];
        Assert.That(e.DeptName, Is.EqualTo("TestDept"));
        Assert.That(e.Status, Is.EqualTo(DepartmentStatus.Active));
    }

    [Test]
    public void ModifyBasic_ShouldRegisterModifiedEvent()
    {
        var id = new AggregateId(1, true);
        var dept = new Department(id, 0, 1, "TestDept", "TD", "Desc", 1, DepartmentStatus.Active);

        dept.ModifyBasic(0, 1, "NewName", "NewCode", "NewDesc");

        var events = dept.GetRegisteredEvents();
        Assert.That(events.Count, Is.EqualTo(1));
        Assert.IsInstanceOf<DepartmentBasicModifiedEvent>(events[0]);
        var e = (DepartmentBasicModifiedEvent)events[0];
        Assert.That(e.DeptName, Is.EqualTo("NewName"));
        Assert.That(dept.DeptName, Is.EqualTo("NewName"));
    }

    [Test]
    public void Activate_ShouldRegisterStatusModifiedEvent()
    {
        var id = new AggregateId(1, true);
        var dept = new Department(id, 0, 1, "TestDept", "TD", "Desc", 1, DepartmentStatus.Inactive);

        dept.Activate();

        var events = dept.GetRegisteredEvents();
        Assert.That(events.Count, Is.EqualTo(1));
        Assert.IsInstanceOf<DepartmentStatusModifiedEvent>(events[0]);
        var e = (DepartmentStatusModifiedEvent)events[0];
        Assert.That(e.Status, Is.EqualTo(DepartmentStatus.Active));
        Assert.That(dept.Status, Is.EqualTo(DepartmentStatus.Active));
    }
}
