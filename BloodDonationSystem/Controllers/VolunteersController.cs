using Application.DTO;
using Application.DTO.BloodHistoryDTO;
using Application.DTO.EventsDTO;
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
            var apiResponse = await _service.RegisterVolunteerDonationAsync(request);
            if (apiResponse?.IsSuccess == false)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
        }

        [Authorize(Roles = "Member")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVolunteerDonation(int id, [FromBody] UpdateVolunteerDonation request)
        {
            var volunteerRegis = await _service.UpdateVolunteerDonationAsync(id, request);

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
        [HttpGet("{facilityId}/paged")]
        public async Task<IActionResult> GetVolunteersByPaged(int facilityId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedVolunteer = await _service.GetVolunteersByPagedAsync(facilityId, pageNumber, pageSize);
            if (pagedVolunteer == null)
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = "Facility not found."
                });

            return Ok(new ApiResponse<PaginatedResult<VolunteersResponse>>
            {
                IsSuccess = true,
                Message = "Get paged volunteers successfully.",
                Data = pagedVolunteer
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("find-donors")]
        public async Task<IActionResult> AddDonationRegistrationWithVolunteer(UrgentEventVolunteer urgentEventVolunteer)
        {
            var apiResponse = await _service.AddDonationRegistrationWithVolunteersAsync(urgentEventVolunteer);

            if (apiResponse?.IsSuccess == false)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
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
