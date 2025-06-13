using Application.DTO.EventsDTO;
using Application.Service.Events;
using Domain.Entities;
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
        public async Task<IActionResult> AddEvent([FromBody] EventDTO eventRequest)
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
        public async Task<IActionResult> AddUrgentEvent([FromBody] EventDTO urgentEvent)
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

        [Authorize(Roles = "Staff")]
        [HttpPut("update/{eventId}")]
        public async Task<IActionResult> UpdateEvent(int eventId, [FromBody]EventDTO updateEvent)
        {
            var eventItem = await _eventService.UpdateEventAsync(eventId, updateEvent);
            if (eventItem == null)
            {
                return BadRequest("Cannot update event");
            }
            return Ok(new
            {
                Message = "Event updated successfully",
                Event = eventItem
            });
        }

        [Authorize (Roles = "Staff")]
        [HttpPut("deactive/{eventId}")]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            if (eventId <= 0)
            {
                return BadRequest("Invalid event ID.");
            }
            await _eventService.DeleteEventAsync(eventId);
            return Ok(new
            {
                Message = "Event deleted successfully"
            });
        }
    }
}