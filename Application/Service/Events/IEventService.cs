using Application.DTO.EventsDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Events
{
    public interface IEventService
    {
        Task<Event?> AddEventAsync(NormalEventDTO eventRequest);
        Task<Event?> AddUrgentEventAsync(UrgentEventDTO eventRequest);
    }
}
