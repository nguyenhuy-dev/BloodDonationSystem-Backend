using Application.DTO;
using Application.DTO.BloodRegistration;
using Application.DTO.BloodRegistrationDTO;
using Application.Service.BloodRegistrationServ;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [EnableCors("LocalPolicy")]
    [ApiController]
    public class BloodRegistrationsController(IBloodRegistrationService _service) : ControllerBase
    {
        [Authorize(Roles = "Member")]
        [HttpPost("api/events/{id}/blood-registrations")]
        public async Task<IActionResult> RegisterDonation(int id, [FromBody] BloodRegistrationRequest request)
        {
            var bloodRegistration = await _service.RegisterDonation(id, request);

            if (bloodRegistration == null)
                return BadRequest(new ApiResponse<BloodRegistrationRequest>()
                {
                    IsSuccess = true,
                    Message = "Register donation unsuccessfully"
                });

            return Ok(new ApiResponse<BloodRegistrationRequest>()
            {
                IsSuccess = true,
                Message = "Register donation successfully"
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("api/blood-registrations/{id}/reject")]
        public async Task<IActionResult> RejectBloodRegistration(int id)
        {
            var bloodRegistration = await _service.RejectBloodRegistration(id);
            if (bloodRegistration == null)
                return BadRequest(new ApiResponse<BloodRegistrationRequest>()
                {
                    IsSuccess = false,
                    Message = "Reject blood registration unsuccessfully"
                });

            return Ok(new ApiResponse<BloodRegistrationRequest>()
            {
                IsSuccess = true,
                Message = "Reject blood registration successfully"
            });
        }

        [Authorize(Roles = "Member")]
        [HttpPut("api/blood-registrations/{id}/cancel-own")]
        public async Task<IActionResult> CancelOwnRegistration(int id)
        {
            var bloodRegistration = await _service.CancelOwnRegistration(id);
            if (bloodRegistration == null)
                return BadRequest(new ApiResponse<BloodRegistrationRequest>()
                {
                    IsSuccess = false,
                    Message = "Blood registration not found or cancel unsuccessfully"
                });

            return Ok(new ApiResponse<BloodRegistrationRequest>()
            {
                IsSuccess = true,
                Message = "Cancel own registration successfully"
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("api/events/{id}/blood-registrations")]
        public async Task<IActionResult> GetBloodRegistrationsByPaged(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var bloodRegisResponse = await _service.GetBloodRegistrationsByPaged(id, pageNumber, pageSize);

            if (bloodRegisResponse == null)
                return NotFound(new ApiResponse<PaginatedResultBloodRegis>
                {
                    IsSuccess = false,
                    Message = "Not found event"
                });

            return Ok(new ApiResponse<PaginatedResultBloodRegis>()
            {
                IsSuccess = true,
                Message = "Get blood registrations successfully",
                Data = bloodRegisResponse
            });
        }
    }
}