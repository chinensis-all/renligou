using Microsoft.AspNetCore.Mvc;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;

namespace Renligou.Api.Boss.Controllers
{
    /// <summary>
    /// 权限组管理
    /// </summary>
    [ApiController]
    [Route("/permission-groups")]
    public class PermissionGroupsController(
        ICommandBus _commandBus,
        IQueryBus _queryBus,
        IUnitOfWork _uow,
        ILogger<PermissionGroupsController> _logger
    ) : Controller
    {
        /// <summary>
        /// 创建权限组
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePermissionGroupRequest request, CancellationToken cancellationToken = default)
        {
            var command = new CreatePermissionGroupCommand
            {
                GroupName = request.GroupName,
                DisplayName = request.DisplayName,
                Description = request.Description
            };

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<CreatePermissionGroupCommand, Result>(command, cancellationToken);
            }, true);

            if (!res.Success)
                return BadRequest(res.Error);

            return Ok();
        }

        /// <summary>
        /// 修改权限组
        /// </summary>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Modify(
            [FromRoute] long id,
            [FromBody] ModifyPermissionGroupRequest request,
            CancellationToken cancellationToken = default
        )
        {
            var command = new ModifyPermissionGroupCommand
            {
                Id = id,
                GroupName = request.GroupName,
                DisplayName = request.DisplayName,
                Description = request.Description
            };

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<ModifyPermissionGroupCommand, Result>(command, cancellationToken);
            }, true);

            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok();
        }

        /// <summary>
        /// 删除权限组
        /// </summary>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(
            [FromRoute] long id,
            CancellationToken cancellationToken = default
        )
        {
            var command = new DeletePermissionGroupCommand(id);

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<DeletePermissionGroupCommand, Result>(command, cancellationToken);
            }, true);

            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok();
        }

        /// <summary>
        /// 获取权限组详情
        /// </summary>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetDetail(
            [FromRoute] long id,
            CancellationToken cancellationToken = default
        )
        {
            var query = new GetPermissionGroupDetailQuery(id);
            var res = await _queryBus.QueryAsync<GetPermissionGroupDetailQuery, Result<PermissionGroupDetailDto?>>(query, cancellationToken);
            
            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok(res.Value);
        }

        /// <summary>
        /// 获取权限组列表
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetList(
            [FromQuery] GetPermissionGroupListRequest request,
            CancellationToken cancellationToken = default
        )
        {
            var query = new GetPermissionGroupListQuery(
                request.GroupName,
                request.DisplayName
            );

            var res = await _queryBus.QueryAsync<GetPermissionGroupListQuery, Result<List<PermissionGroupListDto>>>(query, cancellationToken);
            
            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok(res.Value);
        }

        /// <summary>
        /// 获取权限组分页
        /// </summary>
        [HttpGet("pagination")]
        public async Task<IActionResult> GetPage(
            [FromQuery] GetPermissionGroupPageRequest request,
            CancellationToken cancellationToken = default
        )
        {
            var query = new GetPermissionGroupPageQuery(
                request.GroupName,
                request.DisplayName,
                request.Page,
                request.PageSize
            );

            var res = await _queryBus.QueryAsync<GetPermissionGroupPageQuery, Result<Pagination<PermissionGroupDetailDto>>>(query, cancellationToken);

            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok(res.Value);
        }
    }
}
