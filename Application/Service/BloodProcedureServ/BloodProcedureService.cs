using Application.DTO;
using Application.DTO.BloodProcedureDTO;
using Application.Service.EmailServ;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Helper;
using Infrastructure.Repository.BloodInventoryRepo;
using Infrastructure.Repository.BloodProcedureRepo;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.HealthProcedureRepo;
using Infrastructure.Repository.Users;
using Microsoft.AspNetCore.Http;

namespace Application.Service.BloodProcedureServ
{
    public class BloodProcedureService(IBloodProcedureRepository _repo, IBloodRegistrationRepository _repoRegis,
        IHealthProcedureRepository _repoHealth, IBloodInventoryRepository _repoInven,
        IHttpContextAccessor _contextAccessor, IEmailService _servEmail, 
        IEventRepository _repoEvent, IUserRepository _repoUser) : IBloodProcedureService
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
                    Phone = item.BloodRegistration.Member.Phone,
                    BloodTypeName = item.BloodRegistration.Member.BloodType?.Type,
                    PerformedAt = item.PerformedAt,
                    IsQualified = item.IsQualified
                });
            }
            return pageResult;
        }

        public async Task<ApiResponse<BloodProcedure>?> RecordBloodCollectionAsync(int id, BloodCollectionRequest request)
        {
            ApiResponse<BloodProcedure> apiResponse = new();

            // Kiểm tra đơn đăng ký hiến máu không tồn tại
            var bloodRegistration = await _repoRegis.GetByIdAsync(id);
            if (bloodRegistration == null || bloodRegistration.IsApproved == false)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Blood registration not found or being rejected."; 
                return apiResponse;
            }

            // Kiểm tra xem đã khám sức khỏe hay chưa (hoặc sức khỏe đảm bảo), mới được lấy máu
            var healthProcedure = await _repoHealth.GetByIdAsync(bloodRegistration.HealthId);
            if (healthProcedure == null || healthProcedure.IsHealth == false)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Not yet health procedure or health is not good.";
                return apiResponse;
            }

            // Lấy máu rồi thì không được lấy nữa
            if (bloodRegistration.BloodProcedureId != null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Already collected blood.";
                return apiResponse;
            }

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

            // Update lại lần cuối hiến máu
            var member = await _repoUser.GetUserByIdAsync(bloodRegistration.MemberId);
            if (member == null)
                throw new UnauthorizedAccessException("User not found or invalid");
            var existedEvent = await _repoEvent.GetEventByIdAsync(bloodRegistration.EventId);
            member.LastDonation = existedEvent?.EventTime.ToDateTime(TimeOnly.MinValue);
            await _repoUser.UpdateUserProfileAsync(member);

            // Update lại cho table BloodRegistrations
            bloodRegistration.BloodProcedureId = bloodCollectionAdded.Id;
            bloodRegistration.UpdateAt = DateTime.Now;
            bloodRegistration.StaffId = creatorId;
            await _repoRegis.UpdateAsync(bloodRegistration);

            // Gửi mail thông báo cho người hiến máu
            await _servEmail.SendEmailBloodCollectionAsync(bloodRegistration);

            apiResponse.IsSuccess = true;
            apiResponse.Message = "Blood collection recorded successfully.";
            return apiResponse;
        }

        public async Task<PaginatedResultWithEventTime<SearchBloodProcedureDTO>?> SearchBloodCollectionsByPhoneOrName(int pageNumber, int pageSize, string keyword, int? eventId = null)
        {
            var bloodCollections = await _repo.SearchBloodCollectionsByPhoneOrNameAsync(pageNumber, pageSize, keyword, eventId);

            if (bloodCollections == null || !bloodCollections.Any())
            {
                return null;
            }

            var eventTime = bloodCollections.FirstOrDefault()?.BloodRegistration?.Event?.EventTime;

            var dto = bloodCollections.Select(bc => new SearchBloodProcedureDTO
            {
                Id = bc.Id,
                DonationRegisId = bc.BloodRegistration.Id,
                Volume = bc.Volume,
                FullName = bc.BloodRegistration.Member.LastName + " " + bc.BloodRegistration.Member.FirstName,
                BloodTypeName = bc.BloodRegistration.Member.BloodType?.Type,
                PerformedAt = bc.PerformedAt,
                IsQualified = bc.IsQualified,
                Phone = bc.BloodRegistration.Member.Phone
            }).ToList();

            var totalItems = await _repo.CountAsync(bp =>
                                   (bp.BloodRegistration.Member.FirstName.Contains(keyword)
                                   || bp.BloodRegistration.Member.LastName.Contains(keyword)
                                   || bp.BloodRegistration.Member.Phone.Contains(keyword))
                                   && bp.IsQualified == null);

            return new PaginatedResultWithEventTime<SearchBloodProcedureDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                EventTime = eventTime,
                Items = dto
            };
        }

        public async Task<ApiResponse<RecordBloodQualification>?> UpdateBloodQualificationAsync(int regisId, RecordBloodQualification request)
        {
            ApiResponse<RecordBloodQualification> apiResponse = new();
            // Kiểm tra đơn đăng ký hiến máu có được chấp nhận
            var bloodRegistration = await _repoRegis.GetByIdAsync(regisId);
            if (bloodRegistration == null || bloodRegistration.IsApproved == false)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Blood registration not found or being rejected.";
                return apiResponse;
            }

            // Kiểm tra xem đã lấy máu chưa
            var bloodProcedure = await _repo.GetByIdAsync(bloodRegistration.BloodProcedureId);
            if (bloodProcedure == null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Blood collection not done.";
                return apiResponse;
            }

            // Không thể cập nhật nếu đã kiểm tra chất lượng máu
            if (bloodProcedure.IsQualified != null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Already qualified this blood unit.";
                return apiResponse;
            }

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            // Cập nhật thông tin kiểm tra chất lượng máu
            bloodProcedure.IsQualified = request.IsQualified;
            bloodProcedure.HIV = request.HIV;
            bloodProcedure.HCV = request.HCV;
            bloodProcedure.Syphilis = request.Syphilis;
            bloodProcedure.Hematocrit = request.Hematocrit;
            bloodProcedure.BloodTypeId = request.BloodTypeId;
            bloodProcedure.BloodComponent = request.BloodComponent;
            bloodProcedure.PerformedAt = DateTime.Now;
            bloodProcedure.PerformedBy = creatorId;
            await _repo.UpdateAsync(bloodProcedure);

            apiResponse.IsSuccess = true;
            apiResponse.Message = "Blood qualification recorded successfully.";
            apiResponse.Data = request;
            // Thêm máu vào kho nếu đủ điều kiện
            if (request.IsQualified == true)
            {
                await AddNewBloodUnit(bloodRegistration);
                return apiResponse;
            }

            return apiResponse;
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
                bloodInventory.ExpiredDate = bloodProcedure.PerformedAt.AddDays(35);
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
