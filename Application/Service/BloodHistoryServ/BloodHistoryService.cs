using Application.DTO.BloodHistoryDTO;
using Domain.Entities;
using Infrastructure.Repository.BloodInventoryRepo;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.VolunteerRepo;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BloodHistoryServ
{
    public class BloodHistoryService(IBloodRegistrationRepository _bloodRegistration,
                                     IVolunteerRepository _volunteerRepository,
                                     IHttpContextAccessor _contextAccessor) : IBloodHistoryService
    {
        public async Task<List<UnifiedBloodHistory>> GetBloodRegistraionHistoryAsync()
        {
            var user = _contextAccessor.HttpContext?.User.FindFirst("UserId").Value;
            if (string.IsNullOrEmpty(user) || !Guid.TryParse(user, out Guid userId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            var bloodRegistrations = await _bloodRegistration.GetBloodRegistrationHistoryAsync(userId);

            var volunteerRegistrations = await _bloodRegistration.GetVolunteerRegistrationHistoryAsync(userId);

            var result = new List<UnifiedBloodHistory>();

            if(bloodRegistrations != null)
            {
                var donationHistory = bloodRegistrations
                    .Where(br => br.IsApproved == null)
                    .Select(b => new UnifiedBloodHistory
                {
                    Id = b.Id,
                    Type = "Donation",
                    FacilityName = b.Event.Facility.Name,
                    EventName = b.Event.Title,
                    EventDate = b.Event.EventTime,
                    Longitude = b.Event.Facility.Longitude,
                    Latitude = b.Event.Facility.Latitude,
                    RegisterDate = DateOnly.FromDateTime(b.CreateAt)
                });
                result.AddRange(donationHistory);
            }

            if (volunteerRegistrations != null)
            {
                var volunteerHistory = volunteerRegistrations.Select(v => new UnifiedBloodHistory
                {
                    Id = v.Id,
                    Type = "Volunteer",
                    FacilityName = v.Event.Facility.Name,
                    Longitude = v.Event.Facility.Longitude,
                    Latitude = v.Event.Facility.Latitude,
                    RegisterDate = DateOnly.FromDateTime(v.CreateAt),
                    StartDate = DateOnly.FromDateTime(v.Volunteer.StartVolunteerDate),
                    EndDate = DateOnly.FromDateTime(v.Volunteer.EndVolunteerDate),
                    IsExpired = v.Volunteer.IsExpired
                });
                result.AddRange(volunteerHistory);
            }
            return result.ToList();
        }

        public async Task<List<DonationHistory>> GetDonationHistoryAsync()
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId").Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid id))
            {
                return null;
            }

            var donationHistory = await _bloodRegistration.GetDonationHistoryAsync(id);

            if(donationHistory == null)
            {
                return null;
            }

            var donation = donationHistory
                .Where(br => br.IsApproved != null)
                .Select(br => new DonationHistory
            {
                RegistrationId = br.Id,
                DonateDate = br.Event.EventTime,
                FacilityName = br.Event.Facility.Name,
                FacilityAddress = br.Event.Facility.Address,
                Longitude = br.Event.Facility.Longitude,
                Latitude = br.Event.Facility.Latitude,
                Status = br.IsApproved,
                Volume = br.BloodInventory?.Volume,
                Description = br.IsApproved == true ? (br.BloodInventory != null ? "Hiến máu thành công" : "Chờ hiến máu")
                            : br.IsApproved == false ? "Bị từ chối"
                            : br.HealthProcedure?.IsHealth == false ? br.HealthProcedure?.Description
                            : br.BloodProcedure?.IsQualified == false ? br.BloodProcedure?.Description
                            : br.Event.EventTime > DateOnly.FromDateTime(DateTime.Now) ? "Chưa đến thời gian hiến máu"
                            : "Không đạt chuẩn"
            }).ToList();

            return donation;
        }

        public async Task<bool> UpdateAvailableDateVolunteerAsync(int id, UpdateAvailableDateDTO dto)
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId").Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid uId))
            {
                return false;
            }

            var updated = await _volunteerRepository.UpdateAvailableDateAsync(id, dto.StartDate, dto.EndDate);
            if (!updated)
            {
                return false;
            }

            return true;
        }
    }
}
