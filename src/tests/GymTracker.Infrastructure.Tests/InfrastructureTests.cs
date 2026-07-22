using GymTracker.Domain.Entities;
using GymTracker.Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.Infrastructure.Tests;

public class InfrastructureTests
{
    [Fact]
    public void GyTracDbContext_applies_expected_model_configuration()
    {
        using var context = CreateContext();

        var exerciseEntity = context.Model.FindEntityType(typeof(Exercise));
        Assert.NotNull(exerciseEntity);

        var exerciseId = exerciseEntity!.FindProperty(nameof(Exercise.Id));
        var exerciseType = exerciseEntity.FindProperty(nameof(Exercise.Type));
        var muscleGroup = exerciseEntity.FindProperty(nameof(Exercise.PrimaryMuscleGroup));

        Assert.NotNull(exerciseId);
        Assert.NotNull(exerciseType);
        Assert.NotNull(muscleGroup);

        Assert.Equal(ValueGenerated.OnAdd, exerciseId!.ValueGenerated);
        Assert.Equal("gen_random_uuid()", exerciseId.GetDefaultValueSql());
        Assert.IsType<ExerciseTypeConverter>(exerciseType!.GetValueConverter());
        Assert.IsType<MuscleGroupConverter>(muscleGroup!.GetValueConverter());

        var routineExerciseEntity = context.Model.FindEntityType(typeof(RoutineExercise));
        Assert.NotNull(routineExerciseEntity);

        var routineExerciseKey = routineExerciseEntity!.FindPrimaryKey();
        Assert.NotNull(routineExerciseKey);
        Assert.Collection(
            routineExerciseKey!.Properties,
            property => Assert.Equal(nameof(RoutineExercise.RoutineId), property.Name),
            property => Assert.Equal(nameof(RoutineExercise.ExerciseId), property.Name));

        var routineForeignKey = routineExerciseEntity.GetForeignKeys()
            .Single(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(Routine));
        Assert.Equal(DeleteBehavior.Cascade, routineForeignKey.DeleteBehavior);

        var exerciseForeignKey = routineExerciseEntity.GetForeignKeys()
            .Single(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(Exercise));
        Assert.Equal(DeleteBehavior.Restrict, exerciseForeignKey.DeleteBehavior);

        var exerciseLogEntity = context.Model.FindEntityType(typeof(ExerciseLog));
        Assert.NotNull(exerciseLogEntity);

        var sessionForeignKey = exerciseLogEntity!.GetForeignKeys()
            .Single(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(WorkoutSession));
        Assert.Equal(DeleteBehavior.Cascade, sessionForeignKey.DeleteBehavior);

        var exerciseLogForeignKey = exerciseLogEntity.GetForeignKeys()
            .Single(foreignKey => foreignKey.PrincipalEntityType.ClrType == typeof(Exercise));
        Assert.Equal(DeleteBehavior.Restrict, exerciseLogForeignKey.DeleteBehavior);
    }

    [Fact]
    public void AddInfrastructure_registers_the_npgsql_db_context()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=GymTracker.Tests;Username=test;Password=test"
            })
            .Build();

        services.AddInfrastructure(configuration);

        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<GyTracDbContext>();
        Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", dbContext.Database.ProviderName);
    }

    private static GyTracDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GyTracDbContext>()
            .UseInMemoryDatabase($"ModelTests_{Guid.NewGuid():N}")
            .Options;

        return new GyTracDbContext(options);
    }
}
