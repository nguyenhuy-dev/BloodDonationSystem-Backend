using Application.DTO;
using Application.DTO.BloodInventoryDTO;
using Application.Service.BloodInventoryServ;
using Infrastructure.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [ApiController]
    public class BloodInventoriesController(IBloodInventoryService _service) : ControllerBase
    {
        [Authorize(Roles = "Staff")]
        [HttpGet("api/blood-inventories/paged")]
        public async Task<IActionResult> GetBloodUnitsByPaged(int pageNumber = 1, int pageSize = 10)
        {
            var pagedResult = await _service.GetBloodUnitsByPagedAsync(pageNumber, pageSize);
            return Ok(new ApiResponse<PaginatedResult<BloodInventoryResponse>>
            {
                IsSuccess = true,
                Message = "Blood inventories retrieved successfully.", 
                Data = pagedResult
            });
        }

        [Authorize(Roles = "Staff")]
        [HttpPut("api/blood-inventories/{bloodUnitId}/delete")]
        public async Task<IActionResult> DeleteABloodUnit(int bloodUnitId)
        {
            var apiResponse = await _service.DeleteABloodUnitAsync(bloodUnitId);

            if (apiResponse?.IsSuccess == false)
                return NotFound(apiResponse);

            return Ok(apiResponse);
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("api/blood-inventories/alert")]
        public async Task<IActionResult> AlertAboutBloodInventoryAsync()
        {
            var apiResponse = await _service.AlertAboutBloodInventoryAsync();

            if (apiResponse?.IsSuccess == false)
                return BadRequest(apiResponse);

            return Ok(apiResponse);
        }
    }
}
