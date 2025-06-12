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
        Task<PaginatedResult<Event>> GetAllEventAsync(int pageNumber, int pageSize);
    }
}