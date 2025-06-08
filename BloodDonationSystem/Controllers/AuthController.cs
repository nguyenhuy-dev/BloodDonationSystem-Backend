using Application.DTO;
using Application.DTO.GoogleDTO;
using Application.DTO.LoginDTO;
using Application.Service.Auth;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BloodDonationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService, IConfiguration _configuration,
                                IGoogleService _googleService) : ControllerBase
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

            return Ok(new GoogleAuthResponse
            {
                Gmail = email,
                FirstName = name
            });
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel tokenModel)
        {
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
                    ValidateToken(tokenModel.AccessToken, tokenValidateParam, out var validatedToken);

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
                var storageToken = await _authService.GetRefreshTokenAsync(tokenModel.RefreshToken);
                if (storageToken is null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "Refresh token not found"
                    });
                }

                //Create new token
                var token = _authService.GenerateToken(storageToken.User);
                return Ok(token);
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

        private DateTime ConvertUnixToDateTime(long utcExpireDate) //Chuyen unix thanh date time
        {
            var dateTimeInterval = new DateTime(1970,1,1,0,0,0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate);
            return dateTimeInterval;
        }

        //[HttpPost]
        //public IActionResult Test()
        //{
        //    string t = "";
        //    var list = _context.RefreshTokens.FirstOrDefault(r => r.UserId.ToString() == t);

        //    var token = _context.RefreshTokens.Add(new Domain.Entities.RefreshToken());
        //    _context.SaveChanges();

        //    return Ok(list); // This is just a placeholder for testing purposes.
        //}
    }
}