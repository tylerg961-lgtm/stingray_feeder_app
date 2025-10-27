using System.Collections.Generic;

namespace StingrayFeeder.Data.Models
{
    public class Fish
    {
        public int Id { get; set; }
        public string Species { get; set; } = null!;
        public double? TypicalSizeGrams { get; set; }
        public string? Source { get; set; }
        public string? Notes { get; set; }

        public ICollection<FeedBatch> Batches { get; set; } = new List<FeedBatch>();
    }
}