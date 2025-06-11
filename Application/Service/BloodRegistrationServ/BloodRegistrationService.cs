using Application.DTO.BloodRegistration;
using Domain.Entities;
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

        public async Task<int> RegisterDonation(BloodRegistrationRequest request)
        {
            var registration = new BloodRegistration
            {
                CreateAt = DateTime.UtcNow,
                MemberId = request.MemberId,
                EventId = request.EventId,
                Status = 0,
                UpdateAt = DateTime.UtcNow, 
                VolunteerId = 2,
                HealthId = 2,
                BloodProcedureId = 2,
                StaffId = request.MemberId
            };

            await _repository.AddAsync(registration);
            return registration.Id;
        }
    }
}
