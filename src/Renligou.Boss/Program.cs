using Microsoft.EntityFrameworkCore;
using Renligou.Application.Dynamic;
using Renligou.Boss.Components;
using Renligou.Boss.Extensions;
using Renligou.Infras.Persistence.EFcore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 读取连接字符串
string redisConnStr = builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("配置文件中未找到 ConnectionStrings:Redis，请检查 appsettings.json。");
string mysqlConnStr = builder.Configuration.GetConnectionString("Mysql") ?? throw new InvalidOperationException("配置文件中未找到 ConnectionStrings:Mysql，请检查 appsettings.json。");
RabbitMQOptions rabbitMQOptions = builder.Configuration.GetSection("RabbitMQ").Get<RabbitMQOptions>() ?? throw new InvalidOperationException("配置文件中未找到 RabbitMQ 配置节，请检查 appsettings.json。");


// 注册服务
builder.Services
    .AddAntDesign()                                                                       // Ant Design Blazor
    .AddSnowflake()                                                                       // 雪花算法ID生成器
    .AddCache(redisConnStr)                                                               // 缓存服务
    .AddEventCap(rabbitMQOptions, mysqlConnStr)                                           // 一致性事件发布服务
    .AddMysql(mysqlConnStr)                                                               // MySQL EFCore
    .AddDynamicCrud()                                                                     // 动态CRUD
    ;

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var registry = scope.ServiceProvider.GetRequiredService<DynamicCrudRegistry>();
    EntityConfigRegistrar.Register(registry);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
