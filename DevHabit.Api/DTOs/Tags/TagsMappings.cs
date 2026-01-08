using DevHabit.Api.DTOs.Habits;
using DevHabit.Api.Entities;

namespace DevHabit.Api.DTOs.Tags;

internal static class TagsMappings
{
    public static System.Linq.Expressions.Expression<Func<Tag, TagDto>> ProjectToDto()
    {
        return t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
            CreatedAtUtc = t.CreatedAtUtc,
            Description = t.Description,
            UpdatedAtUtc = t.UpdatedAtUtc,
        };
    }

    public static TagDto ToDto(this Tag tag)
        {
            return  new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                CreatedAtUtc = tag.CreatedAtUtc,
                Description = tag.Description,
                UpdatedAtUtc = tag.UpdatedAtUtc,
            };

        }

    public static Tag ToEntity(this CreateTagDto dto)
    {
        Tag tag = new()
        {
            Id = $"t_{Guid.CreateVersion7()}",
            Name = dto.Name,
            Description = dto.Description,
            CreatedAtUtc = DateTime.UtcNow,

        };
       return tag;
        
    }



    public static void UpdateFromDto(this Tag tag, UpdateTagDto dto)
    {
        tag.Name = dto.Name;
        tag.Description = dto.Description;
        tag.UpdatedAtUtc = DateTime.UtcNow;

    }

}
