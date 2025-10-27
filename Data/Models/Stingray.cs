using System;
using System.Collections.Generic;

namespace StingrayFeeder.Data.Models
{
    public class Stingray
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Species { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Sex { get; set; }
        public string? Notes { get; set; }

        public ICollection<FeedEvent> FeedEvents { get; set; } = new List<FeedEvent>();
    }
}