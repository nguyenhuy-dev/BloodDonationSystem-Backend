using Application.DTO;
using Application.DTO.HealthProcedureDTO;
using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.HealthProcedureRepo;
using Microsoft.AspNetCore.Http;
using MimeKit.Cryptography;

namespace Application.Service.HealthProcedureServ
{
    public class HealthProcedureService(IHealthProcedureRepository _repo, IBloodRegistrationRepository _repoRegis,
        IHttpContextAccessor _contextAccessor, IEventRepository _repoEvent) : IHealthProcedureService
    {
        public async Task<ApiResponse<HealthProcedure>?> CancelHealthProcessAsync(int id)
        {
            ApiResponse<HealthProcedure> apiResponse = new();

            // Kiểm tra xem đơn đăng ký hiến máu có tồn tại không
            var bloodRegistration = await _repoRegis.GetByIdAsync(id);
            if (bloodRegistration == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Blood registration not found.";
                return apiResponse;
            }

            // Kiểm tra xem đơn này sức khỏe có đảm bảo không, nếu sức khỏe không đảm bảo thì không thực hiện request
            var healthProcedure = await _repo.GetByIdAsync(bloodRegistration.HealthId);
            if (healthProcedure == null || healthProcedure.IsHealth == false)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Health was not good.";
                return apiResponse;
            }

            // Nếu đã lấy máu rồi thì không thực hiện request
            if (bloodRegistration.BloodProcedureId != null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Blood collection being completed.";
                return apiResponse;
            }

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

            apiResponse.IsSuccess = true;
            apiResponse.Message = "Health procedure cancelled successfully.";
            return apiResponse;
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
                    Phone = health.BloodRegistration?.Member?.Phone,
                    BloodTypeName = health.BloodRegistration?.Member?.BloodType?.Type,
                    BloodRegisId = health.BloodRegistration.Id
                };
                pagedResult.Items.Add(healthProcedure);
            }
            return pagedResult;
        }

        public async Task<HealthProcedure?> RecordHealthProcedureAsync(int id, HealthProcedureRequest request)
        {
            // Nếu đơn đăng ký Not Found hoặc đã khám thì không được record
            var bloodRegistration = await _repoRegis.GetByIdAsync(id);
            if (bloodRegistration == null || bloodRegistration.IsApproved != null)
                return null;

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            var healthProcedure = new HealthProcedure
            {
                Systolic = request.Systolic,
                Diastolic = request.Diastolic,
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

        public async Task<PaginatedResultWithEventTime<SearchHealthProcedureDTO>?> SearchHealthProceduresByPhoneOrNameAsync(int pageNumber, int pageSize, string keyword, int? eventId = null)
        {
            var healthProcedures = await _repo.SearchHealthProceduresByNameOrPhoneAsync(pageNumber, pageSize, keyword, eventId);

            if (healthProcedures == null || !healthProcedures.Any())
            {
                return null;
            }

            var eventTime = healthProcedures.FirstOrDefault()?.BloodRegistration?.Event?.EventTime;

            var dto = healthProcedures.Select(hp => new SearchHealthProcedureDTO
            {
                Id = hp.Id,
                IsHealth = hp.IsHealth,
                PerformedAt = hp.PerformedAt,
                Phone = hp.BloodRegistration.Member.Phone,
                FullName = hp.BloodRegistration?.Member?.LastName + " " + hp.BloodRegistration?.Member?.FirstName,
                BloodTypeName = hp.BloodRegistration?.Member?.BloodType?.Type,
                BloodRegisId = hp.BloodRegistration.Id
            }).ToList();

            var totalItems = await _repo.CountAsync(hp =>
                                   (hp.BloodRegistration.Member.FirstName.Contains(keyword)
                                   || hp.BloodRegistration.Member.LastName.Contains(keyword)
                                   || hp.BloodRegistration.Member.Phone.Contains(keyword))
                                   && hp.BloodRegistration.IsApproved == true
                                   && hp.BloodRegistration.BloodProcedureId == null);

            return new PaginatedResultWithEventTime<SearchHealthProcedureDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                EventTime = eventTime,
                Items = dto
            };
        }
    }
}