using Moq;
using NUnit.Framework;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.Tests.IdentityAccess;

[TestFixture]
public class ModifyAndDestroyRoleHandlerTests
{
    private Mock<IRoleRepository> _roleRepositoryMock;
    private Mock<IOutboxRepository> _outboxRepositoryMock;
    private ModifyRoleBasicHandler _modifyHandler;
    private DestroyRoleHandler _destroyHandler;

    [SetUp]
    public void SetUp()
    {
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _outboxRepositoryMock = new Mock<IOutboxRepository>();

        _modifyHandler = new ModifyRoleBasicHandler(_roleRepositoryMock.Object, _outboxRepositoryMock.Object);
        _destroyHandler = new DestroyRoleHandler(_roleRepositoryMock.Object, _outboxRepositoryMock.Object);
    }

    [Test]
    public async Task Modify_WhenValid_ShouldUpdateAndSave()
    {
        // Arrange
        var id = new AggregateId(123, false);
        var role = new Role(id, "OldName", "OldDisplay", "OldDesc");
        _roleRepositoryMock.Setup(x => x.LoadAsync(123)).ReturnsAsync(role);
        _roleRepositoryMock.Setup(x => x.IsRoleNameConflictAsync(123, "NewName")).ReturnsAsync(false);
        _roleRepositoryMock.Setup(x => x.IsDisplayNameConflictAsync(123, "NewDisplay")).ReturnsAsync(false);

        var command = new ModifyRoleBasicCommand { RoleId = 123, RoleName = "NewName", DisplayName = "NewDisplay" };

        // Act
        var result = await _modifyHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        Assert.That(role.RoleName, Is.EqualTo("NewName"));
        _roleRepositoryMock.Verify(x => x.SaveAsync(role), Times.Once);
    }

    [Test]
    public async Task Modify_WhenNotFound_ShouldReturnFail()
    {
        // Arrange
        _roleRepositoryMock.Setup(x => x.LoadAsync(123)).ReturnsAsync((Role)null);
        var command = new ModifyRoleBasicCommand { RoleId = 123, RoleName = "NewName", DisplayName = "NewDisplay" };

        // Act
        var result = await _modifyHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Code, Is.EqualTo("Role.NotFound"));
    }

    [Test]
    public async Task Destroy_WhenValid_ShouldCallDestroyAndSave()
    {
        // Arrange
        var id = new AggregateId(123, false);
        var role = new Role(id, "Admin", "管理员", "描述");
        _roleRepositoryMock.Setup(x => x.LoadAsync(123)).ReturnsAsync(role);

        var command = new DestroyRoleCommand(123);

        // Act
        var result = await _destroyHandler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        _roleRepositoryMock.Verify(x => x.SaveAsync(role), Times.Once);
        // Verify event was added (indirectly via aggregator or outbox call)
        _outboxRepositoryMock.Verify(x => x.AddAsync(It.IsAny<IReadOnlyList<Renligou.Core.Shared.Events.IIntegrationEvent>>(), "DOMAIN", "Role", "123"), Times.Once);
    }
}
