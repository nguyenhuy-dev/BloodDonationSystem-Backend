using Application.DTO.EventsDTO;
using Application.Service.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController(IEventService _eventService) : ControllerBase
    {
        [Authorize(Roles = "Staff")]
        [HttpPost("add-event")]
        public async Task<IActionResult> AddEvent([FromBody] NormalEventDTO eventRequest)
        {
            if (eventRequest == null)
            {
                return BadRequest("Event request cannot be null.");
            }
            var createdEvent = await _eventService.AddEventAsync(eventRequest);
            if (createdEvent == null)
            {
                return BadRequest("Failed to create event.");
            }
            return Ok(new 
            { 
                Message = "Event created successfully"
            });
        }
    }
}
