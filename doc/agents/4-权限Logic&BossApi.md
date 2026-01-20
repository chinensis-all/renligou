# 权限Logic & Boss Api

权限用于限制Boss管理员的操作权限，请根据以下要求开发CQRS逻辑以及Boss的Cqrs Api.

## 1 权限Logic

请参考Skill :  `doc\agents\skills\单表CQRS.md`

定义如下：
- [数据库表名] : `permission`
- [基础类名] : Permision
- [领域名] :  IdentityAccess
- [上下文名] : AuthorizationContext

## 2 CRUD Api

请参考Skill :  `doc\agents\skills\标准CRUD-Api.md`

定义如下：

- [控制器名] : PermissionController
- [路由前缀] : /permissions

## 3 补充规则

- 代码如果需要注释，应注尽注，符合AspNet Core及C#规范。
- 所有异常信息必须中文，便于定位。
- 所有新增公开API必须有注释，便于生成文档。
- 全程中文交流，代码应注尽注，关键路径性能优先（减少反射与分配）。
- 提供单元测试覆盖：正常派发、未注册处理器、重复注册/多实现、CancellationToken传播、并发派发一致性、管道行为顺序（若启用管道）。
- 在创建和更新处理中， 如果GroupId不为零（更新还需要Command的GroupId不等于查询的GroupId）， 需要调用PermissionGroupRepository检查所属于的权限组是否存在，所以需要在IPermissionGroupRepository方法添加一个返回bool的ExistsAsync方法，参数为int groupId和CancellationToken cancellationToken, 并在基础设施实现。
- 在删除PermissionGroup时，需要检查其自身还有所有子代PermissionGroup下是否有Perimission存在， 如果存在则不允许删除， 抛出异常提示“该权限组或其子权限组下存在权限， 不允许删除”， 需要在IPermissionRepository中添加一个返回bool的HasPermissionsAsync方法， 参数为int groupId和CancellationToken cancellationToken, 并在基础设施实现。