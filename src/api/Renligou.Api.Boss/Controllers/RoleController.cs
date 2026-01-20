using Microsoft.AspNetCore.Mvc;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;
using Renligou.Core.Application.Common.Queries;

namespace Renligou.Api.Boss.Controllers;

/// <summary>
/// 角色管理控制器
/// </summary>
[ApiController]
[Route("/roles")]
public class RoleController(
    ICommandBus _commandBus,
    IQueryBus _queryBus,
    IUnitOfWork _uow
) : Controller
{
    /// <summary>
    /// 创建角色
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateRoleCommand
        {
            RoleName = request.RoleName,
            DisplayName = request.DisplayName,
            Description = request.Description
        };

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<CreateRoleCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok();
    }

    /// <summary>
    /// 修改角色基础信息
    /// </summary>
    [HttpPut("{id:long}/basic")]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> ModifyBasic(
        [FromRoute] long id,
        [FromBody] ModifyRoleBasicRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var command = new ModifyRoleBasicCommand
        {
            RoleId = id,
            RoleName = request.RoleName,
            DisplayName = request.DisplayName,
            Description = request.Description
        };

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<ModifyRoleBasicCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok();
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(RoleDetailDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> GetDetail(
        [FromRoute] long id,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetRoleDetailQuery(id);
        var res = await _queryBus.QueryAsync<GetRoleDetailQuery, Result<RoleDetailDto?>>(query, cancellationToken);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok(res.Value);
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<RoleListDto>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> GetList(
        [FromQuery] GetRoleListRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetRoleListQuery(
            request.RoleName ?? request.DisplayName
        );

        var res = await _queryBus.QueryAsync<GetRoleListQuery, Result<List<RoleListDto>>>(query, cancellationToken);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok(res.Value);
    }

    /// <summary>
    /// 获取角色分页
    /// </summary>
    [HttpGet("pagination")]
    [ProducesResponseType(typeof(Pagination<RoleDetailDto>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> GetPage(
        [FromQuery] GetRolePageRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetRolePageQuery(
            request.Keyword,
            request.Page,
            request.PageSize
        );

        var res = await _queryBus.QueryAsync<GetRolePageQuery, Result<Pagination<RoleDetailDto>>>(query, cancellationToken);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok(res.Value);
    }

    /// <summary>
    /// 销毁(软删除)角色
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> Delete(
        [FromRoute] long id,
        CancellationToken cancellationToken = default
    )
    {
        var command = new DestroyRoleCommand(id);

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<DestroyRoleCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok();
    }
}
