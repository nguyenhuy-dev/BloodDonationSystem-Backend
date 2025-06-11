using Application.DTO.EventsDTO;
using Domain.Entities;
using Infrastructure.Repository.Events;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Events
{
    public class EventService(IEventRepository _eventRepository, IHttpContextAccessor _contextAccessor) : IEventService
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
                EventType = eventRequest.EventType,
                IsExpired = false,
                CreateBy = creatorId,
                FacilityId = 1
            };
            await _eventRepository.AddEventAsync(events);
            return events;
        }
    }
}
