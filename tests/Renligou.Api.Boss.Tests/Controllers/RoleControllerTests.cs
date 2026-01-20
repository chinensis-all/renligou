using System.Net;
using System.Net.Http.Json;
using Moq;
using NSubstitute;
using NUnit.Framework;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;
using Renligou.Core.Shared.Repo;
using Microsoft.Extensions.DependencyInjection;

namespace Renligou.Api.Boss.Tests.Controllers;

[TestFixture]
public class RoleControllerIntegrationTests
{
    private CustomWebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private ICommandBus _mockCommandBus;
    private IQueryBus _mockQueryBus;
    private IUnitOfWork _mockUow;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _mockCommandBus = Substitute.For<ICommandBus>();
        _mockQueryBus = Substitute.For<IQueryBus>();
        _mockUow = Substitute.For<IUnitOfWork>();

        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped(_ => _mockCommandBus);
                services.AddScoped(_ => _mockQueryBus);
                services.AddScoped(_ => _mockUow);

                _mockUow.ExecuteAsync<Result>(Arg.Any<Func<Task<Result>>>(), Arg.Any<bool>())
                    .Returns(x => ((Func<Task<Result>>)x[0])());
            });
        }).CreateClient();
    }

    [Test]
    public async Task Create_ShouldReturnOk_WhenSuccessful()
    {
        // Arrange
        var request = new CreateRoleRequest { RoleName = "Test", DisplayName = "测试" };
        _mockCommandBus.SendAsync<CreateRoleCommand, Result>(Arg.Any<CreateRoleCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        // Act
        var response = await _client.PostAsJsonAsync("/roles", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetDetail_ShouldReturnData_WhenExists()
    {
        // Arrange
        var id = 12345L;
        var dto = new RoleDetailDto { Id = id, RoleName = "Admin", DisplayName = "管理员" };
        _mockQueryBus.QueryAsync<GetRoleDetailQuery, Result<RoleDetailDto?>>(Arg.Any<GetRoleDetailQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<RoleDetailDto?>.Ok(dto));

        // Act
        var response = await _client.GetAsync($"/roles/{id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var result = await response.Content.ReadFromJsonAsync<RoleDetailDto>();
        Assert.That(result?.Id, Is.EqualTo(id));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
