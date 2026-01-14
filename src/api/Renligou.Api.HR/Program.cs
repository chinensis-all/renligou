using IGeekFan.AspNetCore.Knife4jUI;
using Renligou.Api.HR.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseKnife4UI(options =>
    {
        options.RoutePrefix = "api-docs";
        options.SwaggerEndpoint("../swagger/v1/swagger.json", "Renligou HR API V1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
