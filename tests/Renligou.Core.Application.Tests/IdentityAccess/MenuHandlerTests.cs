using Moq;
using NUnit.Framework;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Model;
using Renligou.Core.Domain.IdentityAccess.UiAccessContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.Tests.IdentityAccess;

[TestFixture]
public class MenuHandlerTests
{
    private Mock<IMenuRepository> _menuRepositoryMock;
    private Mock<IOutboxRepository> _outboxRepositoryMock;
    private Mock<IIdGenerator> _idGeneratorMock;
    
    [SetUp]
    public void SetUp()
    {
        _menuRepositoryMock = new Mock<IMenuRepository>();
        _outboxRepositoryMock = new Mock<IOutboxRepository>();
        _idGeneratorMock = new Mock<IIdGenerator>();
    }

    [Test]
    public async Task CreateMenu_NormalDispatch_ShouldSucceed()
    {
        // Arrange
        var handler = new CreateMenuHandler(_menuRepositoryMock.Object, _idGeneratorMock.Object, _outboxRepositoryMock.Object);
        var command = new CreateMenuCommand { MenuName = "Test", MenuTag = "T1" };
        
        _idGeneratorMock.Setup(x => x.NextId()).Returns(100);
        _menuRepositoryMock.Setup(x => x.IsNameTagConflictAsync("Test", "T1", null, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        _menuRepositoryMock.Verify(x => x.SaveAsync(It.Is<Menu>(m => m.Id.id == 100)), Times.Once);
    }

    [Test]
    public async Task CreateMenu_CancellationTokenPropagation_ShouldPassToRepo()
    {
        // Arrange
        var handler = new CreateMenuHandler(_menuRepositoryMock.Object, _idGeneratorMock.Object, _outboxRepositoryMock.Object);
        var command = new CreateMenuCommand { MenuName = "Test", MenuTag = "T1" };
        using var cts = new CancellationTokenSource();
        
        // Act
        await handler.HandleAsync(command, cts.Token);

        // Assert
        _menuRepositoryMock.Verify(x => x.IsNameTagConflictAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long?>(), cts.Token), Times.Once);
    }

    [Test]
    public async Task ChangeVisibility_WhenMenuNotFound_ShouldReturnFail()
    {
        // Arrange
        var handler = new ChangeMenuVisibilityHandler(_menuRepositoryMock.Object, _outboxRepositoryMock.Object);
        var command = new ChangeMenuVisibilityCommand(123, true);
        _menuRepositoryMock.Setup(x => x.LoadAsync(123)).ReturnsAsync((Menu)null);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Is.EqualTo("菜单不存在"));
    }

    [Test]
    public async Task CreateMenu_NameTagConflict_ShouldReturnFail()
    {
        // Arrange
        var handler = new CreateMenuHandler(_menuRepositoryMock.Object, _idGeneratorMock.Object, _outboxRepositoryMock.Object);
        var command = new CreateMenuCommand { MenuName = "Test", MenuTag = "T1" };
        _menuRepositoryMock.Setup(x => x.IsNameTagConflictAsync("Test", "T1", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Contains.Substring("已存在"));
    }
}
