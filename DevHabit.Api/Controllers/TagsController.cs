
using DevHabit.Api.Database;
using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.DTOs.Tags;
using DevHabit.Api.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Controllers;

[ApiController]
[Route("tags")]
public sealed class TagsController(ApplicationDbContext dbContext):ControllerBase
{
    [HttpGet]

    public async Task<ActionResult<TagsCollectionDto>> GetTags()
    {
        List<TagDto> tags = await dbContext.
              Tags
              .Select(TagsMappings.ProjectToDto()).ToListAsync();

        var tagsCollections = new TagsCollectionDto
        {
            Items = tags,
        };

        return Ok(tagsCollections);
    }

    [HttpGet("{Id}")]
    public async Task<ActionResult<TagDto>> GetTag(string id)
    {
        TagDto? tag = await dbContext
                        .Tags
                        .Where(t => t.Id == id)
                        .Select(TagsMappings.ProjectToDto())
                        .SingleOrDefaultAsync();
        if (tag is null)
        {
            return NotFound();
        }
        return Ok(tag);
    }

    [HttpPost]
    public async Task<ActionResult<TagDto>> CreateTag(CreateTagDto createTagDto,IValidator<CreateTagDto> validator,ProblemDetailsFactory problemDetailsFactory)
    {
        ValidationResult validationResult = await validator.ValidateAsync(createTagDto);

        if (!validationResult.IsValid)
        {
            ProblemDetails problem = problemDetailsFactory.CreateProblemDetails(HttpContext, StatusCodes.Status400BadRequest);
            problem.Extensions.Add("error", validationResult.ToDictionary());
            return BadRequest(problem);
        }
       
        Tag tag = createTagDto.ToEntity();
        if (await dbContext.Tags.AnyAsync(t => t.Name == tag.Name))
        {
            //return Conflict($"The tag '{tag.Name}' already exist");
            return Problem(
                detail: $"The tag '{tag.Name}' already exist",
                statusCode:StatusCodes.Status409Conflict
                );
        }

        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync();

        TagDto tagDto = tag.ToDto();

        return CreatedAtAction(nameof(GetTag), new { id = tagDto.Id}, tagDto);
    }
    [HttpPut("{id}") ]
    public async Task<ActionResult> UpdateTag(string id, UpdateTagDto updateTagDto)
    {
        Tag? tag = await dbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);

        if (tag is null)
        {
            return NotFound();
        }

        tag.UpdateFromDto(updateTagDto);

        await dbContext.SaveChangesAsync();

        return NoContent();

    }
}
