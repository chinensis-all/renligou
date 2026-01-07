using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Renligou.Core.Infrastructure.Data.Outbox;
using Renligou.Core.Infrastructure.Event;
using Renligou.Job.Main.Kernel;

namespace Renligou.Job.Main.Tests.Kernel;

[TestFixture]
public class OutboxWorkerTests
{
    private MockLogger _logger;
    private MockScopeFactory _scopeFactory;
    private MockOutboxDapperRepository _repo;
    private MockEventPublisher _publisher;
    private OutboxWorker _worker;

    [SetUp]
    public void SetUp()
    {
        _logger = new MockLogger();
        _repo = new MockOutboxDapperRepository();
        _publisher = new MockEventPublisher();
        _scopeFactory = new MockScopeFactory(_repo, _publisher);

        _worker = new OutboxWorker(_logger, _scopeFactory);
    }

    [Test]
    public async Task ProcessAsync_WhenEventsExist_ShouldPublishAndMarkSent()
    {
        // Arrange
        var rows = new List<OutboxRow>
        {
            new OutboxRow { Id = 1, Payload = "{}" },
            new OutboxRow { Id = 2, Payload = "{}" }
        };
        _repo.BatchToReturn = rows;

        // Act
        await _worker.ProcessAsync(CancellationToken.None);

        // Assert
        Assert.That(_publisher.PublishedIds, Has.Count.EqualTo(2));
        Assert.That(_repo.SentIds, Has.Length.EqualTo(2));
        Assert.That(_repo.SentIds, Contains.Item(1L));
        Assert.That(_repo.SentIds, Contains.Item(2L));
    }

    [Test]
    public async Task ProcessAsync_WhenPublishFails_ShouldMarkFailed()
    {
        // Arrange
        var rows = new List<OutboxRow>
        {
            new OutboxRow { Id = 1, Payload = "{}" }
        };
        _repo.BatchToReturn = rows;
        _publisher.ShouldFail = true;

        // Act
        await _worker.ProcessAsync(CancellationToken.None);

        // Assert
        Assert.That(_repo.FailedIds, Has.Length.EqualTo(1));
        Assert.That(_repo.FailedIds, Contains.Item(1L));
    }

    [TearDown]
    public void TearDown()
    {
        _worker?.Dispose();
        // MockScopeFactory also implements IServiceScope which is IDisposable
        (_scopeFactory as IDisposable)?.Dispose();
    }

    #region Mock Classes

    private class MockLogger : ILogger<OutboxWorker>
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
        public bool IsEnabled(LogLevel logLevel) => true;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    }

    private class MockScopeFactory : IServiceScopeFactory, IServiceScope
    {
        private readonly IOutboxDapperRepository _repo;
        private readonly IEventPublisher _publisher;

        public MockScopeFactory(IOutboxDapperRepository repo, IEventPublisher publisher)
        {
            _repo = repo;
            _publisher = publisher;
        }

        public IServiceScope CreateScope() => this;

        public IServiceProvider ServiceProvider => new MockServiceProvider(_repo, _publisher);

        public void Dispose() { }
    }

    private class MockServiceProvider : IServiceProvider
    {
        private readonly IOutboxDapperRepository _repo;
        private readonly IEventPublisher _publisher;

        public MockServiceProvider(IOutboxDapperRepository repo, IEventPublisher publisher)
        {
            _repo = repo;
            _publisher = publisher;
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType == typeof(IOutboxDapperRepository)) return _repo;
            if (serviceType == typeof(IEventPublisher)) return _publisher;
            return null;
        }
    }

    private class MockOutboxDapperRepository : IOutboxDapperRepository
    {
        public List<OutboxRow> BatchToReturn { get; set; } = new();
        public long[] SentIds { get; private set; } = Array.Empty<long>();
        public long[] FailedIds { get; private set; } = Array.Empty<long>();

        public Task<List<OutboxRow>> DequeueBatchAsync(int batchSize, CancellationToken ct) => Task.FromResult(BatchToReturn);

        public Task MarkSentAsync(long[] ids, CancellationToken ct)
        {
            SentIds = ids;
            return Task.CompletedTask;
        }

        public Task MarkFailedAsync(long[] ids, int maxRetry, CancellationToken ct)
        {
            FailedIds = ids;
            return Task.CompletedTask;
        }
    }

    private class MockEventPublisher : IEventPublisher
    {
        public List<long> PublishedIds { get; } = new();
        public bool ShouldFail { get; set; }

        public Task PublishAsync(OutboxRow row, CancellationToken ct)
        {
            if (ShouldFail) throw new Exception("Publish failed");
            PublishedIds.Add(row.Id);
            return Task.CompletedTask;
        }
    }

    #endregion
}
