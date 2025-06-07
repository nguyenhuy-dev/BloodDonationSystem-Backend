using Application.DTO;
using Application.DTO.LoginDTO;
using Domain.Entities;
using Infrastructure.Repository.Auth;
using Infrastructure.Repository.Blood;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Auth
{
    public class AuthService(IAuthRepository _authRepository, 
        IBloodRepository _bloodRepository, IConfiguration _configuration) : IAuthService
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
                Token = GenerateToken(user),
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
                RoleId = 3, // Assuming 3 is the default role ID for a user
            };

            var hashedPassword = new PasswordHasher<User>();
            user.HashPass = hashedPassword.HashPassword(user, userDTO.Password);

            await _authRepository.RegisterAsync(user);
            return user; // Return the registered user
        }

        public TokenModel GenerateToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyByte = Encoding.UTF8.GetBytes(_configuration["AppSettings:SecretKey"]); //Lay secret key
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                    new Claim(ClaimTypes.MobilePhone, user.Phone),
                    new Claim(ClaimTypes.Email, user.Gmail),
                    new Claim(ClaimTypes.Role, user.Role.RoleName),
                }), //Config token tra ra cai gi
                Expires = DateTime.UtcNow.AddMinutes(1), //Token expires in 1 min to test
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKeyByte), //Secret key
                    SecurityAlgorithms.HmacSha256)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken(); // Generate a new refresh token

            // Save the refresh token to the database
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiredAt = DateTime.UtcNow.AddDays(7) // Set expiration for the refresh token
            };
            _authRepository.SaveRefreshTokenAsync(refreshTokenEntity);

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
            }
            return Convert.ToBase64String(random); // Generate a random refresh token
        }

        public async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            var token = await _authRepository.GetRefreshTokenAsync(refreshToken);
            return token;
        }
    }
}
