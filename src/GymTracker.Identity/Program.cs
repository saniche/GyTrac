using GymTracker.Identity.Data;
using GymTracker.Identity.Services;
using Microsoft.EntityFrameworkCore;
using GymTracker.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection")));

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.CreateAndSeedDatabaseAsync<IdentityDbContext>((db, sp) =>
    {
        // Seed initial data if necessary
        return Task.CompletedTask;
    });

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHsts();

}


app.UseHttpsRedirection();
app.MapControllers();
app.Run();
