using Moq;
using NUnit.Framework;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.AuthorizationContext.Value;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.Tests.IdentityAccess;

[TestFixture]
public class DepartmentHandlerTests
{
    private Mock<IDepartmentRepository> _repoMock;
    private Mock<IOutboxRepository> _outboxMock;
    private Mock<IIdGenerator> _idGenMock;

    [SetUp]
    public void Setup()
    {
        _repoMock = new Mock<IDepartmentRepository>();
        _outboxMock = new Mock<IOutboxRepository>();
        _idGenMock = new Mock<IIdGenerator>();
    }

    [Test]
    public async Task Create_ValidCommand_ShouldSucceed()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            CompanyId = 1,
            DeptName = "Test",
            ParentId = 0
        };
        _idGenMock.Setup(x => x.NextId()).Returns(100);
        _repoMock.Setup(x => x.IsCompanyDeptNameConflictAsync(1, 0, "Test", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new CreateDepartmentHandler(_repoMock.Object, _outboxMock.Object, _idGenMock.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Success);
        _repoMock.Verify(x => x.SaveAsync(It.IsAny<Department>()), Times.Once);
        _outboxMock.Verify(x => x.AddAsync(It.IsAny<IReadOnlyList<IIntegrationEvent>>(), "DOMAIN", "Department", "100"), Times.Once);
    }

    [Test]
    public async Task Create_DuplicateName_ShouldFail()
    {
        // Arrange
        var command = new CreateDepartmentCommand
        {
            CompanyId = 1,
            DeptName = "Test",
            ParentId = 0
        };
        _repoMock.Setup(x => x.IsCompanyDeptNameConflictAsync(1, 0, "Test", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateDepartmentHandler(_repoMock.Object, _outboxMock.Object, _idGenMock.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.That(result.Error.Message, Is.EqualTo("同级部门下已存在相同部门名称"));
        _repoMock.Verify(x => x.SaveAsync(It.IsAny<Department>()), Times.Never);
    }

    [Test]
    public async Task Modify_ValidCommand_ShouldSucceed()
    {
        var command = new ModifyDepartmentBasicCommand
        {
            Id = 100,
            CompanyId = 1,
            DeptName = "NewName",
            ParentId = 0
        };

        var dept = new Department(new AggregateId(100, true), 0, 1, "OldName", "", "", 1, DepartmentStatus.Active);

        _repoMock.Setup(x => x.LoadAsync(100)).ReturnsAsync(dept);
        _repoMock.Setup(x => x.IsCompanyDeptNameConflictAsync(1, 0, "NewName", It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var handler = new ModifyDepartmentBasicHandler(_repoMock.Object, _outboxMock.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.Success);
        _repoMock.Verify(x => x.SaveAsync(It.Is<Department>(d => d.DeptName == "NewName")), Times.Once);
    }
}
