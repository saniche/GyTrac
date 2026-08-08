using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Infrastructure.Persistence.Converters;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("gen_random_uuid()");

        builder.HasIndex(e => e.Name)
               .HasDatabaseName("IX_Exercise_Name")
               .HasFilter(null)
               .IsUnique(false)
               .HasAnnotation("MaxLength", 100);

        builder.Property(e => e.Description);
        builder.Property(e => e.Type).HasConversion<ExerciseTypeConverter>().IsRequired();
        builder.Property(e => e.PrimaryMuscleGroup).HasConversion<MuscleGroupConverter>().IsRequired();
    }
}