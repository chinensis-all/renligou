using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.AuthorizationContext.Value;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

public class CreateDepartmentHandler(
    IDepartmentRepository _departmentRepository,
    IOutboxRepository _outboxRepository,
    IIdGenerator _idGenerator
) : ICommandHandler<CreateDepartmentCommand, Result>
{
    public async Task<Result> HandleAsync(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validation = command.Validate();
        if (!validation.Success) return validation;

        // Check uniqueness
        if (await _departmentRepository.IsCompanyDeptNameConflictAsync(command.CompanyId, command.ParentId, command.DeptName, cancellationToken))
        {
            return Result.Fail("Department.Create.Error", "同级部门下已存在相同部门名称");
        }

        // Check Parent existence if ParentId > 0
        if (command.ParentId > 0)
        {
             if (!await _departmentRepository.ExistsAsync(command.ParentId, cancellationToken))
             {
                 return Result.Fail("Department.Create.Error", "上级部门不存在");
             }
        }

        var id = _idGenerator.NextId();

        var department = new Department(
            new AggregateId(id, true),
            command.ParentId,
            command.CompanyId,
            command.DeptName,
            command.DeptCode,
            command.Description,
            command.Sorter,
            DepartmentStatus.Active
        );

        department.Create();

        await _departmentRepository.SaveAsync(department);
        await _outboxRepository.AddAsync(department.GetRegisteredEvents(), "DOMAIN", department.GetType().Name, department.Id.id.ToString());

        return Result.Ok();
    }
}
