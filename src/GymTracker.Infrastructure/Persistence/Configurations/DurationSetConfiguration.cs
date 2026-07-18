
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class DurationSetConfiguration : IEntityTypeConfiguration<DurationSet>
{
    public void Configure(EntityTypeBuilder<DurationSet> builder)
    {
        builder.HasKey(ds => ds.Id);
        builder.Property(ds => ds.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("gen_random_uuid()");
        builder.OwnsOne(ds => ds.Duration, d =>
        {
            d.Property(d => d.Value).HasColumnName("DurationValue").IsRequired();
            d.Property(d => d.Unit).HasConversion<string>().HasColumnName("DurationUnit").IsRequired();
        });
        builder.Property(ds => ds.Order).IsRequired();
        builder.Property(ds => ds.IsWarmup).IsRequired();
        builder.Property(ds => ds.Notes);
    }
}