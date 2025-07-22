using Application.DTO;
using Application.DTO.BloodInventoryDTO;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Helper;
using Infrastructure.Repository.Blood;
using Infrastructure.Repository.BloodInventoryRepo;
using Infrastructure.Repository.Events;
using Microsoft.AspNetCore.Http;

namespace Application.Service.BloodInventoryServ
{
    public class BloodInventoryService(IBloodInventoryRepository _repo, IBloodTypeRepository _repoBloodType,
        IHttpContextAccessor _contextAccessor, IEventRepository _repoEvent) : IBloodInventoryService
    {
        public async Task<ApiResponse<BloodInventory>> DeleteABloodUnitAsync(int id)
        {
            ApiResponse<BloodInventory> apiResponse = new();

            // Kiểm tra blood unit có sẵn sàng hay không
            var bloodUnit = await _repo.GetByIdAsync(id);
            if (bloodUnit == null || bloodUnit.IsAvailable == false)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Blood unit not found or not available.";
                return apiResponse;
            }

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
                throw new UnauthorizedAccessException("User not found or invalid");

            bloodUnit.RemoveBy = creatorId;
            bloodUnit.UpdateAt = TimeHelper.NowVietnam;
            bloodUnit.IsAvailable = false;
            await _repo.UpdateAsync(bloodUnit);

            apiResponse.IsSuccess = true;
            apiResponse.Message = "Delete a blood unit successfully.";

            return apiResponse;
        }

        public async Task<PaginatedResult<BloodInventoryResponse>> GetBloodUnitsByPagedAsync(int pageNumber, int pageSize)
        {
            var bloodUnitsPagedRaw = await _repo.GetBloodUnitsByPagedAsync(pageNumber, pageSize);

            var pagedResult = new PaginatedResult<BloodInventoryResponse>()
            {
                Items = new List<BloodInventoryResponse>(),
                TotalItems = bloodUnitsPagedRaw.TotalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            foreach (var bu in bloodUnitsPagedRaw.Items)
            {
                var bloodUnitResponse = new BloodInventoryResponse
                {
                    BloodUnitId = bu.Id,
                    CreateAt = bu.CreateAt,
                    BloodTypeName = (await _repoBloodType.GetBloodTypeByIdAsync(bu.BloodTypeId))?.Type,
                    BloodRegisId = bu.RegistrationId,
                    BloodAge = (bu.ExpiredDate - TimeHelper.NowVietnam).Days,
                    IsAvailable = bu.IsAvailable,
                    Volume = bu.Volume
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

        public async Task<ApiResponse<List<BloodInventoryAlertResponse>>> AlertAboutBloodInventoryAsync()
        {
            var bloodUnits = await _repo.GetAllAsync();

            var apiResponse = new ApiResponse<List<BloodInventoryAlertResponse>>
            {
                IsSuccess = true,
                Message = "Alter executed successfully.",
                Data = new List<BloodInventoryAlertResponse>()
            };

            if (bloodUnits == null || !bloodUnits.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "No blood units found.";
                return apiResponse;
            }

            var urgentEvents = await _repoEvent.GetAllEventNotPagedAsync();
            var existingUrgentEventSet = new HashSet<(int? bloodTypeId, BloodComponent? bloodComponent)>(
                urgentEvents
                    .Where(e => e.IsExpired == false && e.IsUrgent == true)
                    .Select(e => (e.BloodTypeId, e.BloodComponent))
            );

            var bloodTypeIds = (await _repoBloodType.GetAllBloodTypeAsync())
                    .Select(bt => bt.Id)
                    .ToList();
            // Real'
            List<(int bloodTypeId, BloodComponent bloodComponent, float Volume)> allCombinations = bloodTypeIds
                    .SelectMany(bloodTypeId => Enum.GetValues(typeof(BloodComponent))
                        .Cast<BloodComponent>()
                        .Select(bloodComponent => (bloodTypeId, bloodComponent, Volume: 0f)))
                    .ToList();

            var bloodTypeDict = (await _repoBloodType.GetAllBloodTypeAsync())
                .ToDictionary(bt => bt.Id, bt => bt.Type);

            // Real
            apiResponse.Data = allCombinations
                .GroupJoin(
                    bloodUnits,
                    combo => (combo.bloodTypeId, combo.bloodComponent),
                    inv => (inv.BloodTypeId, inv.BloodComponent),
                    (combo, group) => new
                    {
                        combo.bloodTypeId,
                        combo.bloodComponent,
                        Volume = group.Sum(bu => bu.Volume)
                    })
                .Where(x => x.Volume < 500)
                .Where(x => !existingUrgentEventSet.Contains((x.bloodTypeId, x.bloodComponent)))
                .Select(x => new BloodInventoryAlertResponse
                {
                    BloodTypeName = bloodTypeDict.GetValueOrDefault(x.bloodTypeId, "Unknown"),
                    BloodComponentName = x.bloodComponent.ToString(),
                    Volume = x.Volume
                })
                .ToList();

            // Test
            //apiResponse.Data = bloodTypeIds
            //    .GroupJoin(
            //        bloodUnits,
            //        combo => (combo),
            //        inv => (inv.BloodTypeId),
            //        (combo, group) => new
            //        {
            //            combo,
            //            Volume = group.Sum(bu => bu.Volume)
            //        })
            //    .Where(x => x.Volume < 500)
            //    .Where(x => !existingUrgentEventSet.Contains(x.combo))
            //    .Select(x => new BloodInventoryAlertResponse
            //    {
            //        BloodTypeName = bloodTypeDict.GetValueOrDefault(x.combo, "Unknown")
            //    })
            //    .ToList();

            if (!apiResponse.Data.Any())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "No blood inventory alerts found.";
            }

            return apiResponse;
        }

        public async Task<ApiResponse<List<BloodInventorySum>>> SummarizeBloodUnitsAsync()
        {
            var bloodTypeIds = (await _repoBloodType.GetAllBloodTypeAsync())
                .Select(bt => bt.Id)
                .ToList();

            var bloodUnits = (await _repo.GetAllAsync())
                .Where(bu => bu.IsAvailable);

            var bloodTypeDict = (await _repoBloodType.GetAllBloodTypeAsync())
                .ToDictionary(bt => bt.Id, bt => bt.Type);

            var apiResponse = new ApiResponse<List<BloodInventorySum>>
            {
                IsSuccess = true,
                Message = "Summarization executed successfully.",
                Data = bloodTypeIds
                        .GroupJoin(
                            bloodUnits,
                            bt => bt, 
                            bu => bu.BloodTypeId,
                            (bt, group) => new
                            {
                                BloodTypeId = bt,
                                Volume = group.Sum(bu => bu.Volume)
                            })
                        .Select(x => new BloodInventorySum
                        {
                            BloodTypeName = bloodTypeDict.GetValueOrDefault(x.BloodTypeId, "Unknow"),
                            BloodUnitCount = (int)Math.Round(x.Volume / 200)
                        })
                        .ToList()
            };

            return apiResponse;
        }
    }
}