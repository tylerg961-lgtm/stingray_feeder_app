using Microsoft.EntityFrameworkCore;
using StingrayFeeder.Data.Models;

namespace StingrayFeeder.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Core entities required for upcoming weeks
        public DbSet<FeedEvent> FeedEvents { get; set; } = null!;
        public DbSet<FeedBatch> FeedBatches { get; set; } = null!;
        public DbSet<Caretaker> Caretakers { get; set; } = null!;
        public DbSet<Stingray> Stingrays { get; set; } = null!;
        public DbSet<Fish> Fish { get; set; } = null!;
    }
}