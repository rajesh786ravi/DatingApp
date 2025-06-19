using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchingController : ControllerBase
    {
        private readonly MatchService _matchService;

        public MatchingController(MatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpPost("{userId}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        private async Task<IActionResult> MatchUser(int userId)
        {
            try
            {
                await _matchService.HandleMatchAsync(userId);
                return Ok(new { message = $"Matching completed for user {userId}." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Matching failed: {ex.Message}");
            }
        }
    }
}
