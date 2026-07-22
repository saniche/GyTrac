using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;
using GymTracker.Domain.ValueObjects;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class SetConfiguration : IEntityTypeConfiguration<Set>
{
    public void Configure(EntityTypeBuilder<Set> builder)
    {
        builder.ToTable("Sets");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(s => s.Order).IsRequired();
        builder.Property(s => s.IsWarmup).IsRequired();
        builder.Property(s => s.Notes);
    }
}