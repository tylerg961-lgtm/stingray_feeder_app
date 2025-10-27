using System.Collections.Generic;

namespace StingrayFeeder.Data.Models
{
    public class Caretaker
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Notes { get; set; }

        // Navigation: historical feed events performed by this caretaker
        public ICollection<FeedEvent> FeedEvents { get; set; } = new List<FeedEvent>();
    }
}