# Backend Domain Model

This document summarizes the core domain model used by the backend. It is organized by enums, value objects, entities, relationships, and the constraints that are currently enforced in code.

## Enums

### DistanceUnit

- `Kilometers`
- `Miles`

### ExcerciseType

- `Compound`
- `Isolation`
- `Machine`
- `Bodyweight`
- `Cardio`
- `Strength`
- `Flexibility`
- `Balance`

### MuscleGroup

- `None`
- `Chest`
- `Back`
- `Shoulders`
- `Biceps`
- `Triceps`
- `Legs`
- `Core`
- `Calves`
- `Glutes`

### MuscleGroupType

- `UpperBody`
- `LowerBody`
- `FullBody`

### TimeUnit

- `Seconds`
- `Minutes`
- `Hours`

### UnitOfMeasurement

- `Kilograms`
- `Pounds`

## Value Objects

### Weight

- `Value : decimal`
- `Unit : UnitOfMeasurement`

Constraint:

- `Value` must be greater than zero.

### Duration

- `Value : int`
- `Unit : TimeUnit`

Constraint:

- `Value` must be greater than zero.

### Distance

- `Value : decimal`
- `Unit : DistanceUnit`

Constraint:

- `Value` must be greater than zero.

## Entities

### Exercise

Represents a reusable movement that can be logged in a workout.

- `Id : Guid`
- `Name : string`
- `Description : string?`
- `PrimaryMuscleGroup : MuscleGroup`
- `Type : ExcerciseType`

Constraints:

- `Id` is provided by the caller.
- `Description` is optional.
- The current code does not validate `Name`, but it should be treated as required.

### Set

Abstract base type for all logged set variations.

- `Id : Guid`
- `ExerciseLogId : Guid`
- `Order : int`
- `IsWarmup : bool`
- `Notes : string?`

Constraints:

- `Order` is used to preserve set sequence within an exercise log.
- Each set belongs to exactly one `ExerciseLog`.

### WeightSet

Strength set with load and repetitions.

- `Weight : Weight`
- `Reps : int`

### DurationSet

Timed set such as holds, intervals, or isometrics.

- `Duration : Duration`

### DistanceSet

Distance-based set such as running or rowing.

- `Distance : Distance`

### DistanceDurationSet

Distance-based set that also stores duration, typically for cardio tracking.

- `Distance : Distance`
- `Duration : Duration`

### ExerciseLog

Captures one exercise performed during a workout session.

- `Id : Guid`
- `WorkoutSessionId : Guid`
- `ExerciseId : Guid`
- `Order : int`
- `Sets : IReadOnlyCollection<Set>`

Relationships:

- Belongs to one `WorkoutSession`.
- References one `Exercise`.
- Owns many `Set` records.

Constraints:

- Sets are appended in order, starting from 1.
- The in-memory collection preserves the order in which sets are added.

### Routine

Reusable ordered list of exercises for a user.

- `Id : Guid`
- `UserId : Guid`
- `Name : string`
- `Description : string?`
- `Exercises : IReadOnlyCollection<RoutineExercise>`

Constraints:

- `Description` is optional.
- Exercises are stored in order through the join entity.

### RoutineExercise

Join entity between `Routine` and `Exercise`.

- `RoutineId : Guid`
- `ExerciseId : Guid`
- `Order : int`

Constraints:

- Composite key is `RoutineId + ExerciseId`.
- `Order` defines exercise sequencing inside the routine.

### WorkoutProgram

Higher-level container that groups routines for a user.

- `Id : Guid`
- `UserId : Guid`
- `Name : string`
- `Description : string?`
- `Routines : IReadOnlyCollection<ProgramRoutine>`

Constraints:

- `Description` is optional.
- Routines are stored in order through the join entity.

### ProgramRoutine

Join entity between `WorkoutProgram` and `Routine`.

- `WorkoutProgramId : Guid`
- `RoutineId : Guid`
- `Order : int`

Constraints:

- Composite key is `WorkoutProgramId + RoutineId`.
- `Order` defines routine sequencing inside the program.

### WorkoutSession

Represents a user workout instance.

- `Id : Guid`
- `UserId : Guid`
- `StartedAt : DateTimeOffset`
- `CompletedAt : DateTimeOffset?`
- `RoutineId : Guid?`
- `Notes : string?`
- `ExerciseLogs : IReadOnlyCollection<ExerciseLog>`

Relationships:

- Belongs to one user.
- Can optionally be linked to a routine.
- Owns many `ExerciseLog` records.

Constraints:

- `StartedAt` is required.
- `CompletedAt` is optional until the workout is finished.
- `RoutineId` is optional because a session can be freestyle.

## Domain Notes

- `ExerciseLog` is the bridge between a workout session and the selected exercise.
- `Set` has multiple specializations so each training style can store the metrics it needs without forcing unrelated fields.
- The current code enforces numeric positivity for value objects, but it does not yet enforce all string, ordering, or uniqueness rules in constructors.
- If stricter validation is needed later, the most useful additions would be non-empty names, positive ordering values, and uniqueness checks for ordered collections.
