
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class DistanceSetConfiguration : IEntityTypeConfiguration<DistanceSet>
{
    public void Configure(EntityTypeBuilder<DistanceSet> builder)
    {
        builder.ToTable("DistanceSets");
        builder.OwnsOne(ds => ds.Distance, d =>
        {
            d.Property(d => d.Value).HasColumnName("DistanceValue").IsRequired();
            d.Property(d => d.Unit).HasConversion<string>().HasColumnName("DistanceUnit").IsRequired();
        });
    }
}