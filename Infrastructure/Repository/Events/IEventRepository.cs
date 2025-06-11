using Domain.Entities;
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
    }
}
