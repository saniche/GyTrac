using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class RoutineExerciseConfiguration : IEntityTypeConfiguration<RoutineExercise>
{
    public void Configure(EntityTypeBuilder<RoutineExercise> builder)
    {
        builder.HasKey(re => new { re.RoutineId, re.ExerciseId });
        builder.Property(re => re.Order).IsRequired();

        builder.HasOne(re => re.Routine)
               .WithMany(r => r.Exercises)
               .HasForeignKey(re => re.RoutineId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(re => re.Exercise)
               .WithMany()
               .HasForeignKey(re => re.ExerciseId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}