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

            try
            {
                await _db.SaveChangesAsync(cancellationToken);

                // Domain-level success log (no HttpContext here)
                _logger.LogInformation("FeedEvent.Record.Succeeded CorrelationId={CorrelationId} FeedEventId={FeedEventId} StingrayId={StingrayId} Quantity={Quantity} EventTime={EventTime}",
                    feedEvent.CorrelationId, feedEvent.Id, feedEvent.StingrayId, feedEvent.Quantity, feedEvent.EventTime);
            }
            catch (Exception ex)
            {
                // Domain-level failure log with important fields
                _logger.LogError(ex, "FeedEvent.Record.Failed CorrelationId={CorrelationId} StingrayId={StingrayId} Quantity={Quantity}",
                    feedEvent.CorrelationId, feedEvent.StingrayId, feedEvent.Quantity);

                // Re-throw so calling layer (controller) can log request-level info and return an appropriate response
                throw;
            }
        }
    }
}