using Application.DTO;
using Application.DTO.GoogleDTO;
using Application.DTO.LoginDTO;
using Application.DTO.Token;
using Application.Service.Auth;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Text;

namespace BloodDonationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService, IConfiguration _configuration,
                                IGoogleService _googleService, IHttpContextAccessor _httpContextAccessor) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request.Phone, request.Password);
            if (!response.IsSuccess)
            {
                return Unauthorized(response.Message);
            }
            return Ok(new
            {
                response.IsSuccess,
                response.Message,
                response.Token,
                response.Phone,
                response.FirstName,
                response.LastName
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO request)
        {
            var user = await _authService.RegisterAsync(request);
            if (user == null)
            {
                return BadRequest("User already exists or registration failed.");
            }
            return Ok("Register sucessfully");
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequest request)
        {
            var payload = await _googleService.ValidateGoogleTokenAsync(request.Credential);
            if (payload == null)
            {
                return BadRequest("Invalid Google token.");
            }

            var email = payload.Email;
            var name = payload.Name;

            var user = await _authService.GetUserByEmailAsync(email);
            if (user == null)
            {
                var nameParts = name?.Split(' ', 2);
                var firstName = nameParts?[0] ?? "";
                var lastName = nameParts?.Length > 1 ? nameParts[1] : "";

                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Gmail = email,
                    IsActived = false, //Cannot use yet
                    RoleId = 3 // Assuming 3 is the default role ID for a user
                };
                await _authService.RegisterWithGoogleAsync(user);
            }

            return Ok(new
            {
                Gmail = email,
                Name = name
            });
        }

        [HttpPut("google-update-Login")]
        public async Task<IActionResult> UpdateGoogleLogin([FromBody] UpdateGoogleLogin request)
        {
            var user = await _authService.UpdateGoogleLoginAsync(request);
            
            if (user == null)
            {
                return BadRequest("Update failed.");
            }

            var token = _authService.GenerateToken(user);
            return Ok(new
            {
                Message = "Update successfully",
                Token = token
            });
        }


        [HttpPost("renew-token")]
        public async Task<IActionResult> RenewToken()
        {
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Refresh token not found"
                });
            }

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKey = _configuration["AppSettings:SecretKey"]; // Replace with your actual secret key
            var secretKeyByte = Encoding.UTF8.GetBytes(secretKey); // Replace with your actual secret key
            var tokenValidateParam = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyByte),
                ClockSkew = TimeSpan.Zero, // Disable clock skew for immediate expiration

                ValidateLifetime = false // Khong kiem tra token co het han khong
            };

            try
            {
                //Check1: Access token valid format
                var tokenInvalidate = jwtTokenHandler.
                    ValidateToken(_httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", ""),
                    tokenValidateParam, out var validatedToken);

                //Check2: Check alg
                if(validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var alg = jwtSecurityToken.Header.Alg.Equals
                        (SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase);
                    if(!alg)
                    {
                        return Ok(new ApiResponse
                        {
                            Success = false,
                            Message = "Invalid token"
                        });
                    }
                }

                //Check3: Check if token is expired
                var utcExpireDate = long.Parse(tokenInvalidate.Claims
                    .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Token is not expired yet"
                    });
                }

                //Check4: Check refreshToken in db
                var storageToken = await _authService.GetRefreshTokenAsync(refreshToken);
                if (storageToken is null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token not found"
                    });
                }

                //Check5: Check if refresh token is used/revoked?
                if(storageToken.IsUsed)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token is used"
                    });
                }

                if(storageToken.IsRevoked)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token is revoked"
                    });
                }

                //Check6: Check if id of access token == jwt id refresh token match
                var jti = tokenInvalidate.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storageToken.JwtId != jti)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Token is not match"
                    });
                }

                storageToken.IsUsed = true; // Mark the refresh token as used
                storageToken.IsRevoked = true; // Mark the refresh token as revoked
                storageToken.ExpiredAt = DateTime.UtcNow; // Set the expiration date to now
                await _authService.UpdateRefreshTokenAsync(storageToken); // Update the refresh token in the database

                //Create new token
                var newToken = _authService.GenerateToken(storageToken.User);
                SetRefreshTokenCookie(newToken.RefreshToken);
                return Ok(newToken.AccessToken);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Something went wrong"
                });
            }
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Prevents JavaScript access to the cookie
                Secure = true, // Use HTTPS in production
                Expires = DateTime.UtcNow.AddDays(7), // Set expiration for the cookie
                SameSite = SameSiteMode.Strict // Prevent CSRF attacks
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private DateTime ConvertUnixToDateTime(long utcExpireDate) //Chuyen unix thanh date time
        {
            var dateTimeInterval = new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate);
            return dateTimeInterval;
        }
    }
}