using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;

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
            DeptName = "IT", 
            DeptCode = "IT01" 
        };
        _mockCommandBus.SendAsync<CreateDepartmentCommand, Result>(Arg.Any<CreateDepartmentCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        // Act
        var response = await _client.PostAsJsonAsync("/departments", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task GetTree_ShouldReturnData()
    {
        // Arrange
        var tree = new List<DepartmentTreeNodeDto>
        {
            new DepartmentTreeNodeDto { Id = 1, Name = "Root", Children = new List<DepartmentTreeNodeDto>() }
        };
        _mockQueryBus.QueryAsync<GetDepartmentTreeQuery, List<DepartmentTreeNodeDto>>(Arg.Any<GetDepartmentTreeQuery>(), Arg.Any<CancellationToken>())
            .Returns(tree);

        // Act
        var response = await _client.GetAsync("/departments/tree?CompanyId=1");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var result = await response.Content.ReadFromJsonAsync<List<DepartmentTreeNodeDto>>();
        Assert.That(result, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Inactive_ShouldReturnOk()
    {
        // Arrange
        _mockCommandBus.SendAsync<InactiveDepartmentCommand, Result>(Arg.Any<InactiveDepartmentCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        // Act
        var response = await _client.PostAsync("/departments/123/lock", null);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
