using Application.Service.BloodTypeServ;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [EnableCors("LocalPolicy")]
    [ApiController]
    public class BloodTypeController(IBloodTypeService _bloodTypeService) : ControllerBase
    {
        [HttpGet("api/bloodtypes")]
        public async Task<IActionResult> GetAllBloodTypes()
        {
            var bloodTypes = await _bloodTypeService.GetAllBloodTypesAsync();
            if (bloodTypes == null || !bloodTypes.Any())
            {
                return NotFound(new 
                { 
                    IsSuccess = false,
                    Message = "No blood types found." 
                });
            }
            return Ok(new
            {
                IsSuccess = true,
                Message = "Blood types retrieved successfully.",
                Data = bloodTypes
            });
        }
    }
}
