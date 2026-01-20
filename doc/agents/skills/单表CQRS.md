# 单表CQRS Skill

此Skill展示了如何在单个数据库表上实现CQRS（命令查询职责分离）模式。它包括两个主要部分：命令处理器和查询处理器。

## 1. 定义

- [数据库表名] : Mysql数据库表名，数据库表完整定义位于`doc/database/schema.sql`, 可根据表名进行寻找，表名由此Skill的用户在调用时指定。
- [数据库表段落] : 数据表由多个段落组成，段落之间采用空行分隔；
  * 数据库表段落分为主段落和子段落；
  * 数据库表段落主段落从第一个字段开始到第一个空行结束；
  * 数据库表子段落从第二个空行开始到下一个空行结束，以此类推。
- [基础类名] : 表示为此Skill中要创建的模型，命令，Handler等类名称的动态部分，由此Skill的用户在调用时制定。
- [领域名] : 表述应用层逻辑所属的领域名称，由此Skill的用户在调用时指定。
- [上下文名] : 表示领域逻辑所属的上下文名称，由此Skill的用户在调用时指定。

## 2. 命令处理器

基础命令处理器包含创建，更新，删除（软删除），其他命令处理器由用户在调用时指定。

### 2.1 领域逻辑

#### 2.1.1 值对象

数据表定义中Enum类型及子段落，均应在逻辑层创建为值对象。

如表中存在enum数据类型:

```mysql
`company_type` enum('HEADQUARTER','BRANCH','SUBSIDIARY') NOT NULL COMMENT '公司类型: HEADQUARTER(总公司) / BRANCH(分公司) / SUBSIDIARY(子公司，独立法人实体)',
```
应按以下规则创建值对象类:

- 类命名 : `[基础类名][字段名]`， 字段名大小，驼峰化，首字母大写, 如果[基础类名]与[字段名]存在相同单词，则消除字段名里相同单词 ；
- 文件位置 : `src/core/Renligou.Core.Domain/[上下文名]/Value/` ;
- 类型 : 应实现为sealed record ；

参考如下:

```csharp
public sealed record CompanyType
{
	public static readonly CompanyType Headquarter = new CompanyType("HEADQUARTER", "总公司");

	public static readonly CompanyType Branch = new CompanyType("BRANCH", "分公司");

	public static readonly CompanyType Subsidiary = new CompanyType("SUBSIDIARY", "子公司，独立法人实体");

	public string Code { get; }

	public string Description { get; }

	private CompanyType(string code, string description)
	{
		Code = code;
		Description = description;
	}

	public static CompanyType FromCode(string code) => code switch
	{
		"HEADQUARTER" => Headquarter,
		"BRANCH" => Branch,
		"SUBSIDIARY" => Subsidiary,
		_ => throw new ArgumentException($"Invalid company type code: {code}")
	};
}
```

#### 2.1.2 实体

如果数据表中存在字段落，如下所示：
```mysql
-- 地址信息（Address）
`province_id`        BIGINT                                       NOT NULL DEFAULT 0 COMMENT '所属省份ID',
`province`           VARCHAR(50)                                  NOT NULL DEFAULT '' COMMENT '所属省份',
`city_id`            BIGINT                                       NOT NULL DEFAULT 0 COMMENT '所属城市ID',
`city`               VARCHAR(50)                                  NOT NULL DEFAULT '' COMMENT '所属城市',
`district_id`        BIGINT                                       NOT NULL DEFAULT 0 COMMENT '所属区县ID',
`district`           VARCHAR(50)                                  NOT NULL DEFAULT 0 COMMENT '所属区县',
`completed_address`  VARCHAR(256)                                 NOT NULL DEFAULT '' COMMENT '完整地址',
```
子段落前后均为空行，且子段落前有注释说明。应按以下规则创建值对象类:

