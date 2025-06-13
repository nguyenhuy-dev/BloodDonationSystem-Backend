using Application.DTO.BloodProcedureDTO;
using Application.Service.BloodProcedureServ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace BloodDonationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BloodProcedureController : ControllerBase
    {
        private readonly IBloodProcedureService _service;

        public BloodProcedureController(IBloodProcedureService service)
        {
            _service = service;
        }

        [HttpPost("record-blood-collection")]
        public async Task<IActionResult> RecordBloodCollection([FromBody] BloodCollectionRequest request)
        {
            var bloodCollection = await _service.RecordBloodCollectionAsync(request);

            if (bloodCollection == null)
            {
                return BadRequest(new
                {
                    Message = "Failed to record blood collection."
                });
            }

            return Ok(new
            {
                Message = "Blood collection recorded successfully."
            });
        }
    }
}
