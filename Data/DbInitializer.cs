using System;
using Microsoft.EntityFrameworkCore;

namespace StingrayFeeder.Data
{
    public static class DbInitializer
    {
        // Call at app startup to ensure DB exists, schema migrated and seed applied.
        public static void EnsureDatabase(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            // Apply migrations (creates DB if not exists)
            context.Database.Migrate();
            // No further seed is required because modelBuilder.HasData added initial rows.
        }
    }
}