using Renligou.Core.Infrastructure.Data.Connections;
using Renligou.Core.Infrastructure.Data.Inbox;
using Renligou.Core.Infrastructure.Data.Outbox;
using Renligou.Core.Infrastructure.Event;
using Renligou.Job.Main.Extensions;

var builder = Host.CreateApplicationBuilder(args);


builder.Services.AddMysql(
    builder.Configuration.GetConnectionString("Mysql")!,
    builder.Environment.IsDevelopment(),
    builder.Environment.EnvironmentName
);

builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();

builder.Services.AddRepository(new[]
{
    typeof(Renligou.Core.Infrastructure.InfrastructureLayer).Assembly
});

builder.Services.AddBus(new[]
{
    typeof(Renligou.Core.Application.ApplicationLayer).Assembly
});

builder.Services.AddAppFacade(new[]
{
    typeof(Renligou.Core.Application.ApplicationLayer).Assembly
});

builder.Services.AddSingleton<RabbitMqConnection>();

builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();

builder.Services.AddSingleton<IDbConnectionFactory, MySqlConnectionFactory>();

builder.Services.AddScoped<IOutboxDapperRepository, OutboxDapperRepository>();

builder.Services.AddScoped<IIdempotencyService, MySqlIdempotencyService>();

builder.Services.AddWorkers();

var host = builder.Build();
host.Run();
