using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class WorkoutProgramConfiguration : IEntityTypeConfiguration<WorkoutProgram>
{
    public void Configure(EntityTypeBuilder<WorkoutProgram> builder)
    {
        builder.HasKey(wp => wp.Id);
        builder.Property(wp => wp.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(wp => wp.UserId).IsRequired();
        builder.Property(wp => wp.Name).IsRequired();
        builder.Property(wp => wp.Description);

        builder.HasMany(wp => wp.Routines)
               .WithOne(pr => pr.WorkoutProgram)
               .HasForeignKey(pr => pr.WorkoutProgramId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}