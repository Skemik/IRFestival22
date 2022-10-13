using System.Net;
using IRFestival.Api.Contexts;
using Microsoft.AspNetCore.Mvc;

using IRFestival.Api.Data;
using IRFestival.Api.Domain;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FestivalController : ControllerBase
    {
        private FestivalDbContext _ctx { get; set; }
        private TelemetryClient _telemetryClient { get; set; }

        public FestivalController(FestivalDbContext ctx, TelemetryClient telemetryClient)
        {
            _ctx = ctx;
            _telemetryClient = telemetryClient;
        }
        [HttpGet("LineUp")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Schedule))]
        public async Task<ActionResult> GetLineUp()
        {
            var schedule = await _ctx.Schedules.Include(s => s.Festival).Include(s => s.Items)
                .ThenInclude(s => s.Artist).Include(s => s.Items).ThenInclude(s => s.Stage).FirstOrDefaultAsync();
            return Ok(schedule);
        }

        [HttpGet("Artists")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Artist>))]
        public async Task<ActionResult> GetArtists(bool? withRatings)
        {
            //throw new ApplicationException("Error when fetching the artists !");
            //var artists = await _ctx.Artists.ToListAsync();
            //return Ok(artists);

            if (withRatings.HasValue && withRatings.Value)
            {
                _telemetryClient.TrackEvent($"List of artists with ratings");
            }
            else
            {
                _telemetryClient.TrackEvent($"List of artists without ratings");
            }
            var artists = await _ctx.Artists.ToListAsync();
            return Ok(artists);
        }

        [HttpGet("Stages")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Stage>))]
        public async Task<ActionResult> GetStages()
        {
            var stages = await _ctx.Stages.ToListAsync();
            return Ok(stages);
        }

        [HttpPost("Favorite")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ScheduleItem))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> SetAsFavorite(int id)
        {
            var schedule = await _ctx.Schedules.Select(x => x.Items.FirstOrDefault(x => x.Id == id)).FirstAsync();

            if (schedule != null)
            {
                schedule.IsFavorite = !schedule.IsFavorite;
                return Ok(schedule);
            }
            return NotFound();
        }

    }
}