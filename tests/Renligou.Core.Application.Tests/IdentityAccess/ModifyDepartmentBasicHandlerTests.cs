using Moq;
using NUnit.Framework;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.Tests.IdentityAccess;

[TestFixture]
public class ModifyDepartmentBasicHandlerTests
{
    private Mock<IDepartmentRepository> _departmentRepositoryMock;
    private Mock<IOutboxRepository> _outboxRepositoryMock;
    private ModifyDepartmentBasicHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _departmentRepositoryMock = new Mock<IDepartmentRepository>();
        _outboxRepositoryMock = new Mock<IOutboxRepository>();

        _handler = new ModifyDepartmentBasicHandler(
            _departmentRepositoryMock.Object,
            _outboxRepositoryMock.Object
        );
    }

    [Test]
    public async Task HandleAsync_WhenParentIdChangedAndNotExist_ShouldReturnFail()
    {
        // Arrange
        var id = 1001;
        var existingDept = (Department)Activator.CreateInstance(typeof(Department), true)!;
        typeof(Department).GetProperty(nameof(Department.Id))!.SetValue(existingDept, new AggregateId(id, true));
        typeof(Department).GetProperty(nameof(Department.ParentId))!.SetValue(existingDept, 0L);

        _departmentRepositoryMock.Setup(x => x.LoadAsync(id)).ReturnsAsync(existingDept);
        _departmentRepositoryMock.Setup(x => x.ExistsAsync(2002, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var command = new ModifyDepartmentBasicCommand
        {
            Id = id,
            ParentId = 2002,
            DeptName = "New Name"
        };

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error.Message, Contains.Substring("上级部门不存在"));
    }
}
