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
        public async Task<User?> LoginAsync(string phone, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == phone);
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
    }
}
