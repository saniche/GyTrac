namespace GymTracker.Infrastructure.Persistence.Converters;

// change converter to class type converter
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using GymTracker.Domain.Enums;

public class MuscleGroupConverter : ValueConverter<MuscleGroup, string>
{
    public MuscleGroupConverter() : base(
            value => value.ToString().ToLower(),
            value => (MuscleGroup)Enum.Parse(typeof(MuscleGroup), value, true)
        )
    {

    }
}