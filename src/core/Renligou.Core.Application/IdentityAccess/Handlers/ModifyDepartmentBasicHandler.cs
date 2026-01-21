using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 修改部门基础信息处理器
/// </summary>
public sealed class ModifyDepartmentBasicHandler(
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
        {
            return Result.Fail("Department.NotFound", $"没有找到部门: {command.Id}");
        }

        // 检查父部门是否存在 (如果发生了变更)
        if (command.ParentId != department.ParentId && command.ParentId != 0)
        {
            if (!await _departmentRepository.ExistsAsync(command.ParentId, cancellationToken))
            {
                return Result.Fail("Department.Modify.Error", $"上级部门不存在: {command.ParentId}");
            }
        }

        // 检查名称冲突
        if (await _departmentRepository.IsDeptNameConflictAsync(department.CompanyId, command.ParentId, command.DeptName, command.Id, cancellationToken))
        {
            return Result.Fail("Department.Modify.Error", "部门名称在同一级下已存在");
        }

        department.ModifyBasic(
            command.ParentId,
            command.DeptName,
            command.DeptCode,
            command.Description,
            command.Sorter
        );

        await _departmentRepository.SaveAsync(department);
        await _outboxRepository.AddAsync(department.GetRegisteredEvents(), "DOMAIN", department.GetType().Name, department.Id.id.ToString());

        return Result.Ok();
    }
}
