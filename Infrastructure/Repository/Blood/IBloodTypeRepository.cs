using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blood
{
    public interface IBloodTypeRepository
    {
        Task<List<BloodType>> GetAllBloodTypeAsync();
        Task<BloodType?> GetBloodTypeByIdAsync(int? id);
        Task<BloodType?> GetBloodTypeByNameAsync(string name);
    }
}
