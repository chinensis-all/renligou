using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Domain.Tests.Common;

[TestFixture]
public class AggregateBaseTests
{
    private class TestAggregate : AggregateBase
    {
        public TestAggregate(long id)
        {
            Id = new AggregateId(id, true);
        }

        public void DoSomething()
        {
            RegisterEvent(new TestIntegrationEvent());
        }
    }

    private class TestIntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTimeOffset OccurredAt { get; } = DateTimeOffset.UtcNow;
    }

    [SetUp]
    public void SetUp()
    {
        OutboxCollector.Clear();
    }

    [Test]
    public void RegisterEvent_ShouldAddEventToList()
    {
        // Arrange
        var aggregate = new TestAggregate(123);

        // Act
        // NOTE: This will fail until the fix is applied since the current implementation
        // of AggregateBase.RegisterEvent
        aggregate.RegisterEvent(new TestIntegrationEvent());

        // Assert
        var events = aggregate.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<TestIntegrationEvent>());
    }

    [Test]
    public void RegisterEvent_ShouldCollectInOutboxCollector()
    {
        // Arrange
        var aggregate = new TestAggregate(123);

        // Act
        aggregate.RegisterEvent(new TestIntegrationEvent());

        // Assert
        var collected = OutboxCollector.Drain();
        Assert.That(collected, Has.Count.EqualTo(1));
        Assert.That(collected[0].Item1, Is.TypeOf<TestIntegrationEvent>());
        Assert.That(collected[0].Item4, Is.EqualTo("123"), "AggregateId should be the ID string, not 'True' or 'False'");
    }

    [Test]
    public void ClearRegisteredEvents_ShouldEmptyTheList()
    {
        // Arrange
        var aggregate = new TestAggregate(123);
        aggregate.RegisterEvent(new TestIntegrationEvent());

        // Act
        aggregate.ClearRegisteredEvents();

        // Assert
        Assert.That(aggregate.GetRegisteredEvents(), Is.Empty);
    }
}
