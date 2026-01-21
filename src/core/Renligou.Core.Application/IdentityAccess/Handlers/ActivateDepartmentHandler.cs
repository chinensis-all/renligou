using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers;

/// <summary>
/// 启用部门处理器
/// </summary>
public sealed class ActivateDepartmentHandler(
    IDepartmentRepository _departmentRepository,
    IOutboxRepository _outboxRepository
) : ICommandHandler<ActivateDepartmentCommand, Result>
{
    public async Task<Result> HandleAsync(ActivateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var validation = command.Validate();
        if (!validation.Success) return validation;

        var department = await _departmentRepository.LoadAsync(command.Id);
        if (department == null)
        {
            return Result.Fail("Department.NotFound", $"没有找到部门: {command.Id}");
        }

        department.Activate();

        await _departmentRepository.SaveAsync(department);
        await _outboxRepository.AddAsync(department.GetRegisteredEvents(), "DOMAIN", department.GetType().Name, department.Id.id.ToString());

        return Result.Ok();
    }
}
