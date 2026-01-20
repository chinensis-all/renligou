using Moq;
using NUnit.Framework;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.Tests.IdentityAccess
{
    [TestFixture]
    public class CreatePermissionHandlerTests
    {
        private Mock<IPermissionRepository> _permissionRepoMock;
        private Mock<IPermissionGroupRepository> _groupRepoMock;
        private Mock<IOutboxRepository> _outboxRepoMock;
        private Mock<IIdGenerator> _idGenMock;
        private CreatePermissionHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _permissionRepoMock = new Mock<IPermissionRepository>();
            _groupRepoMock = new Mock<IPermissionGroupRepository>();
            _outboxRepoMock = new Mock<IOutboxRepository>();
            _idGenMock = new Mock<IIdGenerator>();
            _handler = new CreatePermissionHandler(
                _permissionRepoMock.Object,
                _groupRepoMock.Object,
                _outboxRepoMock.Object,
                _idGenMock.Object
            );
        }

        [Test]
        public async Task HandleAsync_WhenGroupDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var command = new CreatePermissionCommand
            {
                GroupId = 1,
                PermissionName = "test",
                DisplayName = "Test"
            };
            _groupRepoMock.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.Message, Does.Contain("不存在"));
        }

        [Test]
        public async Task HandleAsync_WhenValid_ShouldSaveAndReturnOk()
        {
            // Arrange
            var command = new CreatePermissionCommand
            {
                GroupId = 1,
                PermissionName = "test",
                DisplayName = "Test"
            };
            _groupRepoMock.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _permissionRepoMock.Setup(x => x.IsPermissionNameConflictAsync(It.IsAny<long>(), "test")).ReturnsAsync(false);
            _permissionRepoMock.Setup(x => x.IsDisplayNameConflictAsync(It.IsAny<long>(), "Test")).ReturnsAsync(false);
            _idGenMock.Setup(x => x.NextId()).Returns(100);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.True);
            _permissionRepoMock.Verify(x => x.SaveAsync(It.IsAny<Permission>()), Times.Once);
            _outboxRepoMock.Verify(x => x.AddAsync(It.IsAny<IEnumerable<IIntegrationEvent>>(), "DOMAIN", nameof(Permission), "100"), Times.Once);
        }
    }
}
