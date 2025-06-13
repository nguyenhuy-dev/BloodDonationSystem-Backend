using Application.DTO.BloodRegistration;
using Application.DTO.BloodRegistrationDTO;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repository.BloodRegistrationRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BloodRegistrationServ
{
    public class BloodRegistrationService : IBloodRegistrationService
    {
        private readonly IBloodRegistrationRepository _repository;

        public BloodRegistrationService(IBloodRegistrationRepository repository)
        {
            _repository = repository;
        }

        public async Task<BloodRegistration?> RegisterDonation(BloodRegistrationRequest request)
        {
            var registration = new BloodRegistration
            {
                CreateAt = DateTime.UtcNow,
                MemberId = request.MemberId,
                EventId = request.EventId
            };

            await _repository.AddAsync(registration);
            return registration;
        }

        public async Task<BloodRegistration?> EvaluateRegistration(int bloodRegisId, EvaluateBloodRegistration evaluate)
        {
            var bloodRegistration = await _repository.GetByIdAsync(bloodRegisId);
            if (bloodRegistration == null)
                return null;

            bloodRegistration.Status = evaluate.Status;
            bloodRegistration.UpdateAt = DateTime.Now;
            bloodRegistration.StaffId = evaluate.StaffId;

            await _repository.UpdateAsync(bloodRegistration);
            return bloodRegistration;
        }
    }
}
