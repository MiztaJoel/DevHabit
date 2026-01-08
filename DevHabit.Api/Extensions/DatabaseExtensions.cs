using DevHabit.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationAsync(this WebApplication app)
    {
       using IServiceScope scope= app.Services.CreateScope();
        await using ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Database Migraton applied successfully.");
        }
        catch(Exception ex) 
        {
            app.Logger.LogError(ex, "An error occurred while applying database migration");
            throw;
        }
    }
}
