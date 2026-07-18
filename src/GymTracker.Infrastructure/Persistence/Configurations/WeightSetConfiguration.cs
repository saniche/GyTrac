
namespace GymTracker.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

public class WeightSetConfiguration : IEntityTypeConfiguration<WeightSet>
{
    public void Configure(EntityTypeBuilder<WeightSet> builder)
    {
        builder.HasKey(ws => ws.Id);
        builder.Property(ws => ws.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("gen_random_uuid()");
        builder.OwnsOne(ws => ws.Weight, p =>
        {
            p.Property(w => w.Value).HasColumnName("Weight").IsRequired();
            p.Property(w => w.Unit).HasConversion<string>().HasColumnName("WeightUnit").IsRequired();
        });
        builder.Property(ws => ws.Reps).IsRequired();
        builder.Property(ws => ws.Order).IsRequired();
        builder.Property(ws => ws.IsWarmup).IsRequired();
        builder.Property(ws => ws.Notes);
    }
}