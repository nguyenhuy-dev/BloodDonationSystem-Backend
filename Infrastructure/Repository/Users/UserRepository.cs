using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Users
{
    public class UserRepository(BloodDonationSystemContext _context) : IUserRepository
    {
        public async Task<int> DeactiveUserAsync(Guid id)
        {
            return await _context.Users
                .Where(u => u.Id == id && u.Status == AccountStatus.Active)
                .ExecuteUpdateAsync(u => u.SetProperty(x => x.Status, AccountStatus.Inactive));
        }



        public async Task<int> CountAllAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<List<User>> GetAllUserAsync(int pageNumber, int pageSize)
        {
            return await _context.Users
                .Include(u => u.Role)
                .OrderByDescending(u => u.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<User?> UpdateUserProfileAsync(User updateUser)
        {
            _context.Users.Update(updateUser);
            await _context.SaveChangesAsync();
            return updateUser;
        }

        public async Task<User> AssignUserRole(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<int> BanUserAsync(Guid id)
        {
            return await _context.Users
                .Where(u => u.Id == id && u.Status == AccountStatus.Active)
                .ExecuteUpdateAsync(u => u.SetProperty(x => x.Status, AccountStatus.Banned));
        }
    }
}
