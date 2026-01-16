using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DevHabit.Api.DTOs.Habits;

public sealed class HabitQueryParameters
{
    [FromQuery(Name ="q")]
    public string? Search { get; set; }
    public HabitStatus? Status { get; init; }    
    public HabitType? Type { get; init; }    
    public string? Sort { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
