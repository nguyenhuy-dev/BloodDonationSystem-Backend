using Application.DTO.BloodProcedureDTO;
using Domain.Entities;
using Infrastructure.Repository.BloodProcedureRepo;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.HealthProcedureRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BloodProcedureServ
{
    public class BloodProcedureService : IBloodProcedureService
    {
        private readonly IBloodProcedureRepository _repo;
        private readonly IBloodRegistrationRepository _repoRegis;
        private readonly IHealthProcedureRepository _repoHealth;

        public BloodProcedureService(IBloodProcedureRepository repo, IBloodRegistrationRepository repoRegis, IHealthProcedureRepository repoHealth)
        {
            _repo = repo;
            _repoRegis = repoRegis;
            _repoHealth = repoHealth;
        }

        public async Task<BloodProcedure?> RecordBloodCollectionAsync(BloodCollectionRequest request)
        {
            var bloodRegistration = await _repoRegis.GetByIdAsync(request.BloodRegistrationId);
            //if (bloodRegistration == null || bloodRegistration.HealthProcedure == null || bloodRegistration.HealthProcedure.IsHealth == false)
            //    return null;

            if (bloodRegistration == null)
                return null;

            var healthProcedure = await _repoHealth.GetByIdAsync(bloodRegistration.HealthId);
            if (healthProcedure == null || healthProcedure.IsHealth == false)
                return null;

            var bloodCollection = new BloodProcedure
            {
                Volume = request.Volume,
                PerformedAt = DateTime.Now,
                Description = request.Description,
                PerformedBy = request.PerformedBy
            };

            var bloodCollectionAdded = await _repo.AddAsync(bloodCollection);
            bloodRegistration.BloodProcedureId = bloodCollectionAdded.Id;
            await _repoRegis.UpdateAsync(bloodRegistration);

            return bloodCollectionAdded;
        }

        public async Task<BloodProcedure?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }
    }
}
