using Application.DTO;
using Application.DTO.LoginDTO;
using Application.Service.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService) : ControllerBase
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
                //  response.Token,
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
    }
}