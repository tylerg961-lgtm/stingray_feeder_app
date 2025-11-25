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

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();
