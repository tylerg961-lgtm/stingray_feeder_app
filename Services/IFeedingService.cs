using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StingrayFeeder.Data.Models;

namespace StingrayFeeder.Services
{
    public interface IFeedingService
    {
        Task<IEnumerable<FeedEvent>> GetRecentFeedEventsAsync(int max = 10, CancellationToken cancellationToken = default);
        Task RecordFeedEventAsync(FeedEvent feedEvent, CancellationToken cancellationToken = default);
    }
}