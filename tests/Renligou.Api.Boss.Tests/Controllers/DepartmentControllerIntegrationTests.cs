using System.Net;
using System.Net.Http.Json;
using NSubstitute;
using NUnit.Framework;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;
using Microsoft.Extensions.DependencyInjection;

namespace Renligou.Api.Boss.Tests.Controllers;

[TestFixture]
public class DepartmentControllerIntegrationTests
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
        var request = new CreateDepartmentRequest
        {
            CompanyId = 1,
            ParentId = 0,
            DeptName = "IntegrationTestDept",
            DeptCode = "ITD",
            Sorter = 1
        };
        _mockCommandBus.SendAsync<CreateDepartmentCommand, Result>(Arg.Any<CreateDepartmentCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        // Act
        var response = await _client.PostAsJsonAsync("/departments", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetDetail_ShouldReturnData_WhenExists()
    {
        // Arrange
        var id = 12345L;
        var dto = new DepartmentDetailDto { Id = id.ToString(), DeptName = "Test Dept" };
        _mockQueryBus.QueryAsync<GetDepartmentDetailQuery, Result<DepartmentDetailDto?>>(Arg.Any<GetDepartmentDetailQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<DepartmentDetailDto?>.Ok(dto));

        // Act
        var response = await _client.GetAsync($"/departments/{id}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var result = await response.Content.ReadFromJsonAsync<DepartmentDetailDto>();
        Assert.That(result?.Id, Is.EqualTo(id.ToString()));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
