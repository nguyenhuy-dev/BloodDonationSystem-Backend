using Application.DTO;
using Application.DTO.LoginDTO;
using Domain.Entities;
using Infrastructure.Repository.Auth;
using Infrastructure.Repository.Blood;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Auth
{
    public class AuthService(IAuthRepository _authRepository, IBloodRepository _bloodRepository) : IAuthService
    {
        public async Task<LoginResponse> LoginAsync(string phone, string password)
        {
            var user = await _authRepository.LoginAsync(phone, password);
            if (user == null)
            {
                return new LoginResponse // Invalid login response
                {
                    IsSuccess = false,
                    Message = "Invalid phone or password."
                };
            }
            return new LoginResponse // Successful login response
            {
                IsSuccess = true,
                Message = "Login successful.",
                //Token = null, // Cai nay de sau
                Phone = user.Phone,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<User?> RegisterAsync(UserDTO userDTO)
        {
            if (await _authRepository.UserExistsAsync(userDTO.Phone))
            {
                return null; // User already exists
            }

            var bloodType = await _bloodRepository.GetBloodTypeByNameAsync(userDTO.BloodType);

            var user = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Phone = userDTO.Phone,
                BloodTypeId = bloodType.Id,
                Dob = userDTO.Dob,
                Gmail = userDTO.Gmail,
                Gender = userDTO.Gender,
                Address = userDTO.Address,
                RoleId = 3, // Assuming 3 is the default role ID for a user
            };

            var hashedPassword = new PasswordHasher<User>();
            user.HashPass = hashedPassword.HashPassword(user, userDTO.Password);

            await _authRepository.RegisterAsync(user);
            return user; // Return the registered user
        }
    }
}
