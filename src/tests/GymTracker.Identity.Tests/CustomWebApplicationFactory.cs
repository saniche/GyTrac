using GymTracker.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace GymTracker.Identity.Tests;

/// <summary>
/// Spins up the Identity API with an isolated in-memory database per factory instance.
/// Use as IClassFixture&lt;CustomWebApplicationFactory&gt; so each test class gets its own DB.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Unique DB name per factory instance → each test class has an isolated store
    private readonly string _dbName = $"IdentityTestDb_{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Use "Test" environment so the Development-only block in Program.cs
        // (which calls GetPendingMigrations → throws on in-memory) is skipped.
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Secret"] = "test-secret-key-must-be-at-least-32-characters-long!",
                ["JwtSettings:Issuer"] = "GymTracker.Identity.Tests",
                ["JwtSettings:Audience"] = "GymTracker.Api.Tests",
                ["JwtSettings:ExpiresInMinutes"] = "60",
                ["JwtSettings:RefreshExpiresInDays"] = "7"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Replace the Postgres DbContext with an in-memory one
            services.RemoveAll<DbContextOptions<IdentityDbContext>>();

            services.AddDbContext<IdentityDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Ensure the in-memory schema is ready before any test runs
        using var scope = host.Services.CreateScope();
        scope.ServiceProvider
             .GetRequiredService<IdentityDbContext>()
             .Database.EnsureCreated();

        return host;
    }
}
