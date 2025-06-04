using Application.DTO.DonationDTO;
using Application.Service.Donation;
using Microsoft.AspNetCore.Mvc;

namespace BloodDonationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonationController : Controller
    {
        private readonly IDonationService _donationService;

        public DonationController(IDonationService donationService)
        {
            _donationService = donationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterDonation(RegisterDonationDto dto)
        {
            var id = await _donationService.RegisterDonationAsync(dto);
            return Ok(new { RegistrationId = id });
        }

        [HttpPost("{id}/checkin")]
        public async Task<IActionResult> CheckInDonor(int id, [FromQuery] Guid staffId)
        {
            await _donationService.CheckInDonorAsync(id, staffId);
            return Ok();
        }

        [HttpPost("{historyId}/medical-check")]
        public async Task<IActionResult> MedicalCheck(int historyId, [FromQuery] bool isHealthy, [FromQuery] Guid staffId)
        {
            var result = await _donationService.ConductMedicalCheckupAsync(historyId, isHealthy, staffId);
            return Ok(result);
        }

        [HttpPost("{historyId}/collect")]
        public async Task<IActionResult> CollectBlood(int historyId, [FromQuery] float volume, [FromQuery] bool isQualified, [FromQuery] Guid staffId)
        {
            var result = await _donationService.CollectBloodAsync(historyId, volume, isQualified, staffId);
            return Ok(result);
        }
    }
}
