using Renligou.Core.Application.Kernel.Commands;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.Tests.Kernel.Commands;

[TestFixture]
public class CreateOutboxHandlerTests
{
    private MockOutboxRepository _repository;
    private CreateOutboxHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _repository = new MockOutboxRepository();
        _handler = new CreateOutboxHandler(_repository);
        OutboxCollector.Clear();
    }

    [Test]
    public async Task HandleAsync_WhenEventsExist_ShouldSaveEvents()
    {
        // Arrange
        var ev1 = new TestIntegrationEvent();
        OutboxCollector.Collect(ev1, "Category1", "Type1", "Id1");
        
        var command = new CreateOutboxCommand();

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(_repository.AddedEvents, Has.Count.EqualTo(1));
        var added = _repository.AddedEvents[0];
        Assert.That(added.Event, Is.EqualTo(ev1));
        Assert.That(added.Category, Is.EqualTo("Category1"));
        Assert.That(added.AggregateType, Is.EqualTo("Type1"));
        Assert.That(added.AggregateId, Is.EqualTo("Id1"));
    }

    [Test]
    public async Task HandleAsync_WhenNoEvents_ShouldDoNothing()
    {
        // Arrange
        var command = new CreateOutboxCommand();

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(_repository.AddedEvents, Is.Empty);
    }

    private class TestIntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredAt() => DateTime.UtcNow;
    }

    private class MockOutboxRepository : IOutboxRepository
    {
        public List<(IIntegrationEvent Event, string Category, string AggregateType, string AggregateId)> AddedEvents { get; } = new();

        public Task AddAsync(IIntegrationEvent @event, string category, string aggregateType, string aggregateId)
        {
            AddedEvents.Add((@event, category, aggregateType, aggregateId));
            return Task.CompletedTask;
        }

        public Task AddAsync(IEnumerable<IIntegrationEvent> events, string category, string aggregateType, string aggregateId)
        {
            foreach (var @event in events)
            {
                AddedEvents.Add((@event, category, aggregateType, aggregateId));
            }
            return Task.CompletedTask;
        }
    }
}
