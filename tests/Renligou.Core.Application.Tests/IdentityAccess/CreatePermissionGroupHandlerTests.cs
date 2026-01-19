using Moq;
using NUnit.Framework;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.Tests.IdentityAccess
{
    [TestFixture]
    public class CreatePermissionGroupHandlerTests
    {
        private Mock<IPermissionGroupRepository> _permissionGroupRepositoryMock;
        private Mock<IOutboxRepository> _outboxRepositoryMock;
        private Mock<IIdGenerator> _idGeneratorMock;
        private CreatePermissionGroupHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _permissionGroupRepositoryMock = new Mock<IPermissionGroupRepository>();
            _outboxRepositoryMock = new Mock<IOutboxRepository>();
            _idGeneratorMock = new Mock<IIdGenerator>();

            _handler = new CreatePermissionGroupHandler(
                _permissionGroupRepositoryMock.Object,
                _outboxRepositoryMock.Object,
                _idGeneratorMock.Object
            );
        }

        [Test]
        public async Task HandleAsync_WhenValidCommand_ShouldSavePermissionGroup()
        {
            // Arrange
            var command = new CreatePermissionGroupCommand
            {
                GroupName = "Admin",
                DisplayName = "管理员",
                Description = "Desc",
                ParentId = 100,
                Sorter = 99
            };

            _idGeneratorMock.Setup(x => x.NextId()).Returns(1);
            _permissionGroupRepositoryMock.Setup(x => x.LoadAsync(100)).ReturnsAsync(new PermissionGroup(new AggregateId(100, false), "Parent", "Parent", "Desc", 0, 0));
            _permissionGroupRepositoryMock.Setup(x => x.IsGroupNameConflictAsync(1, "Admin")).ReturnsAsync(false);
            _permissionGroupRepositoryMock.Setup(x => x.IsDisplayNameConflictAsync(1, "管理员")).ReturnsAsync(false);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.True);
            _permissionGroupRepositoryMock.Verify(x => x.SaveAsync(It.Is<PermissionGroup>(p => 
                p.GroupName == "Admin" && 
                p.Id.id == 1 &&
                p.ParentId == 100 &&
                p.Sorter == 99)), Times.Once);
            _outboxRepositoryMock.Verify(x => x.AddAsync(It.IsAny<IReadOnlyList<IIntegrationEvent>>(), "DOMAIN", "PermissionGroup", "1"), Times.Once);
        }

        [Test]
        public async Task HandleAsync_WhenGroupNameConflict_ShouldReturnFail()
        {
            // Arrange
            var command = new CreatePermissionGroupCommand
            {
                GroupName = "Admin",
                DisplayName = "管理员",
                Description = "Desc"
            };

            _idGeneratorMock.Setup(x => x.NextId()).Returns(1);
            _permissionGroupRepositoryMock.Setup(x => x.IsGroupNameConflictAsync(1, "Admin")).ReturnsAsync(true);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.Code, Is.EqualTo("PermissionGroup.Create.Error"));
            Assert.That(result.Error.Message, Is.EqualTo("权限组名称已存在"));
            _permissionGroupRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<PermissionGroup>()), Times.Never);
        }

        [Test]
        public async Task HandleAsync_WhenParentNotFound_ShouldReturnFail()
        {
            // Arrange
            var command = new CreatePermissionGroupCommand
            {
                GroupName = "Admin",
                DisplayName = "管理员",
                ParentId = 999
            };

            _permissionGroupRepositoryMock.Setup(x => x.LoadAsync(999)).ReturnsAsync((PermissionGroup)null);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.Message, Is.EqualTo("父权限组不存在"));
        }
    }
}
