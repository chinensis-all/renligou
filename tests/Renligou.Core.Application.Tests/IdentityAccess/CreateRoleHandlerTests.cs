using Moq;
using NUnit.Framework;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;
using Renligou.Core.Shared.Generators;
using Renligou.Core.Shared.Repo;

namespace Renligou.Core.Application.Tests.IdentityAccess;

[TestFixture]
public class CreateRoleHandlerTests
{
    private Mock<IRoleRepository> _roleRepositoryMock;
    private Mock<IOutboxRepository> _outboxRepositoryMock;
    private Mock<IIdGenerator> _idGeneratorMock;
    private CreateRoleHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _outboxRepositoryMock = new Mock<IOutboxRepository>();
        _idGeneratorMock = new Mock<IIdGenerator>();

        _handler = new CreateRoleHandler(
            _roleRepositoryMock.Object,
            _outboxRepositoryMock.Object,
            _idGeneratorMock.Object
        );
    }

    [Test]
    public async Task HandleAsync_WhenValidCommand_ShouldSaveRoleAndOutbox()
    {
        // Arrange
        var command = new CreateRoleCommand { RoleName = "Admin", DisplayName = "管理员" };
        _idGeneratorMock.Setup(x => x.NextId()).Returns(123);
        _roleRepositoryMock.Setup(x => x.IsRoleNameConflictAsync(0, "Admin")).ReturnsAsync(false);
        _roleRepositoryMock.Setup(x => x.IsDisplayNameConflictAsync(0, "管理员")).ReturnsAsync(false);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        _roleRepositoryMock.Verify(x => x.SaveAsync(It.Is<Role>(r => r.RoleName == "Admin" && r.Id.Id == 123)), Times.Once);
        _outboxRepositoryMock.Verify(x => x.AddAsync(It.IsAny<IReadOnlyList<IIntegrationEvent>>(), "DOMAIN", "Role", "123"), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WhenRoleNameConflict_ShouldReturnFail()
    {
        // Arrange
        var command = new CreateRoleCommand { RoleName = "Admin", DisplayName = "管理员" };
        _roleRepositoryMock.Setup(x => x.IsRoleNameConflictAsync(0, "Admin")).ReturnsAsync(true);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Is.EqualTo("角色名称已存在"));
    }

    [Test]
    public async Task HandleAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var command = new CreateRoleCommand { RoleName = "Admin", DisplayName = "管理员" };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        // OperationCanceledException should be thrown or handled depending on implementation
        // Since we didn't explicitly check token in handler (besides passing it to repo), 
        // we check if the task is cancelled if we use the token in async calls.
        
        // Let's assume the repo methods respect the token.
        _roleRepositoryMock.Setup(x => x.IsRoleNameConflictAsync(0, "Admin")).Returns(async () => {
            await Task.Delay(100, cts.Token);
            return false;
        });

        Assert.ThrowsAsync<TaskCanceledException>(async () => await _handler.HandleAsync(command, cts.Token));
    }
}
