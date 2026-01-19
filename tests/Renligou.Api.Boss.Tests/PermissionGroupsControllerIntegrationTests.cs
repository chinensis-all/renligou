using System.Net;
using System.Net.Http.Json;
using NSubstitute;
using NUnit.Framework;
using Renligou.Api.Boss;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;

namespace Renligou.Api.Boss.Tests
{
    [TestFixture]
    public class PermissionGroupsControllerIntegrationTests
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
        public async Task Create_ShouldReturnOk_WhenCommandSucceeds()
        {
            // Arrange
            var request = new CreatePermissionGroupRequest
            {
                GroupName = "Admin",
                DisplayName = "管理员",
                Description = "Desc",
                ParentId = 100,
                Sorter = 99
            };

            _mockCommandBus.SendAsync<CreatePermissionGroupCommand, Result>(Arg.Any<CreatePermissionGroupCommand>(), Arg.Any<CancellationToken>())
                .Returns(Result.Ok());

            // Act
            var response = await _client.PostAsJsonAsync("/permission-groups", request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            await _mockCommandBus.Received(1).SendAsync<CreatePermissionGroupCommand, Result>(Arg.Is<CreatePermissionGroupCommand>(c => c.ParentId == 100 && c.Sorter == 99), Arg.Any<CancellationToken>());
        }

        [Test]
        public async Task GetDetail_ShouldReturnData_WhenQuerySucceeds()
        {
            // Arrange
            long id = 123;
            var dto = new PermissionGroupDetailDto { Id = "123", GroupName = "Admin", DisplayName = "管理员", ParentId = "100" };

            _mockQueryBus.QueryAsync<GetPermissionGroupDetailQuery, Result<PermissionGroupDetailDto?>>(Arg.Any<GetPermissionGroupDetailQuery>(), Arg.Any<CancellationToken>())
                .Returns(Result<PermissionGroupDetailDto?>.Ok(dto));

            // Act
            var response = await _client.GetAsync($"/permission-groups/{id}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var result = await response.Content.ReadFromJsonAsync<PermissionGroupDetailDto>();
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo("123"));
            Assert.That(result.ParentId, Is.EqualTo("100"));
        }

        [Test]
        public async Task Destroy_ShouldReturnOk_WhenCommandSucceeds()
        {
            // Arrange
            long id = 123;
            _mockCommandBus.SendAsync<DestroyPermissionGroupCommand, Result>(Arg.Any<DestroyPermissionGroupCommand>(), Arg.Any<CancellationToken>())
                .Returns(Result.Ok());

            // Act
            var response = await _client.DeleteAsync($"/permission-groups/{id}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            await _mockCommandBus.Received(1).SendAsync<DestroyPermissionGroupCommand, Result>(Arg.Is<DestroyPermissionGroupCommand>(c => c.Id == id), Arg.Any<CancellationToken>());
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}
