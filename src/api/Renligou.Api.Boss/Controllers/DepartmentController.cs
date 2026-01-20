using Microsoft.AspNetCore.Mvc;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.IdentityAccess.Commands;
using Renligou.Core.Application.IdentityAccess.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;

namespace Renligou.Api.Boss.Controllers;

/// <summary>
/// 部门管理
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
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

        if (!res.Success) return BadRequest(res.Error);
        return Ok();
    }

    /// <summary>
    /// 修改部门基础信息
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
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
            CompanyId = request.CompanyId,
            DeptName = request.DeptName,
            DeptCode = request.DeptCode,
            Description = request.Description
        };

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<ModifyDepartmentBasicCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success) return BadRequest(res.Error);
        return Ok();
    }

    /// <summary>
    /// 修改部门排序
    /// </summary>
    [HttpPut("{id:long}/put")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ModifyDepartmentSorter(
        [FromRoute] long id,
        [FromBody] ModifyDepartmentSorterRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var command = new ModifyDepartmentSorterCommand
        {
            Id = id,
            Sorter = request.Sorter
        };

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<ModifyDepartmentSorterCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success) return BadRequest(res.Error);
        return Ok();
    }

    /// <summary>
    /// 禁用部门
    /// </summary>
    [HttpPost("{id:long}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InactiveDepartment(
        [FromRoute] long id,
        CancellationToken cancellationToken = default
    )
    {
        var command = new InactiveDepartmentCommand { Id = id };

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<InactiveDepartmentCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success) return BadRequest(res.Error);
        return Ok();
    }

    /// <summary>
    /// 启用部门
    /// </summary>
    [HttpDelete("{id:long}/lock")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ActivateDepartment(
        [FromRoute] long id,
        CancellationToken cancellationToken = default
    )
    {
        var command = new ActivateDepartmentCommand { Id = id };

        var res = await _uow.ExecuteAsync<Result>(async () =>
        {
            return await _commandBus.SendAsync<ActivateDepartmentCommand, Result>(command, cancellationToken);
        }, true);

        if (!res.Success) return BadRequest(res.Error);
        return Ok();
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(DepartmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDetail(
        [FromRoute] long id,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetDepartmentDetailQuery(id);
        var res = await _queryBus.QueryAsync<GetDepartmentDetailQuery, Result<DepartmentDetailDto?>>(query, cancellationToken);
        if (!res.Success) return BadRequest(res.Error);
        return Ok(res.Value);
    }

    /// <summary>
    /// 获取部门列表
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<DepartmentListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetList(
        [FromQuery] GetDepartmentListRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetDepartmentListQuery(request.CompanyId, request.ParentId, request.DeptName);
        var res = await _queryBus.QueryAsync<GetDepartmentListQuery, Result<List<DepartmentListDto>>>(query, cancellationToken);
        if (!res.Success) return BadRequest(res.Error);
        return Ok(res.Value);
    }

    /// <summary>
    /// 获取部门分页
    /// </summary>
    [HttpGet("pagination")]
    [ProducesResponseType(typeof(Pagination<DepartmentDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Error), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPage(
        [FromQuery] GetDepartmentPageRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var query = new GetDepartmentPageQuery(request.PageIndex, request.PageSize, request.CompanyId, request.ParentId, request.DeptName);
        var res = await _queryBus.QueryAsync<GetDepartmentPageQuery, Result<Pagination<DepartmentDetailDto>>>(query, cancellationToken);
        if (!res.Success) return BadRequest(res.Error);
        return Ok(res.Value);
    }
}
