using Microsoft.Extensions.DependencyInjection;
using Renligou.Core.Application.Bus;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Pipeline;

namespace Renligou.Core.Application.Tests.Bus;

[TestFixture]
public class PipelineTests
{
    private IServiceCollection _services;
    private IServiceProvider _serviceProvider;
    private ICommandBus _bus;

    [SetUp]
    public void SetUp()
    {
        _services = new ServiceCollection();
        _services.AddScoped<ICommandBus, CommandBus>();
    }

    [Test]
    public async Task Pipeline_ShouldExecuteInCorrectOrder()
    {
        // Arrange
        var log = new List<string>();
        _services.AddSingleton(log);
        _services.AddScoped<ICommandHandler<TestPipelineCommand>, TestPipelineCommandHandler>();
        _services.AddScoped<IPipelineBehavior<TestPipelineCommand, object>, BehaviorOne>();
        _services.AddScoped<IPipelineBehavior<TestPipelineCommand, object>, BehaviorTwo>();
        
        RefreshBus();

        // Act
        await _bus.SendAsync(new TestPipelineCommand());

        // Assert
        Assert.That(log[0], Is.EqualTo("BehaviorOne Start"));
        Assert.That(log[1], Is.EqualTo("BehaviorTwo Start"));
        Assert.That(log[2], Is.EqualTo("Handled"));
        Assert.That(log[3], Is.EqualTo("BehaviorTwo End"));
        Assert.That(log[4], Is.EqualTo("BehaviorOne End"));
    }

    [Test]
    public async Task Pipeline_ShortCircuit_ShouldNotCallHandler()
    {
        // Arrange
        var log = new List<string>();
        _services.AddSingleton(log);
        _services.AddScoped<ICommandHandler<TestPipelineCommand>, TestPipelineCommandHandler>();
        _services.AddScoped<IPipelineBehavior<TestPipelineCommand, object>, ShortCircuitBehavior>();
        
        RefreshBus();

        // Act
        await _bus.SendAsync(new TestPipelineCommand());

        // Assert
        Assert.That(log.Contains("Handled"), Is.False);
        Assert.That(log[0], Is.EqualTo("ShortCircuited"));
    }

    private void RefreshBus()
    {
        _serviceProvider = _services.BuildServiceProvider();
        _bus = _serviceProvider.GetRequiredService<ICommandBus>();
    }

    #region Test Supports

    public class TestPipelineCommand : ICommand { }

    public class TestPipelineCommandHandler : ICommandHandler<TestPipelineCommand>
    {
        private readonly List<string> _log;
        public TestPipelineCommandHandler(List<string> log) => _log = log;

        public Task HandleAsync(TestPipelineCommand command, CancellationToken cancellationToken)
        {
            _log.Add("Handled");
            return Task.CompletedTask;
        }
    }

    public class BehaviorOne : IPipelineBehavior<TestPipelineCommand, object>
    {
        private readonly List<string> _log;
        public BehaviorOne(List<string> log) => _log = log;

        public async Task<object> HandleAsync(TestPipelineCommand request, CancellationToken ct, Func<Task<object>> next)
        {
            _log.Add("BehaviorOne Start");
            var result = await next();
            _log.Add("BehaviorOne End");
            return result;
        }
    }

    public class BehaviorTwo : IPipelineBehavior<TestPipelineCommand, object>
    {
        private readonly List<string> _log;
        public BehaviorTwo(List<string> log) => _log = log;

        public async Task<object> HandleAsync(TestPipelineCommand request, CancellationToken ct, Func<Task<object>> next)
        {
            _log.Add("BehaviorTwo Start");
            var result = await next();
            _log.Add("BehaviorTwo End");
            return result;
        }
    }

    public class ShortCircuitBehavior : IPipelineBehavior<TestPipelineCommand, object>
    {
        private readonly List<string> _log;
        public ShortCircuitBehavior(List<string> log) => _log = log;

        public Task<object> HandleAsync(TestPipelineCommand request, CancellationToken ct, Func<Task<object>> next)
        {
            _log.Add("ShortCircuited");
            return Task.FromResult<object>(null!);
        }
    }

    #endregion
}
