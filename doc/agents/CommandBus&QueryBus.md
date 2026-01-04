{
  "agent": {
    "name": "Antigravity.CommandQueryBus.Agent",
    "version": "1.0.0",
    "language": "zh-CN",
    "techStack": {
      "runtime": ".NET 10",
      "framework": "ASP.NET Core 10",
      "packagesPolicy": "严禁引入任何新NuGet包；严禁修改现有csproj引用；只允许使用BCL与项目内已有依赖。"
    },
    "conversationPolicy": {
      "allMessagesInChinese": true,
      "alwaysExplainWhatYouAreDoing": true,
      "askBeforeAssuming": false,
      "noBackgroundWork": true
    },
    "projectLayout": {
      "root": "src",
      "modules": [
        "src/core/Renligou.Core.Shared",
        "src/core/Renligou.Core.Application",
        "src/core/Renligou.Core.Domain",
        "src/core/Renligou.Core.Infrastructure"
      ],
      "tests": [
        "tests/Renligou.Core.Application.Tests",
        "tests/Renligou.Core.Domain.Tests",
        "tests/Renligou.Core.Infrastructure.Tests"
      ],
      "note": "文件夹位置与命名由本Agent固定指定，不允许开发工具自由发挥/改名/改层。"
    },
    "goals": [
      "在Renligou.Core.Application中实现高性能CommandBus与QueryBus（不依赖第三方包）。",
      "在Renligou.Core.Shared中定义稳定的接口/基类/异常/可选的中间件管道抽象。",
      "提供DI注册扩展（IServiceCollection扩展）以便在ASP.NET Core中快速接入。",
      "提供单元测试覆盖：正常派发、未注册处理器、重复注册/多实现、CancellationToken传播、并发派发一致性、管道行为顺序（若启用管道）。",
      "全程中文交流，代码应注尽注，关键路径性能优先（减少反射与分配）。"
    ],
    "hardRules": [
      "严禁引入不存在的包/框架；不得新增NuGet引用；不得改动csproj以添加包。",
      "不生成示例使用代码（如Demo Controller / Sample Command等），但必须写测试（测试可自定义最小Command/Query与Handler放在测试项目内）。",
      "接口定义与文件位置必须严格按本Agent指定，不得随意拆分/合并/迁移。",
      "性能要求：派发路径避免每次反射；必须做委托缓存或等效优化；避免不必要装箱/捕获。",
      "所有新增公开API必须写XML注释（///），内部关键逻辑写行内注释（//）。",
      "异常信息必须中文，便于定位。"
    ],
    "designDecisions": {
      "abstractionsPlacement": {
        "shared": [
          "ICommand/ICommand<TResult>",
          "IQuery<TResult>",
          "ICommandHandler<,>/ICommandHandler<>",
          "IQueryHandler<,>",
          "ICommandBus/IQueryBus",
          "IPipelineBehavior<TRequest,TResponse>（可选）",
          "CommandQueryBusException及派生异常"
        ],
        "application": [
          "CommandBus（实现ICommandBus）",
          "QueryBus（实现IQueryBus）",
          "PipelineInvoker（内部：若启用管道）",
          "ServiceCollectionExtensions（注册扩展）"
        ]
      },
      "busDispatchStrategy": "使用IServiceProvider解析处理器；用ConcurrentDictionary缓存“(RequestType,ResponseType)->委托”以避免重复反射；委托内部通过泛型闭包调用强类型Handler方法。",
      "handlerMultiplicity": "默认要求每个Command/Query只有且仅有一个Handler；如果解析到0个或>1个，抛中文异常并带上类型名。",
      "cancellationToken": "必须从Bus入口传递到Handler，且管道（若存在）也必须传递。",
      "pipeline": {
        "enabledByDefault": true,
        "note": "管道接口定义在Shared；实现通过Application内部组合。若没有注册任何IPipelineBehavior，则直接调用Handler，不产生额外枚举分配（使用数组/列表一次性缓存）。"
      }
    },
    "implementationPlan": [
      {
        "step": 1,
        "title": "创建Shared层的命令/查询抽象与异常体系",
        "actions": [
          "在Renligou.Core.Shared新增 Commanding 与 Querying 文件夹，放置接口与异常。",
          "定义 ICommand 与 IQuery<TResult>，以及对应Handler接口。",
          "定义 ICommandBus/IQueryBus 统一入口。",
          "定义异常：HandlerNotFoundException、MultipleHandlersFoundException、InvalidHandlerRegistrationException。"
        ]
      },
      {
        "step": 2,
        "title": "在Application层实现高性能CommandBus/QueryBus",
        "actions": [
          "实现 CommandBus : ICommandBus 与 QueryBus : IQueryBus。",
          "实现委托缓存：ConcurrentDictionary<CacheKey, Func<IServiceProvider, object, CancellationToken, Task<object?>>> 或等效结构。",
          "CacheKey用readonly struct，包含RequestType与ResponseType，重写Equals/GetHashCode避免装箱。",
          "构建委托时仅允许一次反射：定位泛型Handle方法并生成强类型调用（可用静态泛型方法+闭包减少反射）。"
        ]
      },
      {
        "step": 3,
        "title": "实现可选管道（Pipeline）并确保无注册时零额外成本",
        "actions": [
          "定义 IPipelineBehavior<TRequest,TResponse> 于Shared。",
          "在Application内部实现PipelineInvoker：先解析所有IPipelineBehavior，再按注册顺序串联 next。",
          "若无行为：直接执行handler委托。",
          "强调：不得使用LINQ（避免分配/迭代开销），用for循环。"
        ]
      },
      {
        "step": 4,
        "title": "提供DI注册扩展与最小集成方式",
        "actions": [
          "在Renligou.Core.Application创建 DependencyInjection 文件夹与 ServiceCollectionExtensions。",
          "添加 AddRenligouCommandQueryBus(IServiceCollection services) 扩展：注册 ICommandBus/IQueryBus 为单例或Scoped（默认Scoped，避免跨请求持有作用域服务）。",
          "不扫描程序集、不反射注册；只注册Bus本体与可选基础组件。Handler由各模块自行AddScoped注册（测试里手动注册）。"
        ]
      },
      {
        "step": 5,
        "title": "编写测试（以现有测试框架为准，不引入新包）",
        "actions": [
          "先读取 tests/*/*.csproj 判断使用 xUnit / NUnit / MSTest；选择其一写测试，不新增包。",
          "在测试项目内定义最小Command/Query与Handler（仅用于测试，不算“示例使用代码”）。",
          "覆盖：成功派发、找不到Handler、多Handler、CancellationToken传播、并发派发（Parallel.ForEachAsync或Task.WhenAll）、管道顺序与短路（若实现管道）。"
        ]
      }
    ],
    "filesToCreate": [
      {
        "path": "src/core/Renligou.Core.Shared/Commanding/ICommand.cs",
        "contentGuidelines": [
          "定义 marker interface ICommand",
          "定义泛型 ICommand<TResult>（如果需要命令返回值）",
          "写XML注释"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Shared/Commanding/ICommandHandler.cs",
        "contentGuidelines": [
          "定义 ICommandHandler<TCommand> : Task HandleAsync(TCommand, CancellationToken)",
          "定义 ICommandHandler<TCommand,TResult> : Task<TResult> HandleAsync(TCommand, CancellationToken)",
          "写XML注释"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Shared/Querying/IQuery.cs",
        "contentGuidelines": [
          "定义 IQuery<TResult>",
          "写XML注释"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Shared/Querying/IQueryHandler.cs",
        "contentGuidelines": [
          "定义 IQueryHandler<TQuery,TResult> : Task<TResult> HandleAsync(TQuery, CancellationToken)",
          "写XML注释"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Shared/Bus/ICommandBus.cs",
        "contentGuidelines": [
          "定义 SendAsync<TCommand>(TCommand, CancellationToken)",
          "定义 SendAsync<TCommand,TResult>(TCommand, CancellationToken)（如采用命令返回值）",
          "写XML注释"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Shared/Bus/IQueryBus.cs",
        "contentGuidelines": [
          "定义 QueryAsync<TQuery,TResult>(TQuery, CancellationToken)",
          "写XML注释"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Shared/Pipeline/IPipelineBehavior.cs",
        "contentGuidelines": [
          "定义 IPipelineBehavior<TRequest,TResponse>",
          "约定委托：Task<TResponse> HandleAsync(TRequest request, CancellationToken ct, Func<Task<TResponse>> next)",
          "写XML注释"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Shared/Exceptions/CommandQueryBusException.cs",
        "contentGuidelines": [
          "定义基类 CommandQueryBusException : Exception",
          "定义派生异常：HandlerNotFoundException、MultipleHandlersFoundException、InvalidHandlerRegistrationException",
          "异常消息中文，包含request/handler类型名"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Application/Bus/Internal/CacheKey.cs",
        "contentGuidelines": [
          "readonly struct CacheKey { Type RequestType; Type ResponseType; }",
          "实现IEquatable<CacheKey>，重写GetHashCode",
          "避免装箱"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Application/Bus/CommandBus.cs",
        "contentGuidelines": [
          "实现 ICommandBus",
          "使用ConcurrentDictionary<CacheKey, Delegate>缓存派发委托",
          "不得使用LINQ，使用for/if",
          "关键路径写注释说明性能考虑"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Application/Bus/QueryBus.cs",
        "contentGuidelines": [
          "实现 IQueryBus",
          "同样使用缓存",
          "CancellationToken透传"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Application/Bus/Internal/PipelineInvoker.cs",
        "contentGuidelines": [
          "内部类：负责将IPipelineBehavior按顺序串联",
          "无pipeline时直接调用handler",
          "不得使用递归（可用for构造链或反向构造委托）"
        ]
      },
      {
        "path": "src/core/Renligou.Core.Application/DependencyInjection/ServiceCollectionExtensions.cs",
        "contentGuidelines": [
          "定义 AddRenligouCommandQueryBus 扩展",
          "注册 ICommandBus/IQueryBus（默认Scoped）",
          "不做程序集扫描",
          "写XML注释"
        ]
      },
      {
        "path": "tests/Renligou.Core.Application.Tests/Bus/CommandBusTests.cs",
        "contentGuidelines": [
          "先读取csproj确定测试框架；按现有框架写",
          "覆盖：成功、未注册、重复注册、多实现",
          "定义测试用命令与handler放在同文件或TestSupport文件夹"
        ]
      },
      {
        "path": "tests/Renligou.Core.Application.Tests/Bus/QueryBusTests.cs",
        "contentGuidelines": [
          "覆盖：成功、未注册、多handler",
          "覆盖CancellationToken传播：Handler内断言ct.IsCancellationRequested或触发取消"
        ]
      },
      {
        "path": "tests/Renligou.Core.Application.Tests/Bus/PipelineTests.cs",
        "contentGuidelines": [
          "若实现Pipeline：测试顺序与短路",
          "自定义2-3个PipelineBehavior，记录执行顺序（线程安全集合）"
        ]
      }
    ],
    "acceptanceChecklist": [
      "不新增任何NuGet包，不修改csproj依赖。",
      "所有新增public类型有XML注释，关键逻辑有行内注释。",
      "CommandBus/QueryBus派发路径具备缓存，避免每次反射。",
      "异常为中文，且包含关键类型信息。",
      "测试可运行并覆盖主要分支（成功/失败/并发/取消/管道）。"
    ],
    "developerInstructionsToTool": [
      "你是Antigravity开发工具，请严格按本JSON执行。",
      "执行每一步前，用中文说明将要做什么、涉及哪些文件、为什么这么放置。",
      "创建文件时必须完全匹配path，不得自行改名或换目录。",
      "实现接口/类时不得自行引入第三方依赖；如发现缺失引用，只能使用BCL或项目现有引用。",
      "写测试前必须先读取并报告 tests/Renligou.Core.Application.Tests 的csproj 使用了哪种测试框架，然后按该框架写测试。",
      "任何你不确定的地方，不要发问；按本Agent既定决策实现（这是硬规则）。",
      "全程中文与我交流，不得使用其他语言。包括生成的review",
      "代码应注释尽注"
    ]
  }
}
