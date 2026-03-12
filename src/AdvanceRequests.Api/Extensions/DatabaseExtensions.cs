using AdvanceRequests.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AdvanceRequests.Api.Extensions;

public static class DatabaseExtensions
{
    public static WebApplication ApplyMigrations(this WebApplication app)
    {
        if (app.Environment.IsEnvironment("Testing"))
            return app;

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        return app;
    }
}