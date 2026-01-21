using Moq;
using NUnit.Framework;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.Tests.IdentityAccess;

[TestFixture]
public class CreateDepartmentHandlerTests
{
    private Mock<IDepartmentRepository> _departmentRepositoryMock;
    private Mock<IOutboxRepository> _outboxRepositoryMock;
    private Mock<IIdGenerator> _idGeneratorMock;
    private CreateDepartmentHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _departmentRepositoryMock = new Mock<IDepartmentRepository>();
        _outboxRepositoryMock = new Mock<IOutboxRepository>();
        _idGeneratorMock = new Mock<IIdGenerator>();

        _handler = new CreateDepartmentHandler(
            _departmentRepositoryMock.Object,
            _outboxRepositoryMock.Object,
            _idGeneratorMock.Object
        );
    }

    [Test]
    public async Task HandleAsync_WhenValidCommand_ShouldSaveDepartmentAndOutbox()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            CompanyId = 1,
            ParentId = 0,
            DeptName = "IT",
            DeptCode = "IT01",
            Description = "IT Dept",
            Sorter = 1
        };

        _idGeneratorMock.Setup(x => x.NextId()).Returns(1001);
        _departmentRepositoryMock.Setup(x => x.IsDeptNameConflictAsync(1, 0, "IT", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        _departmentRepositoryMock.Verify(x => x.SaveAsync(It.Is<Department>(d => d.DeptName == "IT" && d.Id.id == 1001)), Times.Once);
        _outboxRepositoryMock.Verify(x => x.AddAsync(It.IsAny<IReadOnlyList<IIntegrationEvent>>(), "DOMAIN", "Department", "1001"), Times.Once);
    }

    [Test]
    public async Task HandleAsync_WhenParentIdNotZeroAndNotExist_ShouldReturnFail()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            CompanyId = 1,
            ParentId = 999,
            DeptName = "IT",
            DeptCode = "IT01"
        };

        _departmentRepositoryMock.Setup(x => x.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Contains.Substring("上级部门不存在"));
    }

    [Test]
    public async Task HandleAsync_WhenNameConflict_ShouldReturnFail()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            CompanyId = 1,
            ParentId = 0,
            DeptName = "IT"
        };

        _departmentRepositoryMock.Setup(x => x.IsDeptNameConflictAsync(1, 0, "IT", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Is.EqualTo("部门名称在同一级下已存在"));
    }
}
