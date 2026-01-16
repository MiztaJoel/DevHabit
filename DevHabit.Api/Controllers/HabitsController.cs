using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DevHabit.Api.Services.Sorting;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("habits")]
public sealed class HabitsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits([FromQuery] HabitQueryParameters query, SortMappingProvider sortMappingProvider)
    {
        if (!sortMappingProvider.ValidateMapping<HabitDto, Habit>(query.Sort)){

            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                detail: $"The provided sort parameter isn't valid: '{query.Sort}'"
                );
        }
        query.Search ??= query.Search?.Trim().ToLower();
        // IQueryable<Habit> query =  dbContext.Habits;

        SortMapping[] sortMapping = sortMappingProvider.GetMappings<HabitDto, Habit>();

        List<HabitDto> habits = await dbContext.Habits
            .Where(h =>query.Search == null || 
                                h.Name.Contains(query.Search) ||
                                h.Description != null && h.Description.Contains(query.Search))  
            .Where(h=>query.Type == null || h.Type == query.Type)
            .ApplySort(query.Sort,sortMapping)
            .Where(h=> query.Status == null || h.Status == query.Status)
            .Select(HabitQueries.ProjectToDto())
            .ToListAsync();

        var habitCollectionDto = new HabitsCollectionDto
        {
            Data = habits
        };
        return Ok(habitCollectionDto);

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HabitWithTagsDto>> GetHabit(string id)
    {
        HabitWithTagsDto? habit = await dbContext
                    .Habits
                    .Where(h => h.Id == id)
                    .Select(HabitQueries.ProjectToDtoWithTags())
                    .FirstOrDefaultAsync();
        if (habit is null)
        {
            return NotFound();
        }
        return Ok(habit);
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto>> CreateHabit(CreateHabitDto createHabitDto,IValidator<CreateHabitDto> validator)
    {

       // ValidationResult validationResult = await validator.ValidateAsync(createHabitDto);

        //if (!validationResult.IsValid)
        //{
        //    return BadRequest(validationResult.ToDictionary());
        //}
        await validator.ValidateAndThrowAsync(createHabitDto);

        Habit habit = createHabitDto.ToEntity();
        dbContext.Habits.Add(habit);
        await dbContext.SaveChangesAsync();

        HabitDto habitDto = habit.ToDto();
       
        return CreatedAtAction(nameof(GetHabit),new {id=habitDto.Id},habitDto);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(habit => habit.Id == id);

        if(habit is null)
        {
            return NotFound(); 
        }
        habit.UpdateFromDto(updateHabitDto);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchHabit(string id, JsonPatchDocument<HabitDto> patchDocument)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null)
        {
            return NotFound();
        }

        HabitDto habitDto = habit.ToDto();

        patchDocument.ApplyTo(habitDto,ModelState);
        //if (!ModelState.IsValid)
        if (!TryValidateModel(habitDto))
        {
            return ValidationProblem(ModelState);
        }

        habit.Name = habitDto.Name;
        habit.Description = habitDto.Description;
        habit.UpdatedAtUtc = habitDto.UpdatedAtUtc;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteHabit(string id)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null)
        {
            return NotFound();
        }

        dbContext.Habits.Remove(habit);
        await dbContext.SaveChangesAsync();

        return NoContent();

    }
}
