using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Renligou.Application.Dynamic;
using Renligou.Boss.Components;
using Renligou.Boss.Extensions;
using Renligou.Infras.Persistence.EFcore;
using StackExchange.Redis;
using System.Text.Json;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 配置MySQL连接
builder.Services.AddDbContext<MySQLDBContext>(options =>
{
    string mysqlConnStr = builder.Configuration.GetConnectionString("Mysql") ?? throw new InvalidOperationException("配置文件中未找到 ConnectionStrings:Mysql，请检查 appsettings.json。");
    options.UseMySql(
        mysqlConnStr,
        ServerVersion.AutoDetect(mysqlConnStr)
    ).UseSnakeCaseNamingConvention();
});

// 配置缓存及Redis连接
string redisConnStr = builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("配置文件中未找到 ConnectionStrings:Redis，请检查 appsettings.json。");
builder.Services.AddFusionCache()
    .WithSerializer(
        new FusionCacheSystemTextJsonSerializer(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true } 
        )
    )
    .WithDistributedCache(
        new RedisCache(new RedisCacheOptions { Configuration = redisConnStr })
    );

// 注册服务
builder.Services
    .AddAntDesign()                                 // Ant Design Blazor
    .AddSnowflake()                                 // 雪花算法ID生成器
    .AddDynamicCrud()                               // 动态CRUD
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
