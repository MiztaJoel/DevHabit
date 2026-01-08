using DevHabit.Api.Entities;

namespace DevHabit.Api.DTOs.Habits;

public sealed record HabitWithTagsDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required HabitType Type { get; init; }
    public required FrequencyDto Frequency { get; init; }
    public TargetDto Target { get; init; }
    public required HabitStatus Status { get; init; }
    public required bool IsArchived { get; init; }
    public required DateOnly? EndDate { get; init; }
    public MilestoneDto? Milestone { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? LastCompletedAtUtc { get; init; }
    public required string[] Tags { get; init; }
}


