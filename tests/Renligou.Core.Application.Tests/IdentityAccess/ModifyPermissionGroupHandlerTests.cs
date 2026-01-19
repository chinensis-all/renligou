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
    public class ModifyPermissionGroupHandlerTests
    {
        private Mock<IPermissionGroupRepository> _permissionGroupRepositoryMock;
        private Mock<IOutboxRepository> _outboxRepositoryMock;
        private ModifyPermissionGroupHandler _handler;

        [SetUp]
        public void SetUp()
        {
            _permissionGroupRepositoryMock = new Mock<IPermissionGroupRepository>();
            _outboxRepositoryMock = new Mock<IOutboxRepository>();

            _handler = new ModifyPermissionGroupHandler(
                _permissionGroupRepositoryMock.Object,
                _outboxRepositoryMock.Object
            );
        }

        [Test]
        public async Task HandleAsync_WhenValidCommand_ShouldSavePermissionGroup()
        {
            // Arrange
            var id = 1L;
            var command = new ModifyPermissionGroupCommand
            {
                Id = id,
                GroupName = "AdminUpdated",
                DisplayName = "管理员更新",
                Description = "DescUpdated",
                ParentId = 200,
                Sorter = 50
            };

            var existingGroup = new PermissionGroup(new AggregateId(id, false), "Admin", "管理员", "Desc", 100, 10);
            _permissionGroupRepositoryMock.Setup(x => x.LoadAsync(id)).ReturnsAsync(existingGroup);
            _permissionGroupRepositoryMock.Setup(x => x.LoadAsync(200)).ReturnsAsync(new PermissionGroup(new AggregateId(200, false), "Parent", "Parent", "Desc", 0, 0));
            _permissionGroupRepositoryMock.Setup(x => x.IsGroupNameConflictAsync(id, "AdminUpdated")).ReturnsAsync(false);
            _permissionGroupRepositoryMock.Setup(x => x.IsDisplayNameConflictAsync(id, "管理员更新")).ReturnsAsync(false);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.True);
            _permissionGroupRepositoryMock.Verify(x => x.SaveAsync(It.Is<PermissionGroup>(p => 
                p.GroupName == "AdminUpdated" && 
                p.Id.id == id &&
                p.ParentId == 200 &&
                p.Sorter == 50)), Times.Once);
            _outboxRepositoryMock.Verify(x => x.AddAsync(It.IsAny<IReadOnlyList<IIntegrationEvent>>(), "DOMAIN", "PermissionGroup", id.ToString()), Times.Once);
        }

        [Test]
        public async Task HandleAsync_WhenGroupNameConflict_ShouldReturnFail()
        {
             // Arrange
            var id = 1L;
            var command = new ModifyPermissionGroupCommand
            {
                Id = id,
                GroupName = "AdminUpdated",
                DisplayName = "管理员",
                Description = "Desc"
            };

            var existingGroup = new PermissionGroup(new AggregateId(id, false), "Admin", "管理员", "Desc", 100, 10);
            _permissionGroupRepositoryMock.Setup(x => x.LoadAsync(id)).ReturnsAsync(existingGroup);
            _permissionGroupRepositoryMock.Setup(x => x.IsGroupNameConflictAsync(id, "AdminUpdated")).ReturnsAsync(true);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.Code, Is.EqualTo("PermissionGroup.Modify.Error"));
            _permissionGroupRepositoryMock.Verify(x => x.SaveAsync(It.IsAny<PermissionGroup>()), Times.Never);
        }

        [Test]
        public async Task HandleAsync_WhenParentIsSelf_ShouldReturnFail()
        {
            // Arrange
            var id = 1L;
            var command = new ModifyPermissionGroupCommand
            {
                Id = id,
                GroupName = "Admin",
                DisplayName = "管理员",
                ParentId = id // Self
            };

            var existingGroup = new PermissionGroup(new AggregateId(id, false), "Admin", "管理员", "Desc", 100, 10);
            _permissionGroupRepositoryMock.Setup(x => x.LoadAsync(id)).ReturnsAsync(existingGroup);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.Message, Is.EqualTo("父权限组不能选择自己"));
        }

        [Test]
        public async Task HandleAsync_WhenParentNotFound_ShouldReturnFail()
        {
            // Arrange
            var id = 1L;
            var command = new ModifyPermissionGroupCommand
            {
                Id = id,
                GroupName = "Admin",
                DisplayName = "管理员",
                ParentId = 999
            };

            var existingGroup = new PermissionGroup(new AggregateId(id, false), "Admin", "管理员", "Desc", 100, 10);
            _permissionGroupRepositoryMock.Setup(x => x.LoadAsync(id)).ReturnsAsync(existingGroup);
            _permissionGroupRepositoryMock.Setup(x => x.LoadAsync(999)).ReturnsAsync((PermissionGroup)null);

            // Act
            var result = await _handler.HandleAsync(command, CancellationToken.None);

            // Assert
            Assert.That(result.Success, Is.False);
            Assert.That(result.Error.Message, Is.EqualTo("父权限组不存在"));
        }
    }
}
