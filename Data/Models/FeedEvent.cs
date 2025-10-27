using System;

namespace StingrayFeeder.Data.Models
{
    public class FeedEvent
    {
        public int Id { get; set; }

        public int StingrayId { get; set; }
        public Stingray Stingray { get; set; } = null!;

        public int FeedBatchId { get; set; }
        public FeedBatch FeedBatch { get; set; } = null!;

        public int Quantity { get; set; }         // number of items used from the batch
        public DateTime EventTime { get; set; }
        public Guid CorrelationId { get; set; } = Guid.NewGuid();
        public string? Notes { get; set; }

        // Optional FK to record which caretaker performed the feeding.
        // Nullable so historical FeedEvent rows remain if caretaker is removed.
        public int? CaretakerId { get; set; }
        public Caretaker? Caretaker { get; set; }
    }
}