using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Renligou.Core.Application.Bus;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Moq;

namespace Renligou.Core.Application.Tests.IdentityAccess;

[TestFixture]
public class DepartmentPipelineTests
{
    private ServiceProvider _provider;

    [SetUp]
    public void Setup()
    {
        var services = new ServiceCollection();

        // Mock dependencies
        services.AddScoped(_ => new Mock<IDepartmentRepository>().Object);
        services.AddScoped(_ => new Mock<IOutboxRepository>().Object);
        services.AddScoped(_ => new Mock<IIdGenerator>().Object);

        // Register CommandBus
        services.AddScoped<ICommandBus, CommandBus>();

        // Register Handlers
        services.AddScoped<ICommandHandler<CreateDepartmentCommand, Result>, CreateDepartmentHandler>();

        _provider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown()
    {
        _provider.Dispose();
    }

    [Test]
    public async Task SendAsync_RegisteredHandler_ShouldInvokeHandler()
    {
        var bus = _provider.GetRequiredService<ICommandBus>();
        var command = new CreateDepartmentCommand { CompanyId = 1, DeptName = "Test", ParentId = 0 };

        // Note: Dependencies are mocked to return default (null/false), so it might fail inside handler if not setup.
        // But we just want to verify dispatch.
        // To properly test validation failure vs success, we need to setup mocks.

        // We expect Result because handler returns Result.
        // However, without setting up Mocks properly, it might throw or return Fail.
        // We just assert that it didn't throw "HandlerNotFound".

        var result = await bus.SendAsync<CreateDepartmentCommand, Result>(command);
        Assert.NotNull(result);
    }

    [Test]
    public void SendAsync_UnregisteredHandler_ShouldThrow()
    {
        var bus = _provider.GetRequiredService<ICommandBus>();
        // Using a fake command that isn't registered
        var command = new UnregisteredCommand();

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await bus.SendAsync<UnregisteredCommand, Result>(command));
    }

    public sealed record UnregisteredCommand : Core.Shared.Commanding.ICommand<Result>;
}
