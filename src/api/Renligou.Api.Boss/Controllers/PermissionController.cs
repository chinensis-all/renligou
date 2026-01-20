using Microsoft.AspNetCore.Mvc;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Handlers;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;

namespace Renligou.Api.Boss.Controllers
{
    /// <summary>
    /// 权限管理
    /// </summary>
    [ApiController]
    [Route("/permissions")]
    public class PermissionController(
        ICommandBus _commandBus,
        IQueryBus _queryBus,
        IUnitOfWork _uow
    ) : Controller
    {
        /// <summary>
        /// 创建权限
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result), 200)]
        public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request, CancellationToken cancellationToken = default)
        {
            var command = new CreatePermissionCommand
            {
                GroupId = request.GroupId,
                PermissionName = request.PermissionName,
                DisplayName = request.DisplayName,
                Description = request.Description
            };

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<CreatePermissionCommand, Result>(command, cancellationToken);
            }, true);

            if (!res.Success)
                return BadRequest(res.Error);

            return Ok();
        }

        /// <summary>
        /// 修改权限
        /// </summary>
        [HttpPut("{id:long}")]
        [ProducesResponseType(typeof(Result), 200)]
        public async Task<IActionResult> Modify(
            [FromRoute] long id,
            [FromBody] ModifyPermissionRequest request,
            CancellationToken cancellationToken = default
        )
        {
            var command = new ModifyPermissionCommand
            {
                Id = id,
                GroupId = request.GroupId,
                PermissionName = request.PermissionName,
                DisplayName = request.DisplayName,
                Description = request.Description
            };

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<ModifyPermissionCommand, Result>(command, cancellationToken);
            }, true);

            if (!res.Success)
                return BadRequest(res.Error);

            return Ok();
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(typeof(Result), 200)]
        public async Task<IActionResult> Delete([FromRoute] long id, CancellationToken cancellationToken = default)
        {
            var command = new DestroyPermissionCommand(id);

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<DestroyPermissionCommand, Result>(command, cancellationToken);
            }, true);

            if (!res.Success)
                return BadRequest(res.Error);

            return Ok();
        }

        /// <summary>
        /// 获取权限详情
        /// </summary>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(Result<PermissionDetailDto>), 200)]
        public async Task<IActionResult> GetDetail([FromRoute] long id, CancellationToken cancellationToken = default)
        {
            var query = new GetPermissionDetailQuery(id);
            var res = await _queryBus.QueryAsync<GetPermissionDetailQuery, Result<PermissionDetailDto?>>(query, cancellationToken);

            if (!res.Success)
                return BadRequest(res.Error);

            return Ok(res.Value);
        }

        /// <summary>
        /// 获取权限列表
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Result<List<PermissionListDto>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] GetPermissionListRequest request, CancellationToken cancellationToken = default)
        {
            var query = new GetPermissionListQuery(
                request.GroupId,
                request.PermissionName,
                request.DisplayName
            );

            var res = await _queryBus.QueryAsync<GetPermissionListQuery, Result<List<PermissionListDto>>>(query, cancellationToken);

            if (!res.Success)
                return BadRequest(res.Error);

            return Ok(res.Value);
        }

        /// <summary>
        /// 获取权限分页
        /// </summary>
        [HttpGet("pagination")]
        [ProducesResponseType(typeof(Result<Pagination<PermissionDetailDto>>), 200)]
        public async Task<IActionResult> GetPage([FromQuery] GetPermissionPageRequest request, CancellationToken cancellationToken = default)
        {
            var query = new GetPermissionPageQuery(
                request.GroupId,
                request.PermissionName,
                request.DisplayName,
                request.Page,
                request.PageSize
            );

            var res = await _queryBus.QueryAsync<GetPermissionPageQuery, Result<Pagination<PermissionDetailDto>>>(query, cancellationToken);

            if (!res.Success)
                return BadRequest(res.Error);

            return Ok(res.Value);
        }
    }
}
