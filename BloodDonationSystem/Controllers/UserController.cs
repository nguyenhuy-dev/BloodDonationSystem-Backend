using Application.DTO.UserDTO;
using Application.Service.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [EnableCors("LocalPolicy")]
    [ApiController]
    public class UserController(IUserService _userService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPut("api/users/{userId}/ban")]
        public async Task<IActionResult> BanUser(Guid userId)
        {
            var result = await _userService.BanUserAsync(userId);
            if (!result)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Message = "User may not exist or is already banned."
                });
            }
            return Ok(new
            {
                IsSuccess = true,
                Message = "User banned successfully."
            });
        }

        [Authorize]
        [HttpPut("api/users/deactive")]
        public async Task<IActionResult> DeactiveUser(Guid userId)
        {
            var result = await _userService.DeactiveUserAsync(userId);
            if (!result)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Message = "User may not exist or is already deactivated."
                });
            }
            return Ok(new
            {
                IsSuccess = true,
                Message = "User deactivated successfully."
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("api/users/{userId}/assign-role")]
        public async Task<IActionResult> AssignUserRole(Guid userId, int roleId)
        {
            var user = await _userService.AssignUserRole(userId, roleId);
            if (user == null)
            {
                return BadRequest(new
                {
                    Message = "Cannot assign role to this user"
                });
            }
            return Ok(new
            {
                Message = "Assign role successfully"
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("api/users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _userService.GetAllUserAsync(pageNumber, pageSize);
            if (users == null || !users.Items.Any())
            {
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = "No users found."
                });
            }
            return Ok(new
            {
                IsSuccess = true,
                Message = "Users retrieved successfully.",
                Data = users
            });
        }

        [Authorize]
        [HttpGet("api/users/profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return Unauthorized(new
                {
                    IsSuccess = false,
                    Message = "User not authenticated."
                });
            }
            var profile = await _userService.GetUserByIdAsync(Guid.Parse(userId));
            if (profile == null)
            {
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = "User profile not found."
                });
            }
            return Ok(new
            {
                IsSuccess = true,
                Message = "User profile retrieved successfully.",
                Data = profile
            });
        }

        [Authorize]
        [HttpPut("/api/users/profile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserDTO profileDto)
        {
            if (profileDto == null)
            {
                return BadRequest("Invalid profile data.");
            }
            var userId = User.FindFirst("UserId")?.Value;
            if (userId == null)
            {
                return Unauthorized(new
                {
                    IsSuccess = false,
                    Message = "User not authenticated."
                });
            }
            var updatedProfile = await _userService.UpdateUserProfileAsync(Guid.Parse(userId), profileDto);
            if (updatedProfile == null)
            {
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = "User profile not found or could not be updated."
                });
            }
            return Ok(new
            {
                IsSuccess = true,
                Message = "User profile updated successfully.",
                Data = updatedProfile
            });
        }
    }
}



//[Authorize(Roles = "Admin")]
//[HttpPost("add-staff")]
//public async Task<IActionResult> AddStaffAsync([FromBody] UserDTO request)
//{
//    if (request == null)
//    {
//        return BadRequest("Invalid user data.");
//    }
//    var user = await _userService.AddStaffAsync(request);
//    if (user == null)
//    {
//        return BadRequest("User already exists with the provided phone number.");
//    }
//    return Ok(new
//    {
//        Message = "Staff added successfully.",
//        User = user
//    });
//}