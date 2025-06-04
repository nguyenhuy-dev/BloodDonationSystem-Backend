using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blood
{
    public interface IBloodRepository
    {
        Task<BloodType?> GetBloodTypeByNameAsync(string name);
    }
}
