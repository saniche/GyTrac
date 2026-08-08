using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using GymTracker.Infrastructure.Persistence;
using GymTracker.Infrastructure;

namespace GymTracker.Api.Integration.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GyTracDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a new DbContext using an in-memory database for testing
            services.AddDbContext<GyTracDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (GyTracDbContext)
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<GyTracDbContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();
            }
        });
    }
}