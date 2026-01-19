# 控制器 CRUD Skill

此 Skill 展示了如何实现标准的 CRUD（创建、读取、更新、删除）控制器。它遵循 CQRS 模式，通过 CommandBus 和 QueryBus 进行操作分发。

## 1. 定义

- [控制器名] : 通常对应领域聚合根名称，如 `CompanyController`。
- [路由前缀] : 通常为资源名的复数形式，如 `/companies`。
- [位置] : `src/api/Renligou.Api.Boss/Controllers/` (或其他对应的 API 项目)。
- [Request对象] : 定义在 `src/api/Renligou.Api.Boss/Requests/`，用于接收 HTTP 请求体或查询参数。

## 2. 控制器结构

控制器应继承自 `Microsoft.AspNetCore.Mvc.Controller`，并使用主构造函数注入必要的依赖。

### 依赖项
- `ICommandBus` : 用于发送命令。
- `IQueryBus` : 用于发送查询。
- `IUnitOfWork` : (仅命令操作需要) 用于管理事务。
- `ILogger<T>` : 用于日志记录。

### 示例结构
```csharp
using Microsoft.AspNetCore.Mvc;
using Renligou.Api.Boss.Requests;
using Renligou.Core.Application.Common.Queries;
using Renligou.Core.Application.Enterprise.Commands;
using Renligou.Core.Application.Enterprise.Queries;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.Ddd;
using Renligou.Core.Shared.EFCore;

namespace Renligou.Api.Boss.Controllers
{
    /// <summary>
    /// [控制器中文描述]
    /// </summary>
    [ApiController]
    [Route("/[资源名复数，如 companies]")]
    public class [控制器名]Controller(
        ICommandBus _commandBus,
        IQueryBus _queryBus,
        IUnitOfWork _uow,
        ILogger<[控制器名]Controller> _logger
    ) : Controller
    {
        // Action 方法...
    }
}
```

## 3. 操作实现

### 3.1 创建 (Create)

- **HTTP 方法**: `POST`
- **属性**: `[HttpPost]`
- **参数**: `[FromBody] Create[资源名]Request request`
- **逻辑**: 
  1. 将 Request 映射为 Command。
  2. 使用 `_uow.ExecuteAsync` 包裹 Command 发送，确保持久化和事件发布的原子性。
  3. 处理返回结果：失败返回 `BadRequest`，成功返回 `Ok`。

```csharp
/// <summary>
/// 创建 [资源描述]
/// </summary>
[HttpPost]
public async Task<IActionResult> Create([FromBody] Create[资源名]Request request, CancellationToken cancellationToken = default)
{
    // 1. 映射 Request 到 Command
    var command = new Create[资源名]Command
    {
        // 属性赋值...
        Prop1 = request.Prop1,
        // 如果有日期/枚举转换需在此处理
    };

    // 2. 执行命令 (开启事务)
    var res = await _uow.ExecuteAsync<Result>(async () =>
    {
        return await _commandBus.SendAsync<Create[资源名]Command, Result>(command, cancellationToken);
    }, true);

    // 3. 处理结果
    if (!res.Success)
        return BadRequest(res.Error);

    return Ok();
}
```

### 3.2 更新 (Update)

更新通常分为：基础信息更新、子实体更新、状态更新等。

- **HTTP 方法**: `PUT`
- **属性**: `[HttpPut("{id:long}/[子路径]")]` (如 `basic`, `address`, `state`)
- **参数**: `[FromRoute] long id`, `[FromBody] Modify[资源名][类型]Request request`
- **逻辑**: 
  1. 将 Route ID 和 Request Body 映射为 Command。
  2. 使用 `_uow.ExecuteAsync` 包裹。
  3. 处理结果。

```csharp
/// <summary>
/// 修改 [资源描述] 基础信息
/// </summary>
[HttpPut("{id:long}/basic")]
public async Task<IActionResult> ModifyBasic(
    [FromRoute] long id,
    [FromBody] Modify[资源名]BasicRequest request,
    CancellationToken cancellationToken = default
)
{
    var command = new Modify[资源名]BasicCommand
    {
        [资源名]Id = id, // IDs通常来自Route
        Prop1 = request.Prop1,
        // ...
    };

    var res = await _uow.ExecuteAsync<Result>(async () =>
    {
        return await _commandBus.SendAsync<Modify[资源名]BasicCommand, Result>(command, cancellationToken);
    }, true);

    if (!res.Success)
    {
        return BadRequest(res.Error);
    }

    return Ok();
}
```

