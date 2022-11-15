namespace RecipeManagement.Databases;


using Microsoft.EntityFrameworkCore;

public sealed class DatabaseHelper
{
    private readonly ILogger<DatabaseHelper> _logger;
    private readonly RecipesDbContext _context;

    public DatabaseHelper(ILogger<DatabaseHelper> logger, RecipesDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task MigrateAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while applying the database migrations.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Seed base data, if you want
    }
}