
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class DistanceSetConfiguration : IEntityTypeConfiguration<DistanceSet>
{
    public void Configure(EntityTypeBuilder<DistanceSet> builder)
    {
        builder.HasKey(ds => ds.Id);
        builder.Property(ds => ds.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("gen_random_uuid()");
        builder.OwnsOne(ds => ds.Distance, d =>
        {
            d.Property(d => d.Value).HasColumnName("DistanceValue").IsRequired();
            d.Property(d => d.Unit).HasConversion<string>().HasColumnName("DistanceUnit").IsRequired();
        });
        builder.Property(ds => ds.Order).IsRequired();
        builder.Property(ds => ds.IsWarmup).IsRequired();
        builder.Property(ds => ds.Notes);
    }
}