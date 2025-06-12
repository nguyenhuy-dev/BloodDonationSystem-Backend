using Application.Service.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController (IUserService _userService) : ControllerBase
    {
        [HttpPut("deactive/{userId}")]
        public async Task<IActionResult> DeactiveUser(Guid userId)
        {
            var result = await _userService.DeactiveUserAsync(userId);
            if (!result)
            {
                return BadRequest("User may not exist or is already deactivated.");
            }
            return Ok("Deactive successfully");
        }
    }
}
