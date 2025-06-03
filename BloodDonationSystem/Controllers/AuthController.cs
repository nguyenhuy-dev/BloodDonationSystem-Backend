using Application.DTO.LoginDTO;
using Application.Service.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController (IAuthService _authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
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
    }
}
