using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StingrayFeeder.Data;
using StingrayFeeder.Data.Models;

namespace StingrayFeeder.Services
{
    public class FeedingService : IFeedingService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<FeedingService> _logger;

        public FeedingService(AppDbContext db, ILogger<FeedingService> logger)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<FeedEvent>> GetRecentFeedEventsAsync(int max = 10, CancellationToken cancellationToken = default)
        {
            max = Math.Max(1, max);

            return await _db.FeedEvents
                .AsNoTracking()
                .OrderByDescending(fe => fe.EventTime)
                .Take(max)
                .ToListAsync(cancellationToken);
        }

        public async Task RecordFeedEventAsync(FeedEvent feedEvent, CancellationToken cancellationToken = default)
        {
            if (feedEvent == null) throw new ArgumentNullException(nameof(feedEvent));

            if (feedEvent.CorrelationId == Guid.Empty)
            {
                feedEvent.CorrelationId = Guid.NewGuid();
            }

            if (feedEvent.EventTime == default)
            {
                feedEvent.EventTime = DateTime.UtcNow;
            }

            _db.FeedEvents.Add(feedEvent);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Recorded FeedEvent {FeedEventId} (Stingray {StingrayId})", feedEvent.Id, feedEvent.StingrayId);
        }
    }
}