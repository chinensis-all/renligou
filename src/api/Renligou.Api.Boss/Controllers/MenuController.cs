using Microsoft.AspNetCore.Mvc;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;

namespace Renligou.Api.Boss.Controllers;

/// <summary>
/// Boss 菜单管理
/// </summary>
[ApiController]
[Route("menus")]
public class MenuController(
    ICommandBus _commandBus,
    IQueryBus _queryBus,
    IUnitOfWork _uow
) : ControllerBase
{
    /// <summary>
    /// 创建菜单
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>操作结果</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMenuRequest request)
    {
        var command = new CreateMenuCommand
        {
            ParentId = request.ParentId,
            MenuName = request.MenuName,
            MenuTag = request.MenuTag,
            Path = request.Path,
            Component = request.Component,
            Icon = request.Icon,
            Sorter = request.Sorter,
            IsHidden = request.IsHidden,
            PermitButtons = request.PermitButtons
        };

        var result = await _uow.ExecuteAsync(async () => await _commandBus.SendAsync<CreateMenuCommand, Result>(command));

        if (result == null || !result.Success) return BadRequest(result?.Error);
        return Ok();
    }

    /// <summary>
    /// 修改菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <param name="request">修改请求</param>
    /// <returns>操作结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Modify(long id, [FromBody] ModifyMenuRequest request)
    {
        var command = new ModifyMenuCommand
        {
            Id = id,
            ParentId = request.ParentId,
            MenuName = request.MenuName,
            MenuTag = request.MenuTag,
            Path = request.Path,
            Component = request.Component,
            Icon = request.Icon,
            Sorter = request.Sorter,
            PermitButtons = request.PermitButtons
        };

        var result = await _uow.ExecuteAsync(async () => await _commandBus.SendAsync<ModifyMenuCommand, Result>(command));

        if (result == null || !result.Success)
        {
            if (result?.Error?.Code == "Menu.NotFound") return NotFound(result.Error);
            return BadRequest(result?.Error);
        }
        return Ok();
    }

    /// <summary>
    /// 删除菜单
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>操作结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var command = new DestroyMenuCommand(id);
        var result = await _uow.ExecuteAsync(async () => await _commandBus.SendAsync<DestroyMenuCommand, Result>(command));

        if (result == null || !result.Success)
        {
            if (result?.Error?.Code == "Menu.NotFound") return NotFound(result.Error);
            return BadRequest(result?.Error);
        }
        return Ok();
    }

    /// <summary>
    /// 获取菜单详情
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <returns>详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var result = await _queryBus.QueryAsync<GetMenuDetailQuery, Result<MenuDetailDto?>>(new GetMenuDetailQuery(id));
        if (result.Value == null) return NotFound();
        return Ok(result.Value);
    }

    /// <summary>
    /// 获取菜单树
    /// </summary>
    /// <param name="parentId">父级ID (默认0)</param>
    /// <returns>树结构</returns>
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree([FromQuery] long parentId = 0)
    {
        var result = await _queryBus.QueryAsync<GetMenuTreeQuery, Result<List<MenuTreeNodeDto>>>(new GetMenuTreeQuery(parentId));
        return Ok(result.Value);
    }

    /// <summary>
    /// 修改菜单显示状态
    /// </summary>
    /// <param name="id">菜单ID</param>
    /// <param name="request">可见性请求</param>
    /// <returns>操作结果</returns>
    [HttpPatch("{id}/visibility")]
    public async Task<IActionResult> ChangeMenuVisibility(long id, [FromBody] ChangeMenuVisibilityRequest request)
    {
        var command = new ChangeMenuVisibilityCommand(id, request.IsHidden);
        var result = await _uow.ExecuteAsync(async () => await _commandBus.SendAsync<ChangeMenuVisibilityCommand, Result>(command));

        if (result == null || !result.Success)
        {
            if (result?.Error?.Code == "Menu.NotFound") return NotFound(result.Error);
            return BadRequest(result?.Error);
        }
        return Ok();
    }
}
