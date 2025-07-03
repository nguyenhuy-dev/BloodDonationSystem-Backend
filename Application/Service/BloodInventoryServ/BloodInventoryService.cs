using Application.DTO;
using Application.DTO.BloodInventoryDTO;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Helper;
using Infrastructure.Repository.Blood;
using Infrastructure.Repository.BloodInventoryRepo;
using Microsoft.AspNetCore.Http;

namespace Application.Service.BloodInventoryServ
{
    public class BloodInventoryService(IBloodInventoryRepository _repo, IBloodTypeRepository _repoBloodType,
        IHttpContextAccessor _contextAccessor) : IBloodInventoryService
    {
        public async Task<ApiResponse<BloodInventory>> DeleteABloodUnitAsync(int id)
        {
            ApiResponse<BloodInventory> apiResponse = new();

            var bloodUnit = await _repo.GetByIdAsync(id);
            if (bloodUnit == null || bloodUnit.IsAvailable == false)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Blood unit not found or not available";
                return apiResponse;
            }

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
                throw new UnauthorizedAccessException("User not found or invalid");

            bloodUnit.RemoveBy = creatorId;
            bloodUnit.UpdateAt = DateTime.Now;
            bloodUnit.IsAvailable = false;
            await _repo.UpdateAsync(bloodUnit);

            apiResponse.IsSuccess = true;
            apiResponse.Message = "Delete a blood unit successfully.";

            return apiResponse;
        }

        public async Task<PaginatedResult<BloodInventoryResponse>> GetBloodUnitsByPagedAsync(int pageNumber, int pageSize)
        {
            var bloodUnits = (await _repo.GetAllAsync())
                .OrderBy(bu => bu.IsAvailable == true)
                    .ThenBy(bu => bu.CreateAt)
                .OrderBy(bu => bu.IsAvailable == false)
                    .ThenBy(bu => bu.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var pagedResult = new PaginatedResult<BloodInventoryResponse>()
            {
                Items = new List<BloodInventoryResponse>(),
                TotalItems = (await _repo.GetAllAsync()).Count(),
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            foreach (var bu in bloodUnits)
            {
                var bloodUnitResponse = new BloodInventoryResponse
                {
                    BloodUnitId = bu.Id,
                    CreateAt = bu.CreateAt,
                    BloodTypeName = (await _repoBloodType.GetBloodTypeByIdAsync(bu.BloodTypeId))?.Type,
                    BloodRegisId = bu.RegistrationId,
                    BloodAge = (bu.ExpiredDate - DateTime.Now).Days,
                    IsAvailable = bu.IsAvailable
                };
                if (bu.BloodComponent == BloodComponent.WholeBlood || bu.BloodComponent == BloodComponent.RedBloodCells)
                {
                    bloodUnitResponse.ExpiredDate = "35 ngày";
                    bloodUnitResponse.BloodComponentName = bu.BloodComponent == BloodComponent.WholeBlood ? "Toàn phần" : "Hồng cầu";
                }

                if (bu.BloodComponent == BloodComponent.Plasma)
                {
                    bloodUnitResponse.ExpiredDate = "14 ngày";
                    bloodUnitResponse.BloodComponentName = "Huyết tương";
                }

                if (bu.BloodComponent == BloodComponent.Platelets)
                {
                    bloodUnitResponse.ExpiredDate = "5 ngày";
                    bloodUnitResponse.BloodComponentName = "Tiểu cầu";
                }

                pagedResult.Items.Add(bloodUnitResponse);
            }
            
            return pagedResult;
        }
    }
}