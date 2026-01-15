using System.Linq.Dynamic.Core;
namespace DevHabit.Api.Services.Sorting;

public sealed record  SortMapping(string SortField, string PropertyName, bool Reverse = false);

public interface  ISortMappingDefinition;

#pragma warning disable S2326 // Unused type parameters should be removed
public sealed class SortMappingDefinition<TSource, TDestination> : ISortMappingDefinition
#pragma warning restore S2326 // Unused type parameters should be removed
{
    public required SortMapping[] Mappings { get; init; }
} 

public sealed class SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions)
{ 
    public SortMapping[] GetMappings<TSource, TDestination>()
    {
        SortMappingDefinition<TSource, TDestination> sortMappingDefinition = sortMappingDefinitions.
                    OfType<SortMappingDefinition<TSource, TDestination>>()
                    .FirstOrDefault();
        if(sortMappingDefinition is null)
        {
            throw new InvalidOperationException(
                $"The mapping from '{typeof(TSource).Name}' into'{typeof(TDestination).Name} isn't defined."
                );
        }
        return sortMappingDefinition.Mappings;
    }
}

internal static class QueryableExtension
{
    public static IQueryable<T> ApplySort<T>(
        this IQueryable<T> query,
        string? sort,
        SortMapping[] mappings,
        string defaultOrderBy = "Id"
        )
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return query.OrderBy(defaultOrderBy);
        }

        string[] sortFields = sort.Split(',')
            .Select(s=>s.Trim())
            .Where(s=>!string.IsNullOrWhiteSpace(s))
            .ToArray();

        var orderByParts = new List<string>();

        foreach (string field in sortFields)
        { 
            (string sortField, bool isDescending) = ParseSortField(field);

            SortMapping mapping = mappings.First(m =>
                m.SortField.Equals(sortField, StringComparison.OrdinalIgnoreCase));

            string direction = (isDescending, mapping.Reverse) switch
            {
                (false,false) => "ASC",
                (false,true) => "DESC",
                (true,false) => "DESC",
                (true,true) => "ASC"
            };

            orderByParts.Add($"{mapping.PropertyName} {direction}");
        }
        string orderby = string.Join(",", orderByParts);
        return query.OrderBy(orderby);  
    }

    private static (string SortField,bool IsDescending) ParseSortField(string field)
    {
        string[] parts = field.Split(' ');
        string sortField = parts[0];
        bool isDescending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
        
        return (sortField, isDescending);
    }

};
