using Application.DTO.HealthProcedureDTO;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.HealthProcedureRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.HealthProcedureServ
{
    public class HealthProcedureService : IHealthProcedureService
    {
        private readonly IHealthProcedureRepository _repo;
        private readonly IBloodRegistrationRepository _repoRegis;

        public HealthProcedureService(IHealthProcedureRepository repo, IBloodRegistrationRepository repoRegis)
        {
            _repo = repo;
            _repoRegis = repoRegis; 
        }

        public async Task<HealthProcedure?> RecordHealthProcedureAsync(HealthProcedureRequest request)
        {
            var bloodRegistration = await _repoRegis.GetByIdAsync(request.BloodRegistrationId);

            if (bloodRegistration == null || bloodRegistration.Status != RegistrationStatus.Approved)
                return null;

            var healthProcedure = new HealthProcedure
            {
                Pressure = request.Pressure,
                Temperature = request.Temperature,
                Hb = request.Hb,
                HBV = request.HBV,
                Weight = request.Weight,
                Height = request.Height,
                IsHealth = request.IsHealth,
                PerformedAt = DateTime.UtcNow,
                Description = request.Description,
                PerformedBy = request.PerformedBy
            };

            var healthProcedureAdded = await _repo.AddAsync(healthProcedure);
            bloodRegistration.HealthProcedure = healthProcedureAdded;
            await _repoRegis.UpdateAsync(bloodRegistration);

            return healthProcedureAdded;
        }
    }
}
