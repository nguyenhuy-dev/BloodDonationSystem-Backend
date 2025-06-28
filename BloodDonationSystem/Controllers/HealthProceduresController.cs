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
        [HttpPost("api/blood-registrations/{id}/health-procedures")]
        public async Task<IActionResult> RecordHealthProcedure(int id, [FromBody] HealthProcedureRequest request)
        {
            var healthProcedure = await _service.RecordHealthProcedureAsync(id, request);

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
        [HttpGet("api/events/{id}/health-procedures")]
        public async Task<IActionResult> GetHealthProceduresByPaged(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedHealthProcedure = await _service.GetHealthProceduresByPagedAsync(id, pageNumber, pageSize);
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
            var healthProcedure = await _service.CancelHealthProcessAsync(bloodRegisId);

            if (healthProcedure == null)
                return BadRequest(new ApiResponse<HealthProcedure>
                {
                    IsSuccess = false,
                    Message = "Health procedure not found or cannot be cancelled."
                });

            return Ok(new ApiResponse<HealthProcedure>
            {
                IsSuccess = true,
                Message = "Health procedure cancelled successfully."
            });
        }
    }
}
