using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Renligou.Core.Domain.EnterpriseContext.Repo;
using Renligou.Core.Domain.EventingContext.Repo;
using Renligou.Core.Shared.Bus;
using Renligou.Core.Shared.EFCore;
using Renligou.Core.Shared.Ddd;

namespace Renligou.Api.Boss.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Here you can swap real services with mocks if needed
            // For integration tests, we might want to keep most services but mock the database
            // or use an in-memory database.
            
            // For now, let's assume we want to mock the command bus or the repository 
            // to focus on the controller logic and full chain without real DB connectivity.
            
            _logger.LogInformation("Configuring services for test...");
        });
    }

    private static readonly ILogger<CustomWebApplicationFactory<TProgram>> _logger = 
        LoggerFactory.Create(b => b.AddConsole()).CreateLogger<CustomWebApplicationFactory<TProgram>>();
}
