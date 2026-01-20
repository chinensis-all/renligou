# Boss菜单Logic & Boss Api

Boss菜单是Boss管理员权限分组/分类，请根据以下要求开发CQRS逻辑以及Boss的Cqrs Api.

## 1 Boss菜单Logic

请参考Skill :  `doc\agents\skills\单表CQRS.md`

定义如下：
- [数据库表名] : `menus`
- [基础类名] : Menu
- [领域名] :  IdentityAccess
- [上下文名] : UiAccessContext

## 2 CRUD Api

请参考Skill :  `doc\agents\skills\标准CRUD-Api.md`

定义如下：

- [控制器名] : MenuController
- [路由前缀] : /menus

## 3 补充规则

- 代码如果需要注释，应注尽注，符合AspNet Core及C#规范。
- 所有异常信息必须中文，便于定位。
- 所有新增公开API必须有注释，便于生成文档。
- 全程中文交流，代码应注尽注，关键路径性能优先（减少反射与分配）。
- 提供单元测试覆盖：正常派发、未注册处理器、重复注册/多实现、CancellationToken传播、并发派发一致性、管道行为顺序（若启用管道）。
- menu_name不单独做冲突判断， menu_name及menu_tag联合判断冲突。
- 添加一个修改菜单显示状态的接口
  * router: PATCH /menus/{id}/visibility
  * Controller 方法名: ChangeMenuVisibility
  * 命令类名: ChangeMenuVisibilityCommand   值 long id, bool IsHidden
  * 命令处理器类名: ChangeMenuVisibilityHandler
  * 处理器逻辑，查询对应id的菜单， 如果不存在抛出异常“菜单不存在”， 否则修改其IsVisible属性为传入的值并保存。 