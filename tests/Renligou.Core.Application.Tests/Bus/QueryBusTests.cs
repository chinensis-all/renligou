using Microsoft.Extensions.DependencyInjection;
using Renligou.Core.Application.Bus;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Exceptions;
using Renligou.Core.Shared.Querying;

namespace Renligou.Core.Application.Tests.Bus;

[TestFixture]
public class QueryBusTests
{
    private IServiceCollection _services;
    private IServiceProvider _serviceProvider;
    private IQueryBus _bus;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _services.AddScoped<IQueryBus, QueryBus>();
        _serviceProvider = _services.BuildServiceProvider();
        _bus = _serviceProvider.GetRequiredService<IQueryBus>();
    }

    [Test]
    public async Task QueryAsync_ShouldReturnResult()
    {
        // Arrange
        _services.AddScoped<IQueryHandler<TestQuery, int>, TestQueryHandler>();
        RefreshBus();

        var qry = new TestQuery();

        // Act
        var result = await _bus.QueryAsync<TestQuery, int>(qry);

        // Assert
        Assert.That(result, Is.EqualTo(42));
    }

    [Test]
    public void QueryAsync_NoHandler_ShouldThrowHandlerNotFoundException()
    {
        // Arrange
        var qry = new TestQuery();

        // Act & Assert
        Assert.ThrowsAsync<HandlerNotFoundException>(async () => await _bus.QueryAsync<TestQuery, int>(qry));
    }

    [Test]
    public async Task QueryAsync_CancellationToken_ShouldPropagate()
    {
        // Arrange
        _services.AddScoped<IQueryHandler<TestQuery, int>, TestQueryHandler>();
        RefreshBus();

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var qry = new TestQuery();

        // Act
        var result = await _bus.QueryAsync<TestQuery, int>(qry, cts.Token);

        // Assert
        Assert.That(result, Is.EqualTo(-1)); // Handler returns -1 if cancelled
    }

    [TearDown]
    public void TearDown()
    {
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void RefreshBus()
    {
        _serviceProvider = _services.BuildServiceProvider();
        _bus = _serviceProvider.GetRequiredService<IQueryBus>();
    }

    #region Test Supports

    public class TestQuery : IQuery<int> { }

    public class TestQueryHandler : IQueryHandler<TestQuery, int>
    {
        public Task<int> HandleAsync(TestQuery query, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return Task.FromResult(-1);
            return Task.FromResult(42);
        }
    }

    #endregion
}
