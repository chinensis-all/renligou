using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

public class ModifyDepartmentBasicHandler(
    IDepartmentRepository _departmentRepository,
    IOutboxRepository _outboxRepository
) : ICommandHandler<ModifyDepartmentBasicCommand, Result>
{
    public async Task<Result> HandleAsync(ModifyDepartmentBasicCommand command, CancellationToken cancellationToken)
    {
        var validation = command.Validate();
        if (!validation.Success) return validation;

        var department = await _departmentRepository.LoadAsync(command.Id);
        if (department == null)
            return Result.Fail("Department.NotFound", "部门不存在");

        // Check Parent existence if ParentId > 0 and changed
        if (command.ParentId > 0 && command.ParentId != department.ParentId)
        {
             if (!await _departmentRepository.ExistsAsync(command.ParentId, cancellationToken))
             {
                 return Result.Fail("Department.Modify.Error", "上级部门不存在");
             }
        }

        // Conflict check
        if (command.DeptName != department.DeptName || command.ParentId != department.ParentId || command.CompanyId != department.CompanyId)
        {
             if (await _departmentRepository.IsCompanyDeptNameConflictAsync(command.CompanyId, command.ParentId, command.DeptName, cancellationToken))
             {
                 return Result.Fail("Department.Modify.Error", "同级部门下已存在相同部门名称");
             }
        }

        department.ModifyBasic(
            command.ParentId,
            command.CompanyId,
            command.DeptName,
            command.DeptCode,
            command.Description
        );

        await _departmentRepository.SaveAsync(department);
        await _outboxRepository.AddAsync(department.GetRegisteredEvents(), "DOMAIN", department.GetType().Name, department.Id.id.ToString());

        return Result.Ok();
    }
}