- 类命名 : `[基础类名][子段落注释说明括号内英文]` ；
- 文件位置 : `src/core/Renligou.Core.Domain/[上下文名]/Model/` ;
- 类型 : 应实现为sealed record ；
- 属性 : 包含子段落中的所有字段，采用驼峰形式，首字母大写；
- 属性类型 : 按照数据库字段类型映射为C#类型，如果字段类型为enum，则映射为对应的值对象类型；

参考如下:
```csharp
public sealed record Address
{
	public long ProvinceId { get; init; }

	public string Province { get; init; }

	public long CityId { get; init; }

	public string City { get; init; }

	public long DistrictId { get; init; }

	public string District { get; init; }

	public string CompletedAddress { get; init; }
}
```
#### 2.1.3 聚合根

数据表定义中主段落的字段，子段落，应在逻辑层创建为聚合根实体。

如下所示：
```mysql
CREATE TABLE IF NOT EXISTS `companies`
(
    `id`                 BIGINT                                       NOT NULL PRIMARY KEY COMMENT 'ID',
    `company_type`       ENUM ('HEADQUARTER', 'BRANCH', 'SUBSIDIARY') NOT NULL COMMENT '公司类型: HEADQUARTER(总公司) / BRANCH(分公司) / SUBSIDIARY(子公司)',
    `company_code`       VARCHAR(64)                                  NOT NULL COMMENT '公司编码',
    `company_name`       VARCHAR(128)                                 NOT NULL COMMENT '公司名称',
    `company_short_name` VARCHAR(64)                                           DEFAULT '' COMMENT '公司名简称',
    `legal_person_name`  VARCHAR(64)                                           DEFAULT '' COMMENT '法人',
    `credit_code`        VARCHAR(32)                                           DEFAULT '' COMMENT '统一社会信用代码',
    `registered_address` VARCHAR(256)                                          DEFAULT '' COMMENT '注册地址',
    `remark`             VARCHAR(512)                                          DEFAULT '' COMMENT '备注',
    `created_at`         DATETIME                                     NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at`         DATETIME                                     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',

    -- 地址信息（Address）
    `province_id`        BIGINT                                       NOT NULL DEFAULT 0 COMMENT '所属省份ID',
    `province`           VARCHAR(50)                                  NOT NULL DEFAULT '' COMMENT '所属省份',
    `city_id`            BIGINT                                       NOT NULL DEFAULT 0 COMMENT '所属城市ID',
    `city`               VARCHAR(50)                                  NOT NULL DEFAULT '' COMMENT '所属城市',
    `district_id`        BIGINT                                       NOT NULL DEFAULT 0 COMMENT '所属区县ID',
    `district`           VARCHAR(50)                                  NOT NULL DEFAULT 0 COMMENT '所属区县',
    `completed_address`  VARCHAR(256)                                 NOT NULL DEFAULT '' COMMENT '完整地址',

    -- 启用状态（State)
    `enabled`            TINYINT(1)                                   NOT NULL DEFAULT 1 COMMENT '是否启用: 1=启用, 0=禁用',
    `effective_date`     DATE                                                  DEFAULT NULL COMMENT '生效日期',
    `expired_date`       DATE                                                  DEFAULT NULL COMMENT '失效日期',
    KEY `idx_company_code` (`company_code`) USING BTREE,
    UNIQUE KEY `uk_company_name` (`company_name`) USING BTREE
) ENGINE = InnoDB
  DEFAULT CHARSET = utf8mb4
  COLLATE = utf8mb4_unicode_ci COMMENT ='公司/子公司/分公司表';
