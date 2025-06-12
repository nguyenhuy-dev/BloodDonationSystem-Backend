using Application.DTO.EventsDTO;
using Domain.Entities;
using Infrastructure.Helper;

namespace Application.Service.Events
{
    public interface IEventService
    {
        Task<Event?> AddEventAsync(NormalEventDTO eventRequest);
        Task<Event?> AddUrgentEventAsync(UrgentEventDTO eventRequest);
        Task<PaginatedResult<Event>> GetAllEventAsync(int pageNumber, int pageSize);
    }
}
