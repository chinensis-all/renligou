using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.Events;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public class CreatePermissionGroupHandler(
        IPermissionGroupRepository _permissionGroupRepository,
        IOutboxRepository _outboxRepository,
        IIdGenerator _idGenerator
    ) : ICommandHandler<CreatePermissionGroupCommand, Result>
    {
        public async Task<Result> HandleAsync(CreatePermissionGroupCommand command, CancellationToken cancellationToken)
        {
            var validation = command.Validate();
            if (!validation.Success)
            {
                return validation;
            }

            long id = _idGenerator.NextId();

            if (await _permissionGroupRepository.IsGroupNameConflictAsync(id, command.GroupName))
            {
                return Result.Fail("PermissionGroup.Create.Error", "权限组名称已存在");
            }

            if (await _permissionGroupRepository.IsDisplayNameConflictAsync(id, command.DisplayName))
            {
                return Result.Fail("PermissionGroup.Create.Error", "权限组显示名称已存在");
            }

            var permissionGroup = new PermissionGroup(
                new AggregateId(id, true),
                command.GroupName,
                command.DisplayName,
                command.Description
            );

            permissionGroup.Create();

            await _permissionGroupRepository.SaveAsync(permissionGroup);
            await _outboxRepository.AddAsync(permissionGroup.GetRegisteredEvents(), "DOMAIN", nameof(PermissionGroup), permissionGroup.Id.id.ToString());

            return Result.Ok();
        }
    }
}
