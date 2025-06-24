using Application.DTO.GoogleDTO;
using Application.DTO.LoginDTO;
using Application.DTO.Token;
using Application.DTO.UserDTO;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repository.Auth;
using Infrastructure.Repository.Blood;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Auth
{
    public class AuthService(IAuthRepository _authRepository, 
        IConfiguration _configuration, IHttpContextAccessor _httpContext,
        IBloodTypeRepository _bloodRepository) : IAuthService
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
            if (user.Status == AccountStatus.Inactive)
            {
                return new LoginResponse // Invalid login response
                {
                    IsSuccess = false,
                    Message = "This account not available"
                };
            } else if (user.Status == AccountStatus.Banned)
            {
                return new LoginResponse // Invalid login response
                {
                    IsSuccess = false,
                    Message = "This account is banned"
                };
            }
            var token = GenerateToken(user);
            SetRefreshTokenCookie(token.RefreshToken); // Set the refresh token in a secure cookie

            return new LoginResponse // Successful login response
            {
                IsSuccess = true,
                Message = "Login successful.",
                Token = token.AccessToken,
                Phone = user.Phone,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }

        public async Task<User?> RegisterAsync(UserDTO userDTO)
        {
            if (await _authRepository.UserExistsByPhoneAsync(userDTO.Phone))
            {
                return null; // User already exists
            }

            //var bloodType = await _bloodRepository.GetBloodTypeByNameAsync(userDTO.BloodTypeId);

            var user = new User
            {
                FirstName = userDTO.FirstName,
                LastName = userDTO.LastName,
                Phone = userDTO.Phone,
                BloodTypeId = userDTO.BloodTypeId,
                Longitude = userDTO.Longitude,
                Latitude = userDTO.Latitude,
                Dob = userDTO.Dob,
                Gmail = userDTO.Gmail,
                Gender = userDTO.Gender,
                Status = AccountStatus.Active,
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
            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),  // Thêm UserId vào trong Token
                    new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                    new Claim(ClaimTypes.Role, user.Role.RoleName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            // Only add phone claim if available
            if (!string.IsNullOrEmpty(user.Phone))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Phone));
            }
            // Only add phone claim if available
            if (!string.IsNullOrEmpty(user.Gmail))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Gmail));
            }

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), //Config token tra ra cai gi
                Expires = DateTime.UtcNow.AddMinutes(60), //Token expires in 1 min to test
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
                JwtId = token.Id,
                UserId = user.Id,
                IsUsed = false,
                IsRevoked = false,
                ExpiredAt = DateTime.UtcNow.AddDays(7) // Set expiration for the refresh token
            };
            _authRepository.SaveRefreshTokenAsync(refreshTokenEntity);

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken // Return the generated refresh token
            };
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            // Set the refresh token in a secure cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Prevents JavaScript access to the cookie
                Secure = true, // Use HTTPS in production
                Expires = DateTime.UtcNow.AddDays(7), // Set expiration for the cookie
                SameSite = SameSiteMode.Strict // Prevent CSRF attacks
            };
            _httpContext.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
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

        public Task<User> GetUserByEmailAsync(string email)
        {
            var user = _authRepository.GetUserByEmailAsync(email);
            return user;
        }

        public Task<User> RegisterWithGoogleAsync(User user)
        {
            var account = _authRepository.RegisterAsync(user);
            return account;
        }

        public async Task<User> UpdateGoogleLoginAsync(UpdateGoogleLogin request)
        {
            var existUser = await _authRepository.GetUserByEmailAsync(request.Gmail);
            if (existUser is null)
            {
                return null;
            }
            var hashPassword = new PasswordHasher<User>();
            existUser.HashPass = hashPassword.HashPassword(existUser, request.Password);

            existUser.FirstName = request.FirstName;
            existUser.LastName = request.LastName;
            existUser.Phone = request.Phone;
            existUser.BloodTypeId = request.BloodTypeId;
            existUser.Dob = request.Dob;
            existUser.Gender = request.Gender;
            existUser.Status = AccountStatus.Active;

            await _authRepository.UpdateGoogleLogin(existUser);
            return existUser;
        }

        public async Task<RefreshToken> UpdateRefreshTokenAsync(RefreshToken refreshToken)
        {
            var token = await _authRepository.GetRefreshTokenAsync(refreshToken.Token);
            if (token == null)
            {
                return null; // Refresh token not found
            }

            await _authRepository.UpdateRefreshTokenAsync(refreshToken);
            return refreshToken; // Return the updated refresh token
        }
    }
}