```

应按照如下规则创建聚合根:

- 类命名 : `[基础类名]` ；
- 文件位置 : `src/core/Renligou.Core.Domain/[上下文名]/Model/` ;
- 类型 : 应实现为class ；
- 继承 : 继承自`Renligou.Core.Shared.Ddd.AggregateBase` ；
- 属性 : 包含主段落中的所有字段及子段落对应的值对象类型，采用驼峰形式，首字母大写；
- 规则1: 数据库表中的`created_at` 及 `updated_at`两个字段无需映射为聚合根属性；
- 规则2: 数据库表中的`id`字段不用映射，应为已经在`AggregateBase`中定义；
- 规则3: 如果字段类型为enum，则映射为对应的值对象类型；
- 规则4: 如果存在子段落，则应包含对应的值对象类型属性；
- 规则5: 应包含领域行为方法，如创建，更新，删除等方法；
- 规则6: TINYINT(1)类型应映射为bool类型；
- 规则7: DATETIME类型应映射为DateTimeOffset类型；
- 规则8: DATE类型应映射为DateOnly类型；
- 规则9: VARCHAR类型应映射为string类型；
- 规则10: BIGINT类型应映射为long类型；
- 规则11: 对于可为空字段，应使用可空类型；
- 规则12: 对于有默认值字段，应在属性初始化器中设置默认值；

参考如下:
```csharp
public class Company : AggregateBase
{
    public CompanyType CompanyType { get; private set; }

    public string CompanyCode { get; private set; }

    public string CompanyName { get; private set; }

    public string CompanyShortName { get; private set; } = string.Empty;

    public string LegalPersonName { get; private set; } = string.Empty;

    public string CreditCode { get; private set; } = string.Empty;

    public string RegisteredAddress { get; private set; } = string.Empty;

    public string Remark { get; private set; } = string.Empty;

    public Address CompanyAddress { get; private set; }

    public ConpanyState State { get; private set; }

    public void Create()
    {
        // 创建逻辑
        // 注册创建领域事件逻辑
    }

    public void ModifyBasic(
        string companyType,
        string companyName,
        string companyShortName,
        string legalPersonName,
        string creditCode,
        string registeredAddress,
        string remarke
    )
    {
         // 修改逻辑
         // 注册修改领域事件逻辑
    }

    public void Modify Address(ConpanyAddress address) 
    {
         // 修改地址逻辑
         // 注册修改领域事件逻辑
    }

    public void ModifyState(
        CompanyState state
    )
    {
         // 修改状态逻辑
         // 注册修改领域事件逻辑
    }

    /** 软删除逻辑 
     * 如果数据表中存在deleted_at字段，则实现软删除逻辑, 不存在则不实现，项目中不存在物理删除操作
     */
     public void Destroy()
     {
         // 软删除逻辑
         // 注册删除领域事件逻辑
     }
}
```

#### 2.1.4 领域事件

对于每个命令操作，应创建对应的领域事件类，规则如下:

- 为聚合根创建行为创建一个领域事件类，名称为: `[基础类名]CreatedEvent` ；
- 为聚合根修改行为创建一个领域事件类，名称为: `[基础类名]ModifiedEvent` ；
- 如果数据库表包含`deleted_at`字段，则为聚合根软删除行为创建一个领域事件类，名称为: `[基础类名]DestroyedEvent` ；
- 为聚合根中聚合的每个实体的修改行为创建一个修改领域事件类，名称为: `[实体类名]ModifiedEvent` ；
- 文件位置 : `src/core/Renligou.Core.Domain/[上下文名]/Event/` ;
- 类型 : 应实现为`sealed record` ；
- 继承 : 继承自`Renligou.Core.Shared.Events.IIntegrationEvent`
- 属性 : 包含聚合根ID及事件相关属性， 必须实现属性`OccurredAt`;
- 规则1: 属性命名采用驼峰形式，首字母大写；
- 规则2: 属性类型按聚合根及实体类中对应属性类型映射；
- 规则3: 属性`OccurredAt`类型为DateTimeOffset；
- 规则4: 对于string类型属性，应在初始化器中设置默认值为空字符串；
- 规则5: 对于值对象类型属性，应使用对应的值对象类型；
- 规则6: 对于可为空字段，应使用可空类型；
- 规则7: 对于有默认值字段，应在属性初始化器中设置默认值；
- 规则8: 聚合根ID的类型是AggregateId， 事件的类型为long. 所以需要转换：聚合根.Id.Id

参考如下:
```csharp
public sealed record CompanyCreatedEvent : IIntegrationEvent
{
    public DateTimeOffset OccurredAt { get; init; }

    public long Id { get; init; }

