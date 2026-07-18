using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the ProgramRoutine entity for Entity Framework Core.
/// </summary>
public class ProgramRoutineConfiguration : IEntityTypeConfiguration<ProgramRoutine>
{
    public void Configure(EntityTypeBuilder<ProgramRoutine> builder)
    {
        builder.HasKey(pr => new { pr.WorkoutProgramId, pr.RoutineId });
        builder.Property(pr => pr.Order).IsRequired();

        builder.HasOne(pr => pr.WorkoutProgram)
               .WithMany(wp => wp.Routines)
               .HasForeignKey(pr => pr.WorkoutProgramId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pr => pr.Routine)
               .WithMany()
               .HasForeignKey(pr => pr.RoutineId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}