# 角色Logic & Boss Api

角色是Boss管理员权限分组/分类，请根据以下要求开发CQRS逻辑以及Boss的Cqrs Api.

## 1 角色Logic

请参考Skill :  `doc\agents\skills\单表CQRS.md`

定义如下：
- [数据库表名] : `roles`
- [基础类名] : Role
- [领域名] :  IdentityAccess
- [上下文名] : AuthorizationContext

## 2 CRUD Api

请参考Skill :  `doc\agents\skills\标准CRUD-Api.md`

定义如下：

- [控制器名] : RoleController
- [路由前缀] : /roles

## 3 补充规则

- 代码如果需要注释，应注尽注，符合AspNet Core及C#规范。
- 所有异常信息必须中文，便于定位。
- 所有新增公开API必须有注释，便于生成文档。
- 全程中文交流，代码应注尽注，关键路径性能优先（减少反射与分配）。
- 提供单元测试覆盖：正常派发、未注册处理器、重复注册/多实现、CancellationToken传播、并发派发一致性、管道行为顺序（若启用管道）。