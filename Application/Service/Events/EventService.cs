using Application.DTO.EventsDTO;
using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Blood;
using Infrastructure.Repository.Events;
using Microsoft.AspNetCore.Http;

namespace Application.Service.Events
{
    public class EventService(IEventRepository _eventRepository, 
                            IHttpContextAccessor _contextAccessor, 
                            IBloodRepository _bloodRepository) : IEventService
    {
        public async Task<Event?> AddEventAsync(NormalEventDTO eventRequest)
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            var events = new Event
            {
                Title = eventRequest.Title,
                MaxOfDonor = eventRequest.MaxOfDonor,
                EstimatedVolume = eventRequest.EstimateVolume,
                CreateAt = DateTime.UtcNow,
                EventTime = eventRequest.EventTime,
                IsUrgent = eventRequest.IsUrgent,
                IsExpired = false,
                CreateBy = creatorId,
                FacilityId = 1
            };
            await _eventRepository.AddEventAsync(events);
            return events;
        }

        public async Task<Event?> AddUrgentEventAsync(UrgentEventDTO eventRequest)
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            var bloodType = await _bloodRepository.GetBloodTypeByNameAsync(eventRequest.BloodType);

            var events = new Event
            {
                Title = eventRequest.Title,
                MaxOfDonor = eventRequest.MaxOfDonor,
                EstimatedVolume = eventRequest.EstimateVolume,
                BloodTypeId = bloodType.Id,
                BloodComponent = eventRequest.BloodComponent,
                CreateAt = DateTime.UtcNow,
                EventTime = eventRequest.EventTime,
                IsUrgent = eventRequest.IsUrgent,
                IsExpired = false,
                CreateBy = creatorId,
                FacilityId = 1
            };
            await _eventRepository.AddEventAsync(events);
            return events;
        }

        public async Task<PaginatedResult<Event>> GetAllEventAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page number and page size must be greater than zero.");
            }
            return await _eventRepository.GetAllEventAsync(pageNumber, pageSize);
        }
    }
}