    public CompanyType CompanyType { get; init; }

    public string CompanyCode { get; init; } = string.Empty;

    public string CompanyName { get; init; } = string.Empty;

    public string CompanyShortName { get; init; } = string.Empty;

    public string LegalPersonName { get; init; } = string.Empty;

    public string CreditCode { get; init; } = string.Empty;

    public string RegisteredAddress { get; init; } = string.Empty;

    public string Remark { get; init; } = string.Empty;

    public Address Address { get; init; } = default!;

    public CompanyState State { get; init; } = default!;
}

public sealed record CompanyStateModifiedEvent : IIntegrationEvent {

     public DateTimeOffset OccurredAt { get; init; }
        
     public long CompanyId { get; init; }

     public CompanyState State { get; init; }
}
```

在聚合根中注册领域事件: 参考如下：
```csharp
public class Company : AggregateBase
}
{
    // 省略其他属性和方法
    public void Create()
    {
        // 创建逻辑
        // 注册创建领域事件逻辑
        var createdEvent = new CompanyCreatedEvent
        {
            OccurredAt = DateTimeOffset.UtcNow,
            Id = this.Id.Id,
            CompanyType = this.CompanyType,
            CompanyCode = this.CompanyCode,
            CompanyName = this.CompanyName,
            CompanyShortName = this.CompanyShortName,
            LegalPersonName = this.LegalPersonName,
            CreditCode = this.CreditCode,
            RegisteredAddress = this.RegisteredAddress,
            Remark = this.Remark,
            Address = this.CompanyAddress,
            State = this.State
        };
        this.AddDomainEvent(createdEvent);
    }

    public void ModifyState(
        CompanyState state
    )
    {
         // 修改状态逻辑
         // 注册修改领域事件逻辑
         var stateModifiedEvent = new CompanyStateModifiedEvent
         {
             OccurredAt = DateTimeOffset.UtcNow,
             CompanyId = this.Id.Id,
             State = state
         };
         this.AddDomainEvent(stateModifiedEvent);
    }
}
```

#### 2.1.5 领域仓储接口

领域仓储接口定义了聚合根的插叙及持久化操作，规则如下:

- 类命名 : `I[基础类名]Repository` ；
- 文件位置 : `src/core/Renligou.Core.Domain/[上下文名]/Repo/` ;
- 类型 : 应实现为`interface` ；
- 继承 : 继承自`Renligou.Core.Shared.Ddd.IRepository, Renligou.Core.Domain.CommonContext.Repo.DomainRepository<[聚合根名]>` ；
- 方法 : 
  * 继承了`IRepository`及`DomainRepository`中的方法，无需重复定义；
  * 如果数据库中存在唯一索引字段，如下所示:
  ```mysql
  UNIQUE KEY `uk_company_name` (`company_name`) USING BTREE
  ```
  则应在领域仓储接口中定义一个检测冲突的方法
  ```csharp
  Task<bool> Is[索引名去uk_驼峰首字符大写]ConflictAsync(string [字段名小驼峰])
  ```

### 2.2 创建命令处理器

#### 2.2.1 创建命令

按以下规则创建命令类:

- 类命名 : `Create[基础类名]Command` ；
- 文件位置 : `src/core/Renligou.Core.Application/[领域名]/Commands/` ;
- 类型 : 应实现为`sealed record` ；
- 继承 : 继承自`Renligou.Core.Shared.Commanding.ICommand<Renligou.Core.Shared.Ddd.Result>` ;
- 属性 : 包含创建聚合根所需的所有属性；
- 规则1: 属性命名采用驼峰形式，首字母大写；
- 规则2: 属性类型按聚合根中对应属性类型映射；
- 规则3: 对于string类型属性，应在初始化器中设置默认值为空字符串；
- 规则4: 对于值对象类型属性，应使用对应的值对象类型；
- 规则5: 对于可为空字段，应使用可空类型；
- 规则6: 对于有默认值字段，应在属性初始化器中设置默认值；
- 规则7: 不包含聚合根ID属性；
- 规则8: 不包含`created_at`及`updated_at`属性；
- 规则9: 包含子段落对应的值对象类型属性；
- 规则10: 必须包含`Result Validate()`方法，用于业务用例验证，通常数据库表not null 字段均需进行验证。

参考如下:
```csharp
public sealed record CreateCompanyCommand : ICommand<Result>
{
        public string CompanyType { get; init; } = string.Empty;

