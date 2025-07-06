using Application.DTO;
using Application.DTO.BloodProcedureDTO;
using Application.Service.BloodProcedureServ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Cryptography;

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
            var apiResponse = await _service.RecordBloodCollectionAsync(bloodRegisId, request);

            if (apiResponse?.IsSuccess == false)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
        }

        [Authorize(Roles = "Staff")]
        [HttpPost("api/blood-registrations/{bloodRegisId}/blood-procedures/qualify")]
        public async Task<IActionResult> RecordBloodQualification(int bloodRegisId, [FromBody] RecordBloodQualification request)
        {
            var apiResponse = await _service.UpdateBloodQualificationAsync(bloodRegisId, request);

            if (apiResponse?.IsSuccess == false)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
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

        [Authorize(Roles = "Staff")]
        [HttpGet("api/blood-procedures/search")]
        public async Task<IActionResult> SearchBloodProcedures([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string keyword = null)
        {
            var pagedResult = await _service.SearchBloodCollectionsByPhoneOrName(pageNumber, pageSize, keyword);
            if (pagedResult == null || !pagedResult.Items.Any())
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = "No blood procedures found."
                });
            return Ok(new
            {
                IsSuccess = true,
                Message = "Blood procedures retrieved successfully.",
                Data = pagedResult
            });
        }
    }
}
