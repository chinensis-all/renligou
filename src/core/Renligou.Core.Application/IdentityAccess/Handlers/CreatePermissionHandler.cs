using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Domain.AuthorizationContext.Model;
using Renligou.Core.Domain.AuthorizationContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Commanding;
using Renligou.Core.Shared.Common;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Core.Application.IdentityAccess.Handlers
{
    public class CreatePermissionHandler(
        IPermissionRepository _permissionRepository,
        IPermissionGroupRepository _permissionGroupRepository,
        IOutboxRepository _outboxRepository,
        IIdGenerator _idGenerator
    ) : ICommandHandler<CreatePermissionCommand, Result>
    {
        public async Task<Result> HandleAsync(CreatePermissionCommand command, CancellationToken cancellationToken)
        {
            var validation = command.Validate();
            if (!validation.Success) return validation;

            // 检查权限组是否存在
            if (!await _permissionGroupRepository.ExistsAsync(command.GroupId, cancellationToken))
            {
                return Result.Fail("Permission.Create.Error", $"权限组 {command.GroupId} 不存在");
            }

            long id = _idGenerator.NextId();

            if (await _permissionRepository.IsPermissionNameConflictAsync(id, command.PermissionName))
            {
                return Result.Fail("Permission.Create.Error", "权限标识已存在");
            }

            if (await _permissionRepository.IsDisplayNameConflictAsync(id, command.DisplayName))
            {
                return Result.Fail("Permission.Create.Error", "权限名称已存在");
            }

            var permission = new Permission(
                new AggregateId(id, true),
                command.GroupId,
                command.PermissionName,
                command.DisplayName,
                command.Description
            );
            permission.Create();

            await _permissionRepository.SaveAsync(permission);
            await _outboxRepository.AddAsync(permission.GetRegisteredEvents(), "DOMAIN", nameof(Permission), permission.Id.id.ToString());

            return Result.Ok();
        }
    }
}
