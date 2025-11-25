using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StingrayFeeder.Data.Models;
using StingrayFeeder.Services;
using System.Threading.Tasks;

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

            await _feedingService.RecordFeedEventAsync(feedEvent);

            // feedEvent.Id is populated by EF after SaveChanges
            return Ok(new { id = feedEvent.Id });
        }
    }
}