        public string CompanyCode { get; init; } = string.Empty;

        public string CompanyName { get; init; } = string.Empty;

        public string CompanyShortName { get; init; } = string.Empty;

        public string LegalPersonName { get; init; } = string.Empty;

        public string CreditCode { get; init; } = string.Empty;

        public string RegisteredAddress { get; init; } = string.Empty;

        public string Remark { get; init; } = string.Empty;

        public long ProvinceId { get; init; }

        public long CityId { get; init; }

        public long DistrictId { get; init; }

        public string CompletedAddress { get; init; } = string.Empty;

        public bool Enabled { get; init; }

        public DateOnly? EffectiveDate { get; init; }

        public DateOnly? ExpiredDate { get; init; }

        public Result Validate()
        {
            if (!Enum.TryParse<CompanyType>(CompanyType, ignoreCase: true, out _))
            {
                return Result.Fail("Company.Create.Error", $"非法的公司类型: {CompanyType}");
            }

            if (string.IsNullOrEmpty(CompanyName))
            {
                return Result.Fail("Company.Create.Error", "缺失公司名称");
            }

            if (string.IsNullOrEmpty(CompanyShortName))
            {
                return Result.Fail("Company.Create.Error", "缺失公司简称");
            }

            if (ProvinceId <= 0 || CityId <= 0 || DistrictId <= 0)
            {
                return Result.Fail("Company.Create.Error", "缺失公司地址信息");
            }

            if (Enabled && EffectiveDate == null)
            {
                return Result.Fail("Company.Create.Error", "启用状态下缺失生效日期");
            }

            if (Enabled && EffectiveDate == null)
            {
                return Result.Fail("Company.Create.Error", "启用状态下缺失失效日期");
            }

            return Result.Ok();
        }
}
```   

#### 2.2.2 创建命令处理器

按以下规则创建命令处理器类:

- 类命名 : `Create[基础类名]Handler` ；
- 文件位置 : `src/core/Renligou.Core.Application/[领域名]/Handlers/` ;
- 接口 : `ICommandHandler<Create[基础类名]Command, Result>` ;
- 依赖 : `I[基础类名]Repository`, `IOutboxRepository`, `IIdGenerator` 以及其他必要的查询仓储(如用于填充数据) ;
- 逻辑 : 
  1. 调用`command.Validate()`进行校验；
  2. 调用`_idGenerator.NextId()`生成ID；
  3. 调用仓储检查唯一性约束(如果有)；
  4. 构造值对象(如果有)；
  5. 构造聚合根实例；
  6. 调用聚合根`Create()`方法；
  7. 调用`_[基础类名]Repository.SaveAsync()`保存；
  8. 调用`_outboxRepository.AddAsync()`保存领域事件到Outbox；
  9. 返回`Result.Ok()`；

参考如下:
```csharp
public class CreateCompanyHandler(
    ICompanyRepository _companyRepository,
    IRegionQueryRepository _regionQueryRepository,
    IOutboxRepository _outboxRepository,
    IIdGenerator _idGenerator
) : ICommandHandler<CreateCompanyCommand, Result>
{
    public async Task<Result> HandleAsync(CreateCompanyCommand command, CancellationToken cancellationToken)
    {
        var validation = command.Validate();
        if (!validation.Success) return validation;

        long id = _idGenerator.NextId();

        if (await _companyRepository.CompanyNameExistsAsync(id, command.CompanyName))
        {
            return Result.Fail("Company.Create.Error", "公司名称已存在");
        }
        
        // 构造值对象逻辑...
        var address = new Address(...);
        var state = new CompanyState(...);

        var company = new Company(
            new AggregateId(id, true),
            // ... 属性
            address,
            state
        );
        company.Create();

        await _companyRepository.SaveAsync(company);
        await _outboxRepository.AddAsync(company.GetRegisteredEvents(), "DOMAIN", company.GetType().Name, company.Id.id.ToString());

        return Result.Ok();
    }
}
```

### 2.3 更新命令处理器

通常包含基础信息更新、状态更新等。以基础信息更新为例:

- 命令类 : `Modify[基础类名]BasicCommand`
- 处理器类 : `Modify[基础类名]BasicHandler`
- 逻辑 : 
  1. 验证Command `Validate()`；
  2. 从仓储加载聚合根 `LoadAsync(command.[基础类名]Id)`；
  3. 检查聚合根是否存在；
  4. 调用聚合根业务方法 `ModifyBasic(...)`；
  5. 保存聚合根 `SaveAsync()`；
  6. 保存Outbox事件；

参考如下:
```csharp
public class ModifyCompanyBasicHandler(
    ICompanyRepository _companyRepository,
    IOutboxRepository _outboxRepository
) : ICommandHandler<ModifyCompanyBasicCommand, Result> {
    public async Task<Result> HandleAsync(ModifyCompanyBasicCommand command, CancellationToken cancellationToken)
    {
        var validation = command.Validate();
        if (!validation.Success) return validation;

        var company = await _companyRepository.LoadAsync(command.CompanyId);
        if (company == null)
            return Result.Fail("Company.NotFound", $"没有找到公司: {command.CompanyId}");

        company.ModifyBasic(
            // ... 字段映射
        );

        await _companyRepository.SaveAsync(company);
        await _outboxRepository.AddAsync(company.GetRegisteredEvents(), "DOMAIN", company.GetType().Name, company.Id.id.ToString());

        return Result.Ok();
    }
}
```

### 2.4 软删除命令处理器
如果包含软删除逻辑：

- 命令类 : `Destroy[基础类名]Command` (需包含Id)
- 处理器类 : `Destroy[基础类名]Handler`
- 逻辑 : Load -> Check Null -> Call `Destroy()` -> Save -> Outbox.

### 2.5 实体更新命令处理器
针对聚合根内部实体或者是复杂值对象(Mapping为数据库子段落)的更新，例如地址信息更新：

- 命令类 : `Modify[基础类名][子段落名]Command`
- 处理器类 : `Modify[基础类名][子段落名]Handler`
- 逻辑 : Load -> Check Null -> Construct Value Object -> Call Logic -> Save -> Outbox.

## 3 查询处理器

查询处理器负责读操作，通常返回DTO。

#### 3.1 查询仓储

- 接口命名 : `I[基础类名]QueryRepository`
- 继承 : `IRepository`
- 位置 : `src/core/Renligou.Core.Application/[领域名]/Queries/`
- 方法包含 : 
    - `QueryDetailAsync` : 返回详情DTO
    - `CountAsync` : 返回数量
    - `SearchAsync` : 返回列表DTO
    - `PaginateAsync` : 返回分页对象 `Pagination<T>`

参考:
```csharp
public interface ICompanyQueryRepository : IRepository
{
    Task<CompanyDetailDto?> QueryDetailAsync(long companyId, CancellationToken cancellationToken = default);
    Task<long> CountAsync(CompanySearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<List<CompanyListDto>> SearchAsync(CompanySearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<Pagination<CompanyDetailDto>> PaginateAsync(CompanySearchCriteria searchCriteria, CompanyPaginateCriteria paginateCriteria, CancellationToken cancellationToken = default);
}
```

#### 3.2 详情查询处理器

- 类命名 : `Get[基础类名]DetailHandler`
- Query类 : `Get[基础类名]DetailQuery` (包含Id)
- 返回 : `Result<[基础类名]DetailDto?>`
- 逻辑 : 调用QueryRepository查询，若空返回Fail，否则Ok(dto)。

#### 3.3 列表查询处理器

- 类命名 : `Get[基础类名]ListHandler`
- Query类 : `Get[基础类名]ListQuery` (包含筛选条件)
- 返回 : `Result<List<[基础类名]ListDto>>`
- 逻辑 : 构建Criteria -> 调用QueryRepository.SearchAsync -> 返回Ok(list)。

#### 3.4 分页查询处理器

- 类命名 : `Get[基础类名]PageHandler`
- Query类 : `Get[基础类名]PageQuery` (包含筛选条件 + Page, PageSize)
- 返回 : `Result<Pagination<[基础类名]DetailDto>>`
- 逻辑 : 构建Criteria和PaginateCriteria -> 调用QueryRepository.PaginateAsync -> 返回Ok(pagination)。

## 4 持久化操作

### 4.1 领域对象实现 (PO)

- 类命名 : `[基础类名]Po`
- 位置 : `src/core/Renligou.Core.Infrastructure/Persistence/Pos/`
- 属性 : `[Table("表名")]` 特性， 包含所有列，使用 `[Key]`, `[Required]` 等特性标注。
- 类型映射 : 这里映射到数据库基本类型(short, long, string, DateTimeOffset等)。

### 4.2 领域仓储及查询仓储实现

- 类命名 : `[基础类名]Repository`
- 位置 : `src/core/Renligou.Core.Infrastructure/Persistence/Repos/`
- 接口 : 实现 `I[基础类名]Repository` 和 `I[基础类名]QueryRepository`
- 依赖 : `DbContext`
- 核心逻辑 :
  1. **LoadAsync** : Find PO by ID -> Map PO to Aggregate -> Track PO in Dictionary.
  2. **SaveAsync** : 
     - New Aggregate -> Create PO -> Apply Changes -> `db.Add(po)`.
     - Existing Aggregate -> Get Tracked PO -> Apply Changes -> `db.Update(po)`.
  3. **MapToAggregate** : 将PO转换为聚合根，注意构造函数的调用，以及值对象的重构。
  4. **ApplyAggregateToPo** : 将聚合根属性回写到PO。
  5. **Query Methods** : 使用EF Core的 `Select` 及 `AsNoTracking` (通常Query操作不需要追踪) 将PO投影(Project)为DTO返回。

参考实现结构:
```csharp
public class CompanyRepository(DbContext _db) : ICompanyRepository, ICompanyQueryRepository
{
    private readonly Dictionary<long, CompanyPo> _tracked = new();

    // --- ICompanyRepository 实现 ---
    public async Task<Company?> LoadAsync(long id) { /* ... Get PO, Map, Track ... */ }
    public async Task SaveAsync(Company aggregate) { /* ... Map Aggregate to PO, Add/Update DbContext ... */ }

    // --- ICompanyQueryRepository 实现 ---
    public Task<CompanyDetailDto?> QueryDetailAsync(long id, CancellationToken token) 
    {
         return _db.Set<CompanyPo>()
                   .Where(x => x.Id == id)
                   .Select(x => new CompanyDetailDto { /* ... assignment ... */ })
                   .FirstOrDefaultAsync(token);
    }
    // ... Implement SearchAsync, PaginateAsync similarly using LINQ Select ...
}
```

### 4.3 DbContext配置
- 在`src\core\Renligou.Core.Infrastructure\Persistence\EFCore\MysqlDbContext.cs`文件中添加DbSet:
```csharp
public DbSet<CompanyPo> Companies { get; set; } = null!;
```

# 5 测试

测试是保障CQRS模式实现正确性的关键环节。本Skill推荐采用三层测试策略：领域单元测试、应用层单元测试和集成测试。

使用框架：`NUnit`, `Moq` (或手动Mock类), `WebApplicationFactory` (集成测试)。

### 5.1 领域层测试 (Domain Tests)

针对聚合根（Aggregate Root）进行纯单元测试，验证业务逻辑和领域事件的生成。

- **位置** : `tests/Renligou.Core.Domain.Tests/[上下文名]/`
- **主要验证点** :
  1. 聚合根状态变更是否正确。
  2. 是否注册了正确的领域事件 (`GetRegisteredEvents()`)。
  3. 领域事件内容是否与操作一致。

**示例代码** (`CompanyTests.cs`):

```csharp
[TestFixture]
public class CompanyTests
{
    [Test]
    public void Create_ShouldRegisterCompanyCreatedEvent()
    {
        // 1. Arrange & Act
        var company = new Company(...); // 构造聚合根
        company.Create();               // 执行行为

        // 2. Assert
        var events = company.GetRegisteredEvents();
        Assert.That(events, Has.Count.EqualTo(1));
        Assert.That(events[0], Is.TypeOf<CompanyCreatedEvent>());
        
        var @event = (CompanyCreatedEvent)events[0];
        Assert.That(@event.CompanyName, Is.EqualTo("Test Company"));
    }
}
```

### 5.2 应用层测试 (Application Handler Tests)

针对命令处理器（Command Handler）进行测试，Mock所有外部依赖（仓储、服务等），验证流程控制。

- **位置** : `tests/Renligou.Core.Application.Tests/[领域名]/`
- **Mocks** : 推荐使用手动Mock类（如 `MockCompanyRepository`）或 Moq 框架来模拟仓储行为。
- **主要验证点** :
  1. 验证器 (`Validate`) 是否被调用且生效。
  2. 仓储 (`Repository`) 是否被调用（`SaveAsync` 是否执行，参数是否正确）。
  3. Outbox (`OutboxRepository`) 是否接收到了事件。
  4. 业务规则检查（如名称重复）是否按预期工作。

**示例代码** (`CreateCompanyHandlerTests.cs`):

```csharp
[TestFixture]
public class CreateCompanyHandlerTests
{
    // 定义Mock成员变量
    private MockCompanyRepository _companyRepository;
    private MockOutboxRepository _outboxRepository;
    private CreateCompanyHandler _handler;

    [SetUp]
    public void SetUp()
    {
        // 初始化Mock和Handler
        _companyRepository = new MockCompanyRepository();
        _outboxRepository = new MockOutboxRepository();
        _handler = new CreateCompanyHandler(_companyRepository, ..., _outboxRepository, ...);
    }

    [Test]
    public async Task HandleAsync_WhenValidCommand_ShouldSaveCompanyAndOutbox()
    {
        // Arrange
        var command = new CreateCompanyCommand { ... };

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.That(result.Success, Is.True);
        
        // 验证仓储保存
        Assert.That(_companyRepository.SavedAggregate, Is.Not.Null);
        Assert.That(_companyRepository.SavedAggregate.CompanyName, Is.EqualTo(command.CompanyName));
        
        // 验证Outbox事件
        Assert.That(_outboxRepository.AddedEvents, Has.Count.GreaterThan(0));
    }
}
```

### 5.3 集成测试 (Connect All Layers)

通常位于 API 测试项目中，验证控制器、Total Pipeline、数据库/EF Core 映射（使用内存库或测试库）的集成情况。

- **位置** : `tests/Renligou.Api.Boss.Tests/` (对应API项目)
- **工具** : `WebApplicationFactory`, `HttpClient`, `NSubstitute` (用于Mock部分基础设施如ServiceBus)
- **主要验证点** :
  1. API 端点 (`/companies`) 是否可访问。
  2. 请求参数绑定是否正确。
  3. HTTP 状态码返回值。
  4. 端到端流程是否通畅。

**示例代码** (`CompanyControllerTests.cs`):

```csharp
[TestFixture]
public class CompanyControllerIntegrationTests
{
    private CustomWebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    [Test]
    public async Task Create_ShouldReturnOk()
    {
        var command = new CreateCompanyRequest { ... };
        var response = await _client.PostAsJsonAsync("/companies", command);
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
```

## 6 补充规则
- 应用层DTO的属性应用[Description]特性进行描述，便于生成文档。
```csharp
 [Description("公司ID")]
 public string CompanyId { get; init; }
```
- Dto均应实现为`sealed record`