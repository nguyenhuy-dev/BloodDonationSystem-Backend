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
    public class BloodTypeRepository(BloodDonationSystemContext _context) : IBloodTypeRepository
    {
        public async Task<BloodType?> GetBloodTypeByIdAsync(int? id)
        {
            return await _context.BloodTypes.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<BloodType?> GetBloodTypeByNameAsync(string name)
        {
            return await _context.BloodTypes.FirstOrDefaultAsync(b => b.Type.ToUpper() == name.ToUpper());
        }

        public async Task<List<BloodType>> GetAllBloodTypeAsync()
        {
            return await _context.BloodTypes.ToListAsync();
        }
    }
}
