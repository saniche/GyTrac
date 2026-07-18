namespace GymTracker.Infrastructure.Persistence.Converters;

// change converter to class type converter
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using GymTracker.Domain.Enums;
public class ExerciseTypeConverter : ValueConverter<ExerciseType, string>
{

    public ExerciseTypeConverter()
        : base(
            value => value.ToString().ToLower(),
            value => (ExerciseType)Enum.Parse(typeof(ExerciseType), value, true)
        )
    {
    }
}
