using Application.DTO.HealthProcedureDTO;
using Domain.Entities;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.HealthProcedureRepo;
using Microsoft.AspNetCore.Http;

namespace Application.Service.HealthProcedureServ
{
    public class HealthProcedureService(IHealthProcedureRepository _repo, IBloodRegistrationRepository _repoRegis,
        IHttpContextAccessor _contextAccessor, IEventRepository _repoEvent) : IHealthProcedureService
    {
        public async Task<HealthProcedure?> CancelHealthProcessAsync(int id)
        {
            // Kiểm tra xem đơn đăng ký hiến máu có tồn tại không
            var bloodRegistration = await _repoRegis.GetByIdAsync(id);
            if (bloodRegistration == null)
                return null;

            // Kiểm tra xem đơn này sức khỏe có đảm bảo không, nếu sức khỏe không đảm bảo thì không thực hiện request
            var healthProcedure = await _repo.GetByIdAsync(bloodRegistration.HealthId);
            if (healthProcedure == null || healthProcedure.IsHealth == false)
                return null;

            // Nếu đã lấy máu rồi thì không thực hiện request
            if (bloodRegistration.BloodProcedureId != null)
                return null;

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
                throw new UnauthorizedAccessException("User not found or invalid");

            healthProcedure.IsHealth = false;
            healthProcedure.PerformedAt = DateTime.Now;
            healthProcedure.PerformedBy = creatorId;
            await _repo.UpdateAsync(healthProcedure);

            healthProcedure.BloodRegistration.IsApproved = false;
            healthProcedure.BloodRegistration.UpdateAt = DateTime.Now;
            healthProcedure.BloodRegistration.StaffId = creatorId;
            await _repoRegis.UpdateAsync(healthProcedure.BloodRegistration);

            return healthProcedure;
        }

        public async Task<object?> GetHealthProceduresByPagedAsync(int id, int pageNumber, int pageSize)
        {
            var eventExists = await _repoEvent.GetEventByIdAsync(id);
            if (eventExists == null)
                return null;

            var pagedResultRaw = await _repo.GetHealthProceduresByPaged(id, pageNumber, pageSize);

            var pagedResult = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = pagedResultRaw.TotalPages,
                TotalItems = pagedResultRaw.TotalItems,
                EventTime = _repoEvent.GetEventByIdAsync(id).Result?.EventTime,
                Items = new List<HealthProceduresResponse>()
            };

            foreach (var health in pagedResultRaw.Items)
            {
                var healthProcedure = new HealthProceduresResponse
                {
                    Id = health.Id,
                    IsHealth = health.IsHealth,
                    PerformedAt = health.PerformedAt,
                    FullName = health.BloodRegistration?.Member?.LastName + " " + health.BloodRegistration?.Member?.FirstName,
                    BloodTypeName = health.BloodRegistration?.Member?.BloodType?.Type,
                    BloodRegisId = health.BloodRegistration.Id
                };
                pagedResult.Items.Add(healthProcedure);
            }
            return pagedResult;
        }

        public async Task<HealthProcedure?> RecordHealthProcedureAsync(int id, HealthProcedureRequest request)
        {
            var bloodRegistration = await _repoRegis.GetByIdAsync(id);
            if (bloodRegistration == null || bloodRegistration.IsApproved == false)
                return null;

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            var healthProcedure = new HealthProcedure
            {
                Pressure = request.Pressure,
                Temperature = request.Temperature,
                Hb = request.Hb,
                HBV = request.HBV,
                Weight = request.Weight,
                Height = request.Height,
                IsHealth = request.IsHealth,
                PerformedAt = DateTime.Now,
                Description = request.Description,
                PerformedBy = creatorId
            };
            var healthProcedureAdded = await _repo.AddAsync(healthProcedure);

            if (healthProcedureAdded.IsHealth == true)
                bloodRegistration.IsApproved = true;
            else
                bloodRegistration.IsApproved = false;
            bloodRegistration.HealthId = healthProcedureAdded.Id;
            bloodRegistration.UpdateAt = DateTime.Now;
            bloodRegistration.StaffId = creatorId;
            await _repoRegis.UpdateAsync(bloodRegistration);

            return healthProcedureAdded;
        }
    }
}