using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;
using Renligou.Core.Application.Enterprise.Queries;
using NSubstitute;

namespace Renligou.Api.Boss.Tests;

[TestFixture]
public class CompanyControllerIntegrationTests
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
                
                // Mock UoW ExecuteAsync to just run the action
                _mockUow.ExecuteAsync<Result>(Arg.Any<Func<Task<Result>>>(), Arg.Any<bool>())
                    .Returns(x => ((Func<Task<Result>>)x[0])());
                
                _mockUow.ExecuteAsync<Result<CompanyDetailDto?>>(Arg.Any<Func<Task<Result<CompanyDetailDto?>>>>(), Arg.Any<bool>())
                    .Returns(x => ((Func<Task<Result<CompanyDetailDto?>>>)x[0])());
            });
        }).CreateClient();
    }

    [Test]
    public async Task Create_ShouldReturnOk_WhenCommandSucceeds()
    {
        // Arrange
        var request = new CreateCompanyRequest
        {
            CompanyName = "Test Company",
            CompanyType = "HEADQUARTERS",
            CompanyShortName = "TC",
            ProvinceId = 1,
            CityId = 2,
            DistrictId = 3,
            Enabled = true,
            EffectiveDate = "2024-01-01"
        };
        _mockCommandBus.SendAsync<Renligou.Core.Application.Enterprise.Commands.CreateCompanyCommand, Result>(Arg.Any<Renligou.Core.Application.Enterprise.Commands.CreateCompanyCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Ok());

        // Act
        var response = await _client.PostAsJsonAsync("/companies", request);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        await _mockCommandBus.Received(1).SendAsync<Renligou.Core.Application.Enterprise.Commands.CreateCompanyCommand, Result>(Arg.Any<Renligou.Core.Application.Enterprise.Commands.CreateCompanyCommand>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetDetail_ShouldReturnData_WhenQuerySucceeds()
    {
        // Arrange
        long companyId = 123;
        var dto = new CompanyDetailDto { CompanyName = "Test Company" };
        _mockQueryBus.QueryAsync<GetCompanyDetailQuery, Result<CompanyDetailDto?>>(Arg.Any<GetCompanyDetailQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result<CompanyDetailDto?>.Ok(dto));

        // Act
        var response = await _client.GetAsync($"/companies/{companyId}");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var result = await response.Content.ReadFromJsonAsync<CompanyDetailDto>();
        Assert.That(result?.CompanyName, Is.EqualTo("Test Company"));
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
