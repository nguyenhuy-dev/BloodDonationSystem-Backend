using Application.DTO;
using Application.DTO.EventsDTO;
using Application.Service.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
            return Ok(new
            {
                IsSuccess = true,
                Message = "Events retrieved successfully.",
                Data = events
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("api/events/{eventId}")]
        public async Task<IActionResult> UpdateEvent(int eventId, [FromBody] EventDTO updateEvent)
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

        [Authorize(Roles = "Staff")]
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

        [Authorize(Roles ="Staff")]
        [HttpGet("api/events/waiting-for-blood-procedure")]
        public async Task<IActionResult> GetWaitingList([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 10)
        {
            var events = await _eventService.GetPassedHealthProcedureAsync(pageNumber, pageSize);

            if (events == null)
            {
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = "Cannot found any event"
                });
            }
            return Ok(new
            {
                IsSuccess = true,
                Message = "Event retrieve successfully",
                Data = events
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("api/events/waiting-for-qualify-blood")]
        public async Task<IActionResult> GetWaitingListForQualifyBlood([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var events = await _eventService.GetEventListDoBloodProcedure(pageNumber, pageSize);

            if (events == null)
            {
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = "Cannot found any event"
                });
            }
            return Ok(new
            {
                IsSuccess = true,
                Message = "Event retrieve successfully",
                Data = events
            });
        }

        [HttpGet("api/events/search")]
        public async Task<IActionResult> SearchEventFromDayToDay([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 10,[FromQuery]DateOnly? startDay = null,[FromQuery]DateOnly? endDay = null)
        {
            var events = await _eventService.SearchEventByDayAsync(pageNumber, pageSize, startDay, endDay);
            if (events == null)
            {
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = "Cannot found any events"
                });
            }

            return Ok(new
            {
                IsSuccess = true,
                Message = "Event retrieved successfully",
                Data = events
            });
        }
    }
}