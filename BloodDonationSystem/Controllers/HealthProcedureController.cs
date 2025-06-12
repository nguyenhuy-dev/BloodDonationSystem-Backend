using Application.DTO.HealthProcedureDTO;
using Application.Service.HealthProcedureServ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthProcedureController : ControllerBase
    {
        private readonly IHealthProcedureService _service;

        public HealthProcedureController(IHealthProcedureService service)
        {
            _service = service;
        }

        [HttpPost("record")]
        public async Task<IActionResult> RecordHealthProcedure([FromBody] HealthProcedureRequest request)
        {
            var healthProcedure = await _service.RecordHealthProcedureAsync(request);

            if (healthProcedure == null)
            {
                return BadRequest(new { message = "Health procedure recorded unsuccessfully." });
            }

            return Ok(new { message = "Health procedure recorded successfully." });
        }
    }
}
