using Application.DTO.BloodProcedureDTO;
using Application.Service.EmailServ;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repository.BloodInventoryRepo;
using Infrastructure.Repository.BloodProcedureRepo;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.HealthProcedureRepo;
using Microsoft.AspNetCore.Http;

namespace Application.Service.BloodProcedureServ
{
    public class BloodProcedureService(IBloodProcedureRepository _repo, IBloodRegistrationRepository _repoRegis,
        IHealthProcedureRepository _repoHealth, IBloodInventoryRepository _repoInven,
        IHttpContextAccessor _contextAccessor, IEmailService _servEmail, 
        IEventRepository _repoEvent) : IBloodProcedureService
    {
        public async Task<PaginatedResultBloodProce?> GetBloodCollectionsByPaged(int eventId, int pageNumber, int pageSize)
        {
            var eventExists = await _repoEvent.GetEventByIdAsync(eventId);
            if (eventExists == null)
                return null;

            var pagedResultRaw = await _repo.GetBloodCollectionsByPagedAsync(eventId, pageNumber, pageSize);

            var pageResult = new PaginatedResultBloodProce
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = pagedResultRaw.TotalItems,
                EventTime = eventExists.EventTime,
                Items = new List<BloodCollectionResponse>()
            };

            foreach (var item in pagedResultRaw.Items)
            {
                pageResult.Items.Add(new BloodCollectionResponse
                {
                    Id = item.Id,
                    DonationRegisId = item.BloodRegistration.Id,
                    Volume = item.Volume,
                    FullName = item.BloodRegistration.Member.LastName + " " + item.BloodRegistration.Member.FirstName,
                    BloodTypeName = item.BloodRegistration.Member.BloodType?.Type,
                    PerformedAt = item.PerformedAt,
                    IsQualified = item.IsQualified
                });
            }
            return pageResult;
        }

        public async Task<BloodProcedure?> RecordBloodCollectionAsync(int id, BloodCollectionRequest request)
        {
            var bloodRegistration = await _repoRegis.GetByIdAsync(id);
            if (bloodRegistration == null || bloodRegistration.IsApproved == false)
                return null;

            var healthProcedure = await _repoHealth.GetByIdAsync(bloodRegistration.HealthId);
            if (healthProcedure == null || healthProcedure.IsHealth == false)
                return null;

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            var bloodCollection = new BloodProcedure
            {
                Volume = request.Volume,
                PerformedAt = DateTime.Now,
                Description = request.Description,
                PerformedBy = creatorId
            };
            var bloodCollectionAdded = await _repo.AddAsync(bloodCollection);

            bloodRegistration.BloodProcedureId = bloodCollectionAdded.Id;
            bloodRegistration.UpdateAt = DateTime.Now;
            bloodRegistration.StaffId = creatorId;
            await _repoRegis.UpdateAsync(bloodRegistration);

            // Gửi mail thông báo cho người hiến máu
            await _servEmail.SendEmailBloodCollectionAsync(bloodRegistration);

            return bloodCollectionAdded;
        }

        public async Task<BloodProcedure?> UpdateBloodQualificationAsync(int regisId, RecordBloodQualification request)
        {
            // Kiểm tra đơn đăng ký hiến máu có được chấp nhận
            var bloodRegistration = await _repoRegis.GetByIdAsync(regisId);
            if (bloodRegistration == null || bloodRegistration.IsApproved == false)
                return null;

            // Kiểm tra xem đã lấy máu chưa
            var bloodProcedure = await _repo.GetByIdAsync(bloodRegistration.BloodProcedureId);
            if (bloodProcedure == null)
                return null;

            var existingInventory = await _repoInven.GetByBloodRegisIdAsync(bloodRegistration.Id);
            if (existingInventory != null)
                return null; // Không thể cập nhật nếu đã có đơn vị máu trong kho

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            bloodProcedure.IsQualified = request.IsQualified;
            bloodProcedure.BloodTypeId = request.BloodTypeId;
            bloodProcedure.BloodComponent = request.BloodComponent;
            bloodProcedure.PerformedAt = DateTime.Now;
            bloodProcedure.PerformedBy = creatorId;
            await _repo.UpdateAsync(bloodProcedure);  // Cập nhật thông tin kiểm tra chất lượng máu

            // Thêm máu vào kho nếu đủ điều kiện
            if (request.IsQualified == true)
            {
                await AddNewBloodUnit(bloodRegistration);
                return bloodProcedure;
            }

            return bloodProcedure;
        }

        private async Task<BloodInventory?> AddNewBloodUnit(BloodRegistration bloodRegistration)
        {
            var bloodProcedure = await _repo.GetByIdAsync(bloodRegistration.BloodProcedureId);
            if (bloodProcedure == null) return null;

            var bloodInventory = new BloodInventory
            {
                Volume = bloodProcedure.Volume,
                CreateAt = DateTime.Now,
                IsAvailable = true,
                BloodTypeId = (int) bloodProcedure.BloodTypeId,
                BloodComponent = (BloodComponent) bloodProcedure.BloodComponent,
                RegistrationId = bloodRegistration.Id
            };

            // Set ngày hết hạn dựa trên Blood Component
            if (bloodProcedure.BloodComponent == BloodComponent.WholeBlood || bloodProcedure.BloodComponent == BloodComponent.RedBloodCells)
            {
                bloodInventory.ExpiredDate = bloodProcedure.PerformedAt.AddDays(42);
            }

            if (bloodProcedure.BloodComponent == BloodComponent.Plasma)
            {
                bloodInventory.ExpiredDate = bloodProcedure.PerformedAt.AddDays(14);
            }

            if (bloodProcedure.BloodComponent == BloodComponent.Platelets)
            {
                bloodInventory.ExpiredDate = bloodProcedure.PerformedAt.AddDays(5);
            }
                
            return await _repoInven.AddAsync(bloodInventory);
        }
    }
}
