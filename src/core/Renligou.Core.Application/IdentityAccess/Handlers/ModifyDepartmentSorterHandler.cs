using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

public class ModifyDepartmentSorterHandler(
    IDepartmentRepository _departmentRepository,
    IOutboxRepository _outboxRepository
) : ICommandHandler<ModifyDepartmentSorterCommand, Result>
{
    public async Task<Result> HandleAsync(ModifyDepartmentSorterCommand command, CancellationToken cancellationToken)
    {
        var validation = command.Validate();
        if (!validation.Success) return validation;

        var department = await _departmentRepository.LoadAsync(command.Id);
        if (department == null)
            return Result.Fail("Department.NotFound", "部门不存在");

        department.ModifySorter(command.Sorter);

        await _departmentRepository.SaveAsync(department);
        await _outboxRepository.AddAsync(department.GetRegisteredEvents(), "DOMAIN", department.GetType().Name, department.Id.id.ToString());

        return Result.Ok();
    }
}
