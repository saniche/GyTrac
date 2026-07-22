using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using GymTracker.Domain.Enums;

namespace GymTracker.Infrastructure.Persistence.Converters;

public class MuscleGroupConverter : ValueConverter<MuscleGroup, string>
{
    public MuscleGroupConverter() : base(
            value => value.ToString().ToLowerInvariant(),
            value => (MuscleGroup)Enum.Parse(typeof(MuscleGroup), value, true)
        )
    {

    }
}