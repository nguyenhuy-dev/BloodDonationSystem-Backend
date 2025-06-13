using Application.DTO.BloodRegistration;
using Application.DTO.BloodRegistrationDTO;
using Application.Service.BloodRegistrationServ;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodRegistrationController : ControllerBase
    {
        private readonly IBloodRegistrationService _service;

        public BloodRegistrationController(IBloodRegistrationService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDonation([FromBody] BloodRegistrationRequest request)
        {
            var bloodRegistration = await _service.RegisterDonation(request);
            return Ok(new
            {
                Message = "Register donation successfully"
            });
        }

        [HttpPatch("evaluate/{id}")]
        public async Task<IActionResult> EvaluateRegistration(int id, [FromBody] EvaluateBloodRegistration evaluate)
        {
            var bloodRegistration = await _service.EvaluateRegistration(id, evaluate);
            if (bloodRegistration == null)
                return NotFound(new
                {
                    Message = "Blood registration not found"
                });

            return Ok(new
            {
                Message = "Evaluate registration successfully",
            });
        }
    }
}
