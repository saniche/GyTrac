using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace GymTracker.Common.Extensions;

public static class HostExtensions
{

    /// <summary>
    /// Ensures the database is created and seeded with initial data. This should be called after building the WebApplication but before running it.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="host"></param>
    /// <param name="seedData"></param>
    /// <returns></returns>
    public static async Task<IHost> CreateAndSeedDatabaseAsync<TContext>(this IHost host, Func<TContext, IServiceProvider, Task> seedData) where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();

        if (db.Database.GetPendingMigrations().Any())
        {
            await db.Database.MigrateAsync();
        }
        else
        {
            await db.Database.EnsureCreatedAsync();
        }

        if (seedData != null)
        {
            await seedData(db, scope.ServiceProvider);
        }
        return host;
    }

    /// <summary>
    /// Ensures the database is created and seeded with initial data. This should be called after building the WebApplication but before running it.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <param name="builder"></param>
    /// <param name="seedData"></param>
    /// <returns></returns>
    public static WebApplication CreateAndSeedDatabase<TContext>(this WebApplication builder, Action<TContext, IServiceProvider> seedData) where TContext : DbContext
    {

        //Ensure database is created and seeded
        using var scope = builder.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TContext>();

        if (db.Database.GetPendingMigrations().Any())
        {
            db.Database.Migrate();
        }
        else
        {
            db.Database.EnsureCreated();
        }

        if (seedData != null)
        {
            seedData(db, scope.ServiceProvider);
        }
        return builder;
    }
}