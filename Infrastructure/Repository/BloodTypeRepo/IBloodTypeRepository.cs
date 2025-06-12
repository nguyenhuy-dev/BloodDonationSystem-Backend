using Domain.Entities;
using Infrastructure.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BloodTypeRepo
{
    public interface IBloodTypeRepository : IGenericRepository<BloodType>
    {
        Task<BloodType?> GetBloodTypeByNameAsync(string typeName);
    }
}
