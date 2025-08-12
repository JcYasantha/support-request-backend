using Microsoft.AspNetCore.Mvc;
using SupportRequest.Core.Interfaces.Service;
using SupportRequest.Core.Models;

namespace SupportRequest.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupportRequestController(ISupportRequestSessionService supportRequestSessionService) : ControllerBase
    {
        [HttpPost("Create")]
        public IActionResult CreateSupportRequest()
        {
            var supportRequestSession = new SupportRequestSession();
            var accepted = supportRequestSessionService.AddToQueue(supportRequestSession);

            if (!accepted)
                return StatusCode(429, new { Message = "Chat refused, Queue is full. Please try again later."});

            return Ok(new { Message = "Chat queued successfully", supportRequestSession });
        }

        [HttpGet("poll/{id}")]
        public IActionResult Poll(Guid id)
        {
            var supportRequestSession = supportRequestSessionService.UpdateLastPoll(id);
            if (supportRequestSession == null) return NotFound();
            return Ok(supportRequestSession);
        }
    }
}
