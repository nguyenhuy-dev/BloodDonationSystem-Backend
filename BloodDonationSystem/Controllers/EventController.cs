using Application.DTO;
using Application.DTO.EventsDTO;
using Application.Service.Events;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [EnableCors("LocalPolicy")]
    [ApiController]
    public class EventController(IEventService _eventService) : ControllerBase
    {
        [Authorize(Roles = "Staff")]
        [HttpPost("api/events")]
        public async Task<IActionResult> AddEvent([FromBody] NormalEventDTO eventRequest)
        {
            if (eventRequest == null)
            {
                return BadRequest(new ApiResponse<NormalEventDTO>
                {
                    IsSuccess = false,
                    Message = "Event request cannot be null"
                });
            }
            var createdEvent = await _eventService.AddEventAsync(eventRequest);
            if (createdEvent == null)
            {
                return BadRequest(new ApiResponse<NormalEventDTO>
                {
                    IsSuccess = false,
                    Message = "Failed to create event."
                });
            }
            return Ok(new ApiResponse<NormalEventDTO>
            {
                IsSuccess = true,
                Message = "Event created successfully",
                Data = eventRequest
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("api/events/urgent")]
        public async Task<IActionResult> AddUrgentEvent([FromBody] UrgentEventDTO urgentEvent)
        {
            if (urgentEvent == null)
            {
                return BadRequest(new ApiResponse<UrgentEventDTO>
                {
                    IsSuccess = false,
                    Message = "Urgent request cannot be null"
                });
            }

            var createdEvent = await _eventService.AddUrgentEventAsync(urgentEvent);
            if (createdEvent == null)
            {
                return BadRequest(new ApiResponse<UrgentEventDTO>
                {
                    IsSuccess = false,
                    Message = "Failed to create urgent event."
                });
            }
            return Ok(new ApiResponse<UrgentEventDTO>
            {
                IsSuccess = true,
                Message = "Urgent event created successfully"
            });
        }

        [HttpGet("api/events")]
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
        [HttpPut("api/events/{eventId}")]
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
                EventDTO = eventItem
            });
        }

        [Authorize (Roles = "Staff")]
        [HttpPut("api/events/{eventId}/deactive")]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            if (eventId <= 0)
            {
                return BadRequest("Invalid event ID.");
            }
            await _eventService.DeleteEventAsync(eventId);
            return Ok(new
            {
                Success = true,
                Message = "Event deleted successfully"
            });
        }
    }
}