using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Users
{
    public class UserRepository(BloodDonationSystemContext _context) : IUserRepository
    {
        public async Task<int> DeactiveUserAsync(Guid id)
        {
            return await _context.Users
                .Where(u => u.Id == id && u.IsActived)
                .ExecuteUpdateAsync(u => u.SetProperty(x => x.IsActived, false));
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
    }
}
