using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class ExerciseLogConfiguration : IEntityTypeConfiguration<ExerciseLog>
{
    public void Configure(EntityTypeBuilder<ExerciseLog> builder)
    {
        builder.HasKey(el => el.Id);
        builder.Property(el => el.Id)
                  .ValueGeneratedOnAdd()
                  .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(el => el.WorkoutSessionId).IsRequired();
        builder.Property(el => el.ExerciseId).IsRequired();
        builder.Property(el => el.Order).IsRequired();

        builder.HasOne(el => el.WorkoutSession)
               .WithMany(ws => ws.ExerciseLogs)
               .HasForeignKey(el => el.WorkoutSessionId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(el => el.Exercise)
               .WithMany()
               .HasForeignKey(el => el.ExerciseId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(el => el.Sets)
               .WithOne()
               .HasForeignKey(s => s.ExerciseLogId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}