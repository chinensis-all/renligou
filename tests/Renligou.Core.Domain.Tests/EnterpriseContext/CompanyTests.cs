using Renligou.Core.Domain.CommonContext.Value;
using Renligou.Core.Domain.EnterpriseContext.Event;
using Renligou.Core.Domain.EnterpriseContext.Model;
using Renligou.Core.Domain.EnterpriseContext.Value;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Domain.Tests.EnterpriseContext;

[TestFixture]
public class CompanyTests
{
    private Company CreateTestCompany(long id = 1)
    {
        var address = new Address(1, "Prov", 2, "City", 3, "Dist", "Street");
        var state = new CompanyState(true, DateOnly.FromDateTime(DateTime.Today), null);
        
        return new Company(
            new AggregateId(id, true),
            CompanyType.HEADQUARTER,
            "C001",
            "Test Company",
            "TC",
            "Legal Person",
            "123456789",
            "Registered Address",
            "Remark",
            address,
            state
        );
    }

    [Test]
    public void Create_ShouldRegisterCompanyCreatedEvent()
    {
        // Arrange
        var company = CreateTestCompany();

        // Act
        company.Create();

        // Assert
        var events = company.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<CompanyCreatedEvent>());
        
        var @event = (CompanyCreatedEvent)events[0];
        Assert.That(@event.CompanyName, Is.EqualTo("Test Company"));
        Assert.That(@event.CompanyId, Is.EqualTo(company.Id.id));
    }

    [Test]
    public void ModifyBasic_ShouldRegisterCompanyBasicModifiedEvent()
    {
        // Arrange
        var company = CreateTestCompany();
        company.ClearRegisteredEvents();

        // Act
        company.ModifyBasic(
            CompanyType.BRANCH,
            "C002",
            "New Name",
            "NN",
            "New Legal",
            "987654321",
            "New Reg Addr",
            "New Remark"
        );

        // Assert
        var events = company.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<CompanyBasicModifiedEvent>());
        
        var @event = (CompanyBasicModifiedEvent)events[0];
        Assert.That(@event.CompanyName, Is.EqualTo("New Name"));
        Assert.That(@event.CompanyId, Is.EqualTo(company.Id.id));
    }

    [Test]
    public void ModifyAddress_ShouldRegisterCompanyAddressModifiedEvent()
    {
        // Arrange
        var company = CreateTestCompany();
        company.ClearRegisteredEvents();
        var newAddress = new Address(4, "P4", 5, "C5", 6, "D6", "New Street");

        // Act
        company.ModifyAddress(newAddress);

        // Assert
        var events = company.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<CompanyAddressModifiedEvent>());
        
        var @event = (CompanyAddressModifiedEvent)events[0];
        Assert.That(@event.Address.CompletedAddress, Is.EqualTo("New Street"));
    }

    [Test]
    public void ModifyState_ShouldRegisterCompanyStateModifiedEvent()
    {
        // Arrange
        var company = CreateTestCompany();
        company.ClearRegisteredEvents();
        var newState = new CompanyState(false, null, null);

        // Act
        company.ModifyState(newState);

        // Assert
        var events = company.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<CompanyStateModifiedEvent>());
        
        var @event = (CompanyStateModifiedEvent)events[0];
        Assert.That(@event.State.Enabled, Is.False);
    }
}
