using Microsoft.AspNetCore.Http;
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
    public class PermissionGroupController(
        ICommandBus _commandBus,
        IQueryBus _queryBus,
        IUnitOfWork _uow,
        ILogger<PermissionGroupController> _logger
    ) : Controller
    {
        /// <summary>
        /// 创建权限组
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePermissionGroupRequest request, CancellationToken cancellationToken = default)
        {
            var command = new CreatePermissionGroupCommand
            {
                GroupName = request.GroupName,
                DisplayName = request.DisplayName,
                Description = request.Description,
                ParentId = request.ParentId,
                Sorter = request.Sorter
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
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
                Description = request.Description,
                ParentId = request.ParentId,
                Sorter = request.Sorter
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Destroy(
            [FromRoute] long id,
            CancellationToken cancellationToken = default
        )
        {
            var command = new DestroyPermissionGroupCommand(id);

            var res = await _uow.ExecuteAsync<Result>(async () =>
            {
                return await _commandBus.SendAsync<DestroyPermissionGroupCommand, Result>(command, cancellationToken);
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
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{id:long}")]
        [ProducesResponseType(typeof(PermissionGroupDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(typeof(List<PermissionGroupListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
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

        /*[HttpGet("pagination")]
        [ProducesResponseType(typeof(Pagination<PermissionGroupDetailDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
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
        }*/

        /// <summary>
        /// 获取权限组树
        /// </summary>
        /// <param name="parentId">父权限组ID（0表示顶级）</param>
        /// <param name="name">权限组名称/显示名称（模糊匹配）</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>树形结构列表</returns>
        [HttpGet("tree")]
        [ProducesResponseType(typeof(List<PermissionGroupTreeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTreeAsync(
            [FromQuery] long parentId = 0,
            [FromQuery] string? name = null,
            CancellationToken cancellationToken = default
        )
        {
            var query = new GetPermissionGroupTreeQuery
            {
                ParentId = parentId,
                Name = name
            };

            // 关键路径性能优先：权限组数据量通常较小，在内存中构建树结构
            var res = await _queryBus.QueryAsync<GetPermissionGroupTreeQuery, Result<List<PermissionGroupTreeDto>>>(query, cancellationToken);

            if (!res.Success)
            {
                return BadRequest(res.Error);
            }

            return Ok(res.Value);
        }
    }
}
