using Renligou.Job.Main.Extensions;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddJobServices(builder.Configuration, builder.Environment);

var host = builder.Build();
host.Run();
