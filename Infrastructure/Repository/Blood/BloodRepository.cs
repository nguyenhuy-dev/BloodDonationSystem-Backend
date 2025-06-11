using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Blood
{
    public class BloodRepository (BloodDonationSystemContext _context) : IBloodRepository
    {
        public async Task<BloodType?> GetBloodTypeByNameAsync(string name)
        {
            return await _context.BloodTypes.FirstOrDefaultAsync(b => b.Type.ToUpper() == name.ToUpper());
        }
    }
}
