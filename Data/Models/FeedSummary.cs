using System;

namespace StingrayFeeder.Data.Models
{
    // Keyless DTO to map results from dbo.GetFeedSummary stored procedure
    public class FeedSummary
    {
        public int StingrayId { get; set; }
        public string StingrayName { get; set; } = null!;
        public int TotalQuantity { get; set; }
        public DateTime? LastFeedTime { get; set; }
    }
}