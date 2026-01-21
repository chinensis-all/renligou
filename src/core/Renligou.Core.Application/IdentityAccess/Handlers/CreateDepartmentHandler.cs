using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 创建部门处理器
/// </summary>
public sealed class CreateDepartmentHandler(
    IDepartmentRepository _departmentRepository,
    IOutboxRepository _outboxRepository,
    IIdGenerator _idGenerator
) : ICommandHandler<CreateDepartmentCommand, Result>
{
    public async Task<Result> HandleAsync(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validation = command.Validate();
        if (!validation.Success) return validation;

        // 检查父部门是否存在
        if (command.ParentId != 0)
        {
            if (!await _departmentRepository.ExistsAsync(command.ParentId, cancellationToken))
            {
                return Result.Fail("Department.Create.Error", $"上级部门不存在: {command.ParentId}");
            }
        }

        // 检查名称冲突
        if (await _departmentRepository.IsDeptNameConflictAsync(command.CompanyId, command.ParentId, command.DeptName, null, cancellationToken))
        {
            return Result.Fail("Department.Create.Error", "部门名称在同一级下已存在");
        }

        var id = _idGenerator.NextId();
        var department = new Department(
            new AggregateId(id, true),
            command.ParentId,
            command.CompanyId,
            command.DeptName,
            command.DeptCode,
            command.Description,
            command.Sorter
        );

        department.Create();

        await _departmentRepository.SaveAsync(department);
        await _outboxRepository.AddAsync(department.GetRegisteredEvents(), "DOMAIN", department.GetType().Name, department.Id.id.ToString());

        return Result.Ok();
    }
}
