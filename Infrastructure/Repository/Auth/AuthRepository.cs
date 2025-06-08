using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Auth
{
    public class AuthRepository (BloodDonationSystemContext _context) : IAuthRepository
    {
        public async Task<bool> UserExistsByPhoneAsync(string phone)
        {
            return await _context.Users.AnyAsync(u => u.Phone == phone);
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Gmail == email);
        }

        public async Task<User?> GetUserByPhoneAsync(string phone)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
        }

        public async Task<User?> LoginAsync(string phone, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role) // Include Role if needed
                .FirstOrDefaultAsync(u => u.Phone == phone);
            if (user == null) 
            {
                //Console.WriteLine("User error"); //Fix bug
                return null; // User not found
            }

            var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.HashPass, password);
            if (result == PasswordVerificationResult.Failed)
            {
                //Console.WriteLine("Password error"); //Fix bug
                return null; // Password failed
            }
            return user; // Successful login, return user
        }

        public async Task<User?> RegisterAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user; // Return the registered user
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .Include(u => u.User)
                .ThenInclude(u => u.Role) // Include Role if needed
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            return token;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Gmail == email);
        }
    }
}
