using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using StingrayFeeder.Data;
using StingrayFeeder.Services;
using Microsoft.EntityFrameworkCore.SqlServer; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IFeedingService, FeedingService>();

builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Lightweight health endpoint that performs a real dependency check (database).
app.MapGet("/healthz", async (IServiceProvider services, ILogger<Program> logger) =>
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var sw = Stopwatch.StartNew();
    bool dbOk = false;
    string dbDetails = "OK";

    try
    {
        dbOk = await db.Database.CanConnectAsync();
        if (!dbOk)
        {
            dbDetails = "Unable to connect to database.";
            logger.LogWarning("Health check: Database.CanConnectAsync returned false.");
        }
    }
    catch (Exception ex)
    {
        // Do not expose secrets. Keep message short and truncated to avoid leaking sensitive info.
        var msg = ex.Message ?? "Unexpected error";
        dbDetails = msg.Length > 200 ? msg.Substring(0, 200) + "..." : msg;
        logger.LogWarning(ex, "Health check: Exception while checking database connectivity.");
    }
    finally
    {
        sw.Stop();
    }

    var overallHealthy = dbOk;

    var payload = new
    {
        status = overallHealthy ? "Healthy" : "Unhealthy",
        checks = new
        {
            database = new
            {
                status = dbOk ? "Healthy" : "Unhealthy",
                details = dbDetails,
                elapsedMs = sw.ElapsedMilliseconds
            }
        },
        timestamp = DateTime.UtcNow.ToString("o")
    };

    return Results.Json(payload, statusCode: overallHealthy ? 200 : 503);
})
.WithName("Health")
.WithTags("Diagnostics");

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();
