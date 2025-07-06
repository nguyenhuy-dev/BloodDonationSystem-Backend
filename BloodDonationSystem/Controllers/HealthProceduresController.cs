using Application.DTO;
using Application.DTO.HealthProcedureDTO;
using Application.Service.HealthProcedureServ;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [EnableCors("LocalPolicy")]
    [ApiController]
    public class HealthProceduresController(IHealthProcedureService _service) : ControllerBase
    {
        [Authorize(Roles = "Staff")]
        [HttpPost("api/blood-registrations/{bloodRegisId}/health-procedures")]
        public async Task<IActionResult> RecordHealthProcedure(int bloodRegisId, [FromBody] HealthProcedureRequest request)
        {
            var healthProcedure = await _service.RecordHealthProcedureAsync(bloodRegisId, request);

            if (healthProcedure == null)
            {
                return BadRequest(new ApiResponse<HealthProcedureRequest>()
                {
                    IsSuccess = false,
                    Message = "Health procedure recorded unsuccessfully." 
                });
            }

            return Ok(new ApiResponse<HealthProcedureRequest>()
            {
                IsSuccess = true,
                Message = "Health procedure recorded successfully.",
                Data = request
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("api/events/{eventId}/health-procedures")]
        public async Task<IActionResult> GetHealthProceduresByPaged(int eventId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedHealthProcedure = await _service.GetHealthProceduresByPagedAsync(eventId, pageNumber, pageSize);
            if (pagedHealthProcedure == null)
                return NotFound(new ApiResponse<object>
                {
                    IsSuccess = false,
                    Message = "Not found event."
                });

            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Health procedures retrieved successfully.",
                Data = pagedHealthProcedure
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("api/blood-registrations/{bloodRegisId}/health-procedures/cancel")]
        public async Task<IActionResult> CancelHealthProcess(int bloodRegisId)
        {
            var apiResponse = await _service.CancelHealthProcessAsync(bloodRegisId);

            if (apiResponse?.IsSuccess == false)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("api/health-procedures/search")]
        public async Task<IActionResult> SearchHealthProcedures([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string keyword = null)
        {
            var healthProcedures = await _service.SearchHealthProceduresByPhoneOrNameAsync(pageNumber, pageSize, keyword);
            if (healthProcedures == null || !healthProcedures.Items.Any())
            {
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = "No health procedures found."
                });
            }
            return Ok(new
            {
                IsSuccess = true,
                Message = "Health procedures retrieved successfully.",
                Data = healthProcedures
            });
        }
    }
}
