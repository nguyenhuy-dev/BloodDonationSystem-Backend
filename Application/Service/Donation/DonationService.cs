using Application.DTO.DonationDTO;
using Domain.Entities;
using Infrastructure.Repository.Donation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Donation
{
    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _repo;
        //private readonly IEmailService _email;

        public DonationService(IDonationRepository repo)
        {
            _repo = repo;
            //_email = email;
        }

        public async Task<int> RegisterDonationAsync(RegisterDonationDto dto)
        {
            var reg = new Registration
            {
                CreateAt = DateTime.UtcNow,
                UserId = dto.UserId,
                EventId = dto.EventId,
                IsVolunteer = dto.IsVolunteer
            };

            await _repo.CreateRegistrationAsync(reg);
            var user = await _repo.GetUserByIdAsync(dto.UserId);
            var ev = await _repo.GetEventByIdAsync(dto.EventId);

            //await _email.SendDonationScheduleEmail(user.Email, ev);
            return reg.Id;
        }

        public async Task CheckInDonorAsync(int regId, Guid staffId)
        {
            var hist = new DonationHistory
            {
                RegistrationId = regId,
                CreateAt = DateTime.UtcNow
            };

            await _repo.CreateDonationHistoryAsync(hist);
            await _repo.AddProcessStepAsync(new DonationProcessStep
            {
                StepName = "Check-In",
                DonationHistoryId = hist.Id,
                PerformedBy = staffId,
                PerformedAt = DateTime.UtcNow
            });

            await _repo.SaveAsync();
        }

        public async Task<bool> ConductMedicalCheckupAsync(int histId, bool healthy, Guid staffId)
        {
            var hist = await _repo.GetDonationHistoryByIdAsync(histId);
            hist.HealthStatus = healthy;

            await _repo.AddProcessStepAsync(new DonationProcessStep
            {
                StepName = "Medical Checkup",
                DonationHistoryId = histId,
                PerformedBy = staffId,
                PerformedAt = DateTime.UtcNow,
                Description = healthy ? "Healthy" : "Unfit"
            });

            if (!healthy)
            {
                await CancelDonationAsync(histId, staffId, "Health check failed");
                return false;
            }

            await _repo.SaveAsync();
            return true;
        }

        public async Task<bool> CollectBloodAsync(int histId, float volume, bool isQualified, Guid staffId)
        {
            var hist = await _repo.GetDonationHistoryByIdAsync(histId);
            hist.Volume = volume;
            hist.BloodStatus = isQualified;

            if (isQualified)
            {
                var comp = await _repo.GetDefaultBloodComponentAsync();
                var inv = new Inventory
                {
                    BloodTypeId = hist.Registration.User.BloodTypeId,
                    BloodComponentId = comp.Id,
                    CreateAt = DateTime.UtcNow,
                    ExpiredDate = DateTime.UtcNow.AddDays(42),
                    DonationHistoryId = hist.Id
                };
                await _repo.AddToInventoryAsync(inv);

                await _repo.AddProcessStepAsync(new DonationProcessStep
                {
                    StepName = "Blood Qualified",
                    DonationHistoryId = histId,
                    PerformedBy = staffId,
                    PerformedAt = DateTime.UtcNow
                });

                //await _email.SendPostDonationConfirmationEmail(hist.Registration.User.Email);
            }
            else
            {
                await CancelDonationAsync(histId, staffId, "Blood not qualified");
                return false;
            }

            await _repo.SaveAsync();
            return true;
        }

        private async Task CancelDonationAsync(int histId, Guid staffId, string reason)
        {
            await _repo.AddProcessStepAsync(new DonationProcessStep
            {
                StepName = "Cancel",
                DonationHistoryId = histId,
                PerformedBy = staffId,
                PerformedAt = DateTime.UtcNow,
                Description = reason
            });

            var user = (await _repo.GetDonationHistoryByIdAsync(histId)).Registration.User;
            //await _email.SendCancellationEmail(user.Email, reason);

            await _repo.SaveAsync();
        }
    }
}
