using Domain.Entities;
using Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Events
{
    public interface IEventRepository
    {
        Task<Event?> AddEventAsync(Event newEvent);

        Task<int> CountAllActiveEventAsync();
        Task<int> CountAllEventAsync();
        Task<int> CountEventFromDayToDay(DateOnly? startDay, DateOnly? endDay);

        Task<List<Event>> GetAllActiveEventAsync(int pageNumber, int pageSize);
        Task<List<Event>> GetAllEventAsync(int pageNumber, int pageSize);

        Task<List<Event>> GetAllUrgentEventAsync(int pageNumber, int pageSize);
        Task<List<Event>> GetAllNormalEventAsync(int pageNumber, int pageSize);
        Task<Event?> GetEventByIdAsync(int eventId);

        Task<Event> UpdateEventAsync(Event updateEvent);

        Task<List<Event>> GetPassedHealthProcedureAsync(int pageNumber, int pageSize);
        Task<int> CountEventPassedHealthProcedureAsync();
        Task<List<Event>> GetEventListDoBloodProcedure(int pageNumber, int pageSize);
        Task<int> CountEventListDoBloodProcedure();

        Task<List<Event>> SearchEventByDayAsync(int pageNumber, int pageSize, DateOnly? startDay, DateOnly? endDay);

        Task<int> EventExpiredAsync();
    }
}
