using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GymTracker.Domain.Entities;

namespace GymTracker.Infrastructure.Persistence.Configurations;

public class WorkoutSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
{
    public void Configure(EntityTypeBuilder<WorkoutSession> builder)
    {
        builder.HasKey(ws => ws.Id);
        builder.Property(ws => ws.Id)
               .ValueGeneratedOnAdd()
               .HasDefaultValueSql("gen_random_uuid()");
        builder.Property(ws => ws.UserId).IsRequired();
        builder.Property(ws => ws.StartedAt).IsRequired();
        builder.Property(ws => ws.CompletedAt);
        builder.Property(ws => ws.Notes).HasMaxLength(1000);
        builder.Property(ws => ws.RoutineId);
    }
}