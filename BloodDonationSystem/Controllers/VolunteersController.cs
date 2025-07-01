using Application.DTO;
using Application.DTO.BloodHistoryDTO;
using Application.DTO.VolunteerDTO;
using Application.Service.BloodHistoryServ;
using Application.Service.VolunteerServ;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [EnableCors("LocalPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class VolunteersController(IVolunteerService _service, IBloodHistoryService _bloodHistory) : ControllerBase
    {
        [Authorize(Roles = "Member")]
        [HttpPost]
        public async Task<IActionResult> RegisterVolunteerDonation([FromBody] RegisterVolunteerDonation request)
        {
            var volunteerRegistration = await _service.RegisterVolunteerDonation(request);
            if (volunteerRegistration == null)
                return BadRequest(new ApiResponse<RegisterVolunteerDonation>
                {
                    IsSuccess = false,
                    Message = "Register volunteer donation unsuccessfully"
                });

            return Ok(new ApiResponse<RegisterVolunteerDonation>()
            {
                IsSuccess = true,
                Message = "Register volunteer donation successfully"
            });
        }

        [Authorize(Roles = "Member")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVolunteerDonation(int id, [FromBody] UpdateVolunteerDonation request)
        {
            var volunteerRegis = await _service.UpdateVolunteerDonation(id, request);

            if (volunteerRegis == null)
                return BadRequest(new ApiResponse<UpdateVolunteerDonation>
                {
                    IsSuccess = false,
                    Message = "Volunteer registration not found or expired"
                });

            return Ok(new ApiResponse<UpdateVolunteerDonation>
            {
                IsSuccess = true,
                Message = "Update volunteer registration successfully"
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("paged")]
        public async Task<IActionResult> GetVolunteersByPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedVolunteer = await _service.GetVolunteersByPaged(pageNumber, pageSize);

            return Ok(new ApiResponse<PaginatedResult<VolunteersResponse>>
            {
                IsSuccess = true,
                Message = "Get paged volunteers successfully",
                Data = pagedVolunteer
            });
        }

        [Authorize]
        [HttpPut("{id}/date")]
        public async Task<IActionResult> UpdateAvailableDateForVolunteer(int id, [FromBody]UpdateAvailableDateDTO request)
        {
            var update = await _bloodHistory.UpdateAvailableDateVolunteerAsync(id, request);
            if (!update)
            {
                return BadRequest(new
                {
                    IsSuccess = false,
                    Message = "Failed to update"
                });
            }
            return Ok(new
            {
                IsSuccess = true,
                Message = "Update successfully"
            });
        }
    }
}
