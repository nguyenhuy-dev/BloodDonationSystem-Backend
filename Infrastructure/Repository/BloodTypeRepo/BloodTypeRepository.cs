using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BloodTypeRepo
{
    public class BloodTypeRepository : GenericRepository<BloodType>, IBloodTypeRepository
    {
        public BloodTypeRepository(BloodDonationSystemContext context) : base(context)
        {
        }

        public async Task<BloodType?> GetBloodTypeByNameAsync(string typeName)
        {
            return await _dbSet.FirstOrDefaultAsync(b => b.Type.ToUpper() == typeName.ToUpper());
        }
    }
}