### 3.3 获取详情 (Read Detail)

- **HTTP 方法**: `GET`
- **属性**: `[HttpGet("{id:long}")]`
- **参数**: `[FromRoute] long id`
- **逻辑**: 
  1.这 创建 Query 对象。
  2. 使用 `_queryBus.QueryAsync` 获取结果。
  3. 检查结果一般在 QueryHandler 内部处理 Not Found 并返回 Fail，或者 Controller 层判空。推荐 Handler 层返回 Result。

```csharp
/// <summary>
/// 获取 [资源描述] 详情
/// </summary>
[HttpGet("{id:long}")]
public async Task<IActionResult> GetDetail(
    [FromRoute] long id,
    CancellationToken cancellationToken = default
)
{
    var query = new Get[资源名]DetailQuery(id);
    var res = await _queryBus.QueryAsync<Get[资源名]DetailQuery, Result<[资源名]DetailDto?>>(query, cancellationToken);
    
    if (!res.Success)
    {
        return BadRequest(res.Error);
    }

    return Ok(res.Value);
}
```

### 3.4 获取列表 (Read List)

用于下拉框选择或非分页列表。

- **HTTP 方法**: `GET`
- **属性**: `[HttpGet]`
- **参数**: `[FromQuery] Get[资源名]ListRequest request`
- **逻辑**: 映射 Request 到 Query -> QueryBus -> Ok。

```csharp
/// <summary>
/// 获取 [资源描述] 列表
/// </summary>
[HttpGet]
public async Task<IActionResult> GetList(
    [FromQuery] Get[资源名]ListRequest request,
    CancellationToken cancellationToken = default
)
{
    var query = new Get[资源名]ListQuery(
        request.Param1,
        request.Param2
        // ...
    );

    var res = await _queryBus.QueryAsync<Get[资源名]ListQuery, Result<List<[资源名]ListDto>>>(query, cancellationToken);
    
    if (!res.Success)
    {
        return BadRequest(res.Error);
    }

    return Ok(res.Value);
}
```

### 3.5 获取分页 (Read Pagination)

- **HTTP 方法**: `GET`
- **属性**: `[HttpGet("pagination")]`
- **参数**: `[FromQuery] Get[资源名]PageRequest request`
- **逻辑**: 
  1. Request 应包含分页参数 (Page, PageSize) 和筛选参数。
  2. 映射到 Query。
  3. QueryBus 返回 `Pagination<Dto>`。

```csharp
/// <summary>
/// 获取 [资源描述] 分页
/// </summary>
[HttpGet("pagination")]
public async Task<IActionResult> GetPage(
    [FromQuery] Get[资源名]PageRequest request,
    CancellationToken cancellationToken = default
)
{
    var query = new Get[资源名]PageQuery(
        request.Param1,
        request.Param2,
        request.Page,
        request.PageSize
    );

    var res = await _queryBus.QueryAsync<Get[资源名]PageQuery, Result<Pagination<[资源名]DetailDto>>>(query, cancellationToken);

    if (!res.Success)
    {
        return BadRequest(res.Error);
    }

    return Ok(res.Value);
}
```

### 3.6 删除 (Delete) - 可选

如果业务支持删除（物理或软删除），通常提供 DELETE 端点。

- **HTTP 方法**: `DELETE`
- **属性**: `[HttpDelete("{id:long}")]`
- **参数**: `[FromRoute] long id`

```csharp
/// <summary>
/// 删除 [资源描述]
/// </summary>
[HttpDelete("{id:long}")]
public async Task<IActionResult> Delete(
    [FromRoute] long id,
    CancellationToken cancellationToken = default
)
{
    var command = new Destroy[资源名]Command(id);

    var res = await _uow.ExecuteAsync<Result>(async () =>
    {
        return await _commandBus.SendAsync<Destroy[资源名]Command, Result>(command, cancellationToken);
    }, true);

    if (!res.Success)
    {
        return BadRequest(res.Error);
    }

    return Ok();
}
```
