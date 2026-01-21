# 部门Logic & Boss Api

部门用于限制Boss管理员的操作部门，请根据以下要求开发CQRS逻辑以及Boss的Cqrs Api.

## 1 部门Logic

请参考Skill :  `doc\agents\skills\单表CQRS.md`

定义如下：
- [数据库表名] : `departments`
- [基础类名] : Department
- [领域名] :  IdentityAccess
- [上下文名] : AuthorizationContext

## 2 CRUD Api

请参考Skill :  `doc\agents\skills\标准CRUD-Api.md`

定义如下：

- [控制器名] : DepartmentController
- [路由前缀] : /departments

## 3 部门树结构API

请参考Skill :  `doc\agents\skills\单表树查询处理器.md`

定义如下：
- [数据库表名] : `departments`
- [基础类名] : Department
- [领域名] : IdentityAccess
- [传输对象名] : DepartmentTreeNodeDto

## 4 补充规则

- 代码如果需要注释，应注尽注，符合AspNet Core及C#规范。
- 所有异常信息必须中文，便于定位。
- 所有新增公开API必须有注释，便于生成文档。
- 全程中文交流，代码应注尽注，关键路径性能优先（减少反射与分配）。
- 提供单元测试覆盖：正常派发、未注册处理器、重复注册/多实现、CancellationToken传播、并发派发一致性、管道行为顺序（若启用管道）。
- 创建和更新部门时， 如果ParentId不为零（更新还需要Command的ParentId不等于查询的ParentId）， 需要调用DepartmentRepository检查所属于的上级部门是否存在，所以需要在IDepartmentRepository方法添加一个返回bool的ExistsAsync方法，参数为int parentDepartmentId和CancellationToken cancellationToken, 并在基础设施实现。
- 需要添加一个禁用部门的接口
  * router: post /departments/{id}/lock
  * Controller 方法名: InactiveDepartment
  * 命令类名: InactiveDepartmentCommand
  * 名命令处理器类名: InactiveDepartmentHandler
- 需要添加一个启用部门的接口
  * router: delete /departments/{id}/lock
  * Controller 方法名: ActivateDepartment
  * 命令类名: ActivateDepartmentCommand
  * 名命令处理器类名: ActivateDepartmentHandler  