using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using GymTracker.Domain.Enums;

namespace GymTracker.Infrastructure.Persistence.Converters;

public class ExerciseTypeConverter : ValueConverter<ExerciseType, string>
{

    public ExerciseTypeConverter()
        : base(
            value => value.ToString().ToLowerInvariant(),
            value => (ExerciseType)Enum.Parse(typeof(ExerciseType), value, true)
        )
    {
    }
}
