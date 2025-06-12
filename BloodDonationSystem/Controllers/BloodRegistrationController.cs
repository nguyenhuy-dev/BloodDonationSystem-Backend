using Application.DTO.BloodRegistration;
using Application.Service.BloodRegistrationServ;
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
            var registration = await _service.RegisterDonation(request);
            return Ok(new
            {
                Message = "Register donation successfully"
            });
        }
    }
}
