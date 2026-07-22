using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class DistanceDurationSetConfiguration : IEntityTypeConfiguration<DistanceDurationSet>
{
    public void Configure(EntityTypeBuilder<DistanceDurationSet> builder)
    {
        builder.ToTable("DistanceDurationSets");
        builder.OwnsOne(ds => ds.Distance, d =>
        {
            d.Property(x => x.Value).HasColumnName("DistanceValue").IsRequired();
            d.Property(x => x.Unit).HasConversion<string>().HasColumnName("DistanceUnit").IsRequired();
        });

        builder.OwnsOne(ds => ds.Duration, d =>
        {
            d.Property(x => x.Value).HasColumnName("DurationValue").IsRequired();
            d.Property(x => x.Unit).HasConversion<string>().HasColumnName("DurationUnit").IsRequired();
        });
    }
}