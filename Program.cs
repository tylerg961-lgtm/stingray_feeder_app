using Microsoft.EntityFrameworkCore;
using StingrayFeeder.Data;
using Microsoft.EntityFrameworkCore.SqlServer; 

var builder = WebApplication.CreateBuilder(args);

// Add DbContext (SQL Server configured in appsettings.json)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ... other services like Razor Pages, logging, etc.
builder.Services.AddRazorPages();

var app = builder.Build();

// Ensure DB migrated at startup in Development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();
