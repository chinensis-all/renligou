using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;

namespace Renligou.Api.Boss.Controllers;

/// <summary>
/// 部门管理控制器
/// </summary>
[ApiController]
[Route("/departments")]
public class DepartmentController(
    ICommandBus _commandBus,
    IQueryBus _queryBus,
    IUnitOfWork _uow
) : Controller
{
    /// <summary>
    /// 创建部门
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateDepartmentCommand
        {
            ParentId = request.ParentId,
            CompanyId = request.CompanyId,
            DeptName = request.DeptName,
            DeptCode = request.DeptCode,
            Description = request.Description,
            Sorter = request.Sorter
        };

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<CreateDepartmentCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok();
    }

    /// <summary>
    /// 修改部门基础信息
    /// </summary>
    [HttpPut("{id:long}/basic")]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> ModifyBasic(
        [FromRoute] long id,
        [FromBody] ModifyDepartmentBasicRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var command = new ModifyDepartmentBasicCommand
        {
            Id = id,
            ParentId = request.ParentId,
            DeptName = request.DeptName,
            DeptCode = request.DeptCode,
            Description = request.Description,
            Sorter = request.Sorter
        };

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<ModifyDepartmentBasicCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok();
    }

    /// <summary>
    /// 禁用部门
    /// </summary>
    [HttpPost("{id:long}/lock")]
    public async Task<IActionResult> InactiveDepartment(
        [FromRoute] long id,
        CancellationToken cancellationToken = default
    )
    {
        var command = new InactiveDepartmentCommand(id);

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<InactiveDepartmentCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok();
    }

    /// <summary>
    /// 启用部门
    /// </summary>
    [HttpDelete("{id:long}/lock")]
    public async Task<IActionResult> ActivateDepartment(
        [FromRoute] long id,
        CancellationToken cancellationToken = default
    )
    {
        var command = new ActivateDepartmentCommand(id);

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<ActivateDepartmentCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok();
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DepartmentDetailDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> GetDetail(
        [FromRoute] long id,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetDepartmentDetailQuery(id);
        var res = await _queryBus.QueryAsync<GetDepartmentDetailQuery, Result<DepartmentDetailDto?>>(query, cancellationToken);

        if (!res.Success)
            return BadRequest(res.Error);

        return Ok(res.Value);
    }

    /// <summary>
    /// 获取部门树
    /// </summary>
    [HttpGet("tree")]
    [ProducesResponseType(typeof(List<DepartmentTreeNodeDto>), 200)]
    public async Task<IActionResult> GetTree(
        [FromQuery] GetDepartmentTreeRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetDepartmentTreeQuery
        {
            ParentId = request.ParentId,
            Name = request.Name,
            CompanyId = request.CompanyId
        };

        var res = await _queryBus.QueryAsync<GetDepartmentTreeQuery, List<DepartmentTreeNodeDto>>(query, cancellationToken);

        return Ok(res);
    }
}
