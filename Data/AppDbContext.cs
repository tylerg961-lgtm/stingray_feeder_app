using Microsoft.EntityFrameworkCore;
using StingrayFeeder.Data.Models;

namespace StingrayFeeder.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Stingray> Stingrays { get; set; } = null!;
        public DbSet<Fish> Fish { get; set; } = null!;
        public DbSet<FeedBatch> FeedBatches { get; set; } = null!;
        public DbSet<FeedEvent> FeedEvents { get; set; } = null!;
        public DbSet<Caretaker> Caretakers { get; set; } = null!; // newly added

        // Keyless mapping to support stored-proc result
        public DbSet<FeedSummary> FeedSummaries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Stingray
            modelBuilder.Entity<Stingray>(b =>
            {
                b.HasKey(s => s.Id);
                b.Property(s => s.Name).IsRequired().HasMaxLength(100);
                b.Property(s => s.Species).HasMaxLength(100);
                b.HasIndex(s => s.Name);
            });

            // Fish
            modelBuilder.Entity<Fish>(b =>
            {
                b.HasKey(f => f.Id);
                b.Property(f => f.Species).IsRequired().HasMaxLength(100);
                b.HasIndex(f => f.Species);
            });

            // FeedBatch
            modelBuilder.Entity<FeedBatch>(b =>
            {
                b.HasKey(fb => fb.Id);
                b.Property(fb => fb.UnitWeightGrams).HasPrecision(9, 3);
                b.HasOne(fb => fb.Fish)
                 .WithMany(f => f.Batches)
                 .HasForeignKey(fb => fb.FishId)
                 .OnDelete(DeleteBehavior.Restrict);
                b.HasIndex(fb => fb.BatchDate);
            });

            // Caretaker (new)
            modelBuilder.Entity<Caretaker>(b =>
            {
                b.HasKey(c => c.Id);
                b.Property(c => c.Name).IsRequired().HasMaxLength(120);
                b.Property(c => c.Email).HasMaxLength(200);
                b.HasIndex(c => c.Name);
            });

            // FeedEvent
            modelBuilder.Entity<FeedEvent>(b =>
            {
                b.HasKey(fe => fe.Id);
                b.Property(fe => fe.EventTime).IsRequired();
                b.Property(fe => fe.Quantity).IsRequired();
                b.Property(fe => fe.CorrelationId).IsRequired();

                b.HasOne(fe => fe.Stingray)
                 .WithMany(s => s.FeedEvents)
                 .HasForeignKey(fe => fe.StingrayId)
                 .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(fe => fe.FeedBatch)
                 .WithMany(fb => fb.FeedEvents)
                 .HasForeignKey(fe => fe.FeedBatchId)
                 .OnDelete(DeleteBehavior.Restrict);

                // Optional relationship to Caretaker; if caretaker removed, leave FK null
                b.HasOne(fe => fe.Caretaker)
                 .WithMany(c => c.FeedEvents)
                 .HasForeignKey(fe => fe.CaretakerId)
                 .OnDelete(DeleteBehavior.SetNull);

                b.HasIndex(fe => fe.EventTime);
            });

            // Keyless entity mapping for stored-proc results
            modelBuilder.Entity<FeedSummary>(b =>
            {
                b.HasNoKey();
                b.ToView(null); // not mapped to a database view/table
                b.Property(fs => fs.StingrayName).HasMaxLength(100);
            });

            // Basic seed data for local dev/testing
            modelBuilder.Entity<Stingray>().HasData(
                new Stingray { Id = 1, Name = "Mango", Species = "Atlantic Stingray", DateOfBirth = new System.DateTime(2020, 3, 5) },
                new Stingray { Id = 2, Name = "Cassia", Species = "Bluespotted Ribbontail", DateOfBirth = new System.DateTime(2019, 8, 12) }
            );

            modelBuilder.Entity<Fish>().HasData(
                new Fish { Id = 1, Species = "Herring", TypicalSizeGrams = 150, Source = "Local Supplier" },
                new Fish { Id = 2, Species = "Sardine", TypicalSizeGrams = 25, Source = "Supplier B" }
            );

            modelBuilder.Entity<FeedBatch>().HasData(
                new FeedBatch { Id = 1, FishId = 1, BatchDate = System.DateTime.UtcNow.AddDays(-3), Quantity = 50, UnitWeightGrams = 150, Source = "Local Supplier" },
                new FeedBatch { Id = 2, FishId = 2, BatchDate = System.DateTime.UtcNow.AddDays(-1), Quantity = 200, UnitWeightGrams = 25, Source = "Supplier B" }
            );

            // Seed caretakers (new)
            modelBuilder.Entity<Caretaker>().HasData(
                new Caretaker { Id = 1, Name = "Alex Rivera", Email = "alex@example.com", Phone = "555-0101" },
                new Caretaker { Id = 2, Name = "Sam Taylor", Email = "sam@example.com", Phone = "555-0202" }
            );

            modelBuilder.Entity<FeedEvent>().HasData(
                new FeedEvent { Id = 1, StingrayId = 1, FeedBatchId = 1, Quantity = 3, EventTime = System.DateTime.UtcNow.AddDays(-2), CorrelationId = System.Guid.NewGuid(), CaretakerId = 1 },
                new FeedEvent { Id = 2, StingrayId = 2, FeedBatchId = 2, Quantity = 8, EventTime = System.DateTime.UtcNow.AddDays(-1), CorrelationId = System.Guid.NewGuid(), CaretakerId = 2 }
            );
        }
    }
}