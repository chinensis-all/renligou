using Microsoft.Extensions.DependencyInjection;
using Renligou.Core.Application.Bus;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Exceptions;

namespace Renligou.Core.Application.Tests.Bus;

[TestFixture]
public class CommandBusTests
{
    private IServiceCollection _services;
    private IServiceProvider _serviceProvider;
    private ICommandBus _bus;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _services.AddScoped<ICommandBus, CommandBus>();
        // 模拟 sp
        _serviceProvider = _services.BuildServiceProvider();
        _bus = _serviceProvider.GetRequiredService<ICommandBus>();
    }

    [Test]
    public async Task SendAsync_NoResult_ShouldSucceed()
    {
        // Arrange
        var handler = new TestCommandHandler();
        _services.AddSingleton<ICommandHandler<TestCommand>>(handler);
        RefreshBus();

        var cmd = new TestCommand();

        // Act
        await _bus.SendAsync(cmd);

        // Assert
        Assert.That(handler.Handled, Is.True);
    }

    [Test]
    public async Task SendAsync_WithResult_ShouldReturnResult()
    {
        // Arrange
        var expectedResult = "Result123";
        _services.AddScoped<ICommandHandler<TestCommandWithResult, string>, TestCommandWithResultHandler>();
        RefreshBus();

        var cmd = new TestCommandWithResult();

        // Act
        var result = await _bus.SendAsync<TestCommandWithResult, string>(cmd);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [Test]
    public void SendAsync_NoHandler_ShouldThrowHandlerNotFoundException()
    {
        // Arrange
        var cmd = new TestCommand();

        // Act & Assert
        Assert.ThrowsAsync<HandlerNotFoundException>(async () => await _bus.SendAsync(cmd));
    }

    [Test]
    public void SendAsync_MultipleHandlers_ShouldThrowMultipleHandlersFoundException()
    {
        // Arrange
        _services.AddTransient<ICommandHandler<TestCommand>, TestCommandHandler>();
        _services.AddTransient<ICommandHandler<TestCommand>, TestCommandHandler>();
        RefreshBus();

        var cmd = new TestCommand();

        // Act & Assert
        Assert.ThrowsAsync<MultipleHandlersFoundException>(async () => await _bus.SendAsync(cmd));
    }

    [Test]
    public async Task SendAsync_CancellationToken_ShouldPropagate()
    {
        // Arrange
        var handler = new TestCommandHandler();
        _services.AddSingleton<ICommandHandler<TestCommand>>(handler);
        RefreshBus();

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var cmd = new TestCommand();

        // Act
        await _bus.SendAsync(cmd, cts.Token);

        // Assert
        Assert.That(handler.TokenState, Is.True);
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
        _bus = _serviceProvider.GetRequiredService<ICommandBus>();
    }

    #region Test Supports

    public class TestCommand : ICommand { }

    public class TestCommandHandler : ICommandHandler<TestCommand>
    {
        public bool Handled { get; private set; }
        public bool TokenState { get; private set; }

        public Task HandleAsync(TestCommand command, CancellationToken cancellationToken)
        {
            Handled = true;
            TokenState = cancellationToken.IsCancellationRequested;
            return Task.CompletedTask;
        }
    }

    public class TestCommandWithResult : ICommand<string> { }

    public class TestCommandWithResultHandler : ICommandHandler<TestCommandWithResult, string>
    {
        public Task<string> HandleAsync(TestCommandWithResult command, CancellationToken cancellationToken)
        {
            return Task.FromResult("Result123");
        }
    }

    #endregion
}
