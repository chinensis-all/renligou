using IGeekFan.AspNetCore.Knife4jUI;
using Renligou.Api.Boss.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
     // 自定义文档信息
     options.AddDocumentTransformer((document, context, cancellationToken) =>
     {
         document.Info.Title = "Renligou Boss API";
         document.Info.Version = "v1";
         document.Info.Description = "Boss接口文档";
         return Task.CompletedTask;
     });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo { Title = "Renligou Boss API", Version = "v1" });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

builder.Services.AddMysql(
    builder.Configuration.GetConnectionString("Mysql")!,
    builder.Environment.IsDevelopment(),
    builder.Environment.EnvironmentName
);

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseKnife4UI(options => 
    {
        options.RoutePrefix = "api-docs";
        options.SwaggerEndpoint("../swagger/v1/swagger.json", "Renligou Boss API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
