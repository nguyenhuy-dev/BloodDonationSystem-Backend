using Application.Service.BloodCompatibilitySer;
using Domain.Enums;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [EnableCors("LocalPolicy")]
    [ApiController]
    public class BloodCompatibilitiesController(IBloodCompatibilityService _bloodCompatibilityService) : ControllerBase
    {
        [HttpGet("api/bloodcompatibilities/receive/{receipientId}/{component}")]
        public async Task<IActionResult> GetCompatibilityDonors(int receipientId, [FromRoute]BloodComponent component)
        {
            var result = await _bloodCompatibilityService.GetCompatibilityDonors(receipientId, component);
            return Ok(new
            {
                IsSuccess = true,
                Message = "Can receive from",
                Data = result
            });
        }

        [HttpGet("api/bloodcompatibilities/donate/{donorId}/{component}")]
        public async Task<IActionResult> GetCompatibilityRecipients(int donorId, [FromRoute] BloodComponent component)
        {
            var result = await _bloodCompatibilityService.GetCompatibilityRecipients(donorId, component);
            return Ok(new
            {
                IsSuccess = true,
                Message = "Can donate to",
                Data = result
            });
        }
    }
}
