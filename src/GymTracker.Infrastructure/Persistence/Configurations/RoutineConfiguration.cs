using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures the Routine entity for Entity Framework Core.
/// </summary>
public class RoutineConfiguration : IEntityTypeConfiguration<Routine>
{
    public void Configure(EntityTypeBuilder<Routine> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(r => r.UserId).IsRequired();
        builder.Property(r => r.Name).IsRequired();
        builder.Property(r => r.Description);

        builder.HasMany(r => r.Exercises)
               .WithOne(re => re.Routine)
               .HasForeignKey(re => re.RoutineId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}