using System;
using System.Collections.Generic;

namespace StingrayFeeder.Data.Models
{
    public class FeedBatch
    {
        public int Id { get; set; }
        public int FishId { get; set; }
        public Fish Fish { get; set; } = null!;

        public DateTime BatchDate { get; set; }
        public int Quantity { get; set; }              // number of fish/items in the batch
        public double UnitWeightGrams { get; set; }    // per-item weight, grams
        public string? Source { get; set; }            // vendor / source string
        public string? Notes { get; set; }

        public ICollection<FeedEvent> FeedEvents { get; set; } = new List<FeedEvent>();
    }
}