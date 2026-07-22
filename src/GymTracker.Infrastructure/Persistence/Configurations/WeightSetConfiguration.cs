using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class WeightSetConfiguration : IEntityTypeConfiguration<WeightSet>
{
    public void Configure(EntityTypeBuilder<WeightSet> builder)
    {
        builder.ToTable("WeightSets");
        builder.OwnsOne(ws => ws.Weight, p =>
        {
            p.Property(w => w.Value).HasColumnName("Weight").IsRequired();
            p.Property(w => w.Unit).HasConversion<string>().HasColumnName("WeightUnit").IsRequired();
        });
        builder.Property(ws => ws.Reps).IsRequired();
    }
}