using Application.DTO;
using Application.DTO.BloodProcedureDTO;
using Application.Service.BloodProcedureServ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [EnableCors("LocalPolicy")]
    [ApiController]
    public class BloodProceduresController(IBloodProcedureService _service) : ControllerBase
    {
        [Authorize(Roles = "Staff")]
        [HttpPost("api/blood-registrations/{bloodRegisId}/blood-procedures/collect")]
        public async Task<IActionResult> RecordBloodCollection(int bloodRegisId, [FromBody] BloodCollectionRequest request)
        {
            var bloodCollection = await _service.RecordBloodCollectionAsync(bloodRegisId, request);

            if (bloodCollection == null)
            {
                return BadRequest(new ApiResponse<BloodCollectionRequest>
                {
                    IsSuccess = false,
                    Message = "Failed to record blood collection."
                });
            }

            return Ok(new ApiResponse<BloodCollectionRequest>
            {
                IsSuccess = true,
                Message = "Blood collection recorded successfully.",
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("api/blood-registrations/{bloodRegisId}/blood-procedures/qualify")]
        public async Task<IActionResult> RecordBloodQualification(int bloodRegisId, [FromBody] RecordBloodQualification request)
        {
            var bloodProcedure = await _service.UpdateBloodQualificationAsync(bloodRegisId, request);

            if (bloodProcedure == null)
                return BadRequest(new ApiResponse<RecordBloodQualification>()
                {
                    IsSuccess = false,
                    Message = "Failed to record blood qualification."
                });

            return Ok(new ApiResponse<RecordBloodQualification>()
            {
                IsSuccess = true,
                Message = "Blood qualification recorded successfully."
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("api/events/{eventId}/blood-procedures")]
        public async Task<IActionResult> GetBloodCollectionsByPaged(int eventId, int pageNumber = 1, int pageSize = 10)
        {
            var pagedResult = await _service.GetBloodCollectionsByPaged(eventId, pageNumber, pageSize);

            if (pagedResult == null)
                return NotFound(new ApiResponse<PaginatedResultBloodProce>
                {
                    IsSuccess = false,
                    Message = "Not found event."
                });

            return Ok(new ApiResponse<PaginatedResultBloodProce>
            {
                IsSuccess = true,
                Message = "Blood collections retrieved successfully.",
                Data = pagedResult
            });
        }
    }
}
