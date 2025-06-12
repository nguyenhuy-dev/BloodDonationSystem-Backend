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

        [Authorize(Roles = "Staff")]
        [HttpPost("add-urgent-event")]
        public async Task<IActionResult> AddUrgentEvent([FromBody] UrgentEventDTO urgentEvent)
        {
            if (urgentEvent == null)
            {
                return BadRequest("Urgent event request cannot be null.");
            }

            var createdEvent = await _eventService.AddUrgentEventAsync(urgentEvent);
            if (createdEvent == null)
            {
                return BadRequest("Failed created urgent event");
            }
            return Ok(new
            {
                Message = "Urgent event created successfully"
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var events = await _eventService.GetAllEventAsync(pageNumber, pageSize);
            if (events == null || !events.Items.Any())
            {
                return NotFound("No events found.");
            }
            return Ok(events);
        }
    }
}