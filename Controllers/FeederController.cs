using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StingrayFeeder.Data.Models;
using StingrayFeeder.Services;

namespace stingray_feeder_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedingController : ControllerBase
    {
        private readonly IFeedingService _feedingService;
        private readonly ILogger<FeedingController> _logger;

        public FeedingController(IFeedingService feedingService, ILogger<FeedingController> logger)
        {
            _feedingService = feedingService;
            _logger = logger;
        }

        // Thin controller: forwards query to service and returns result
        [HttpGet("recent")]
        public async Task<IActionResult> GetRecent([FromQuery] int max = 10)
        {
            var items = await _feedingService.GetRecentFeedEventsAsync(max);
            return Ok(items);
        }

        // Thin controller: forward DTO/entity to service; service performs defaults/validation/persistence
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FeedEvent feedEvent)
        {
            if (feedEvent == null) return BadRequest();

            // Ensure there is a correlation id we can log (service will also set this if missing)
            if (feedEvent.CorrelationId == Guid.Empty)
            {
                feedEvent.CorrelationId = Guid.NewGuid();
            }

            var requestId = HttpContext.TraceIdentifier;
            var correlationId = feedEvent.CorrelationId;
            _logger.LogInformation("FeedEvent.Record.Started RequestId={RequestId} CorrelationId={CorrelationId} StingrayId={StingrayId} FeedBatchId={FeedBatchId} Quantity={Quantity}",
                requestId, correlationId, feedEvent.StingrayId, feedEvent.FeedBatchId, feedEvent.Quantity);

            try
            {
                await _feedingService.RecordFeedEventAsync(feedEvent);

                // feedEvent.Id is populated by EF after SaveChanges
                _logger.LogInformation("FeedEvent.Record.Succeeded RequestId={RequestId} CorrelationId={CorrelationId} FeedEventId={FeedEventId} StingrayId={StingrayId} Quantity={Quantity}",
                    requestId, correlationId, feedEvent.Id, feedEvent.StingrayId, feedEvent.Quantity);

                return Ok(new { id = feedEvent.Id });
            }
            catch (Exception ex)
            {
                // Error must be actionable and include the correlation and request ids
                _logger.LogError(ex, "FeedEvent.Record.Failed RequestId={RequestId} CorrelationId={CorrelationId} StingrayId={StingrayId} FeedBatchId={FeedBatchId} Quantity={Quantity}",
                    requestId, correlationId, feedEvent.StingrayId, feedEvent.FeedBatchId, feedEvent.Quantity);

                // Keep client-facing message generic
                return StatusCode(500, "An error occurred while recording the feed event.");
            }
        }
    }
}
