using Application.DTO;
using Application.DTO.BloodRegistration;
using Application.DTO.BloodRegistrationDTO;
using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Blood;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.Users;
using Microsoft.AspNetCore.Http;

namespace Application.Service.BloodRegistrationServ
{
    public class BloodRegistrationService(IBloodRegistrationRepository _repository, IHttpContextAccessor _contextAccessor,
        IEventRepository _repoEvent, IUserRepository _repoUser, 
        IBloodTypeRepository _repoBloodType) : IBloodRegistrationService
    {

        public async Task<ApiResponse<BloodRegistration>?> RegisterDonation(int eventId, BloodRegistrationRequest request)
        {
            ApiResponse<BloodRegistration> apiResponse = new();

            // Kiểm tra event tương ứng với đơn đăng ký máu có tồn tại(event hết hạn hay chưa)
            // Không được đăng ký vào ngày diễn ra sự kiện
            var existingEvent = await _repoEvent.GetEventByIdAsync(eventId);
            if (existingEvent == null || existingEvent.IsExpired == true)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Event not found or already expired.";
                return apiResponse;
            }
            if (existingEvent.EventTime == DateOnly.FromDateTime(DateTime.Now))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Event already started.";
                return apiResponse;
            }

            // Không được đăng ký khi đạt tới MaxOfDonor
            var bloodRegisList = await _repository.GetByEventAsync(eventId);
            if (bloodRegisList.Count() >= existingEvent.MaxOfDonor)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Event reached max of donor";
                return apiResponse;
            }

            // Lấy thông tin user đang thao tác form
            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }
            var user = await _repoUser.GetUserByIdAsync(creatorId);
            if (user == null)
                return null;


            //// Kiểm tra member chỉ được đăng ký hiến máu 1 lần vào 1 event 
            //var checkedRegis = _repository.GetByEventAsync(eventId).Result
            //    .FirstOrDefault(br => br.MemberId == user.Id);
            //if (checkedRegis != null)
            //{
            //    apiResponse.IsSuccess = false;
            //    apiResponse.Message = "Already registered in this event.";
            //    return apiResponse;
            //}

            // Kiểm tra xem nếu đăng ký vào urgent event, thì blood type phải hợp lệ 
            if (existingEvent.IsUrgent == true &&
                existingEvent.BloodTypeId != user.BloodTypeId)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Not suitable blood type for urgent event.";
                return apiResponse;
            }

            // Kiểm tra member đã từng hiến máu ở hệ thống lần nào chưa
            bool changedLastDonation = false;
            if (user.LastDonation == null)
            {
                user.LastDonation = request.LastDonation;
                changedLastDonation = true;
            }

            // Kiểm tra xem lần cuối hiến máu có phù hợp
            if (user.LastDonation >= DateTime.Now.AddDays(-90))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Last donation time not suitable.";
                return apiResponse;  // Request xuống đều có LastDonation nên không cần xét trong hệ thống
            }
            if (changedLastDonation == true)
                await _repoUser.UpdateUserProfileAsync(user);

            var bloodRegis = new BloodRegistration
            {
                CreateAt = DateTime.Now,
                MemberId = creatorId,
                EventId = eventId
            };
            await _repository.AddAsync(bloodRegis);

            apiResponse.IsSuccess = true;
            apiResponse.Message = "Register donation successfully.";
            return apiResponse;
        }

        public async Task<ApiResponse<BloodRegistration>?> RejectBloodRegistration(int bloodRegisId)
        {
            ApiResponse<BloodRegistration> apiResponse = new();

            // Đơn đăng ký không tồn tại hoặc đã bị reject rồi thì thôi
            var bloodRegistration = await _repository.GetByIdAsync(bloodRegisId);
            if (bloodRegistration == null || bloodRegistration.IsApproved == false)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Blood registration not exist or be rejected before.";
                return apiResponse;
            }
            
            // Khi đã lấy máu rồi thì không được hủy nữa
            if (bloodRegistration.BloodProcedureId != null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Collected blood, so can't reject.";
                return apiResponse;
            }

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            bloodRegistration.IsApproved = false;
            bloodRegistration.UpdateAt = DateTime.Now;
            bloodRegistration.StaffId = creatorId;
            await _repository.UpdateAsync(bloodRegistration);
            
            apiResponse.IsSuccess = true;
            apiResponse.Message = "Reject blood registration successfully.";
            return apiResponse;
        }

        public async Task<ApiResponse<BloodRegistration>?> CancelOwnRegistration(int id)
        {
            ApiResponse<BloodRegistration> apiResponse = new(); 

            // Check đơn có tồn tại, bị hủy, hay bị từ chối
            var bloodRegistration = await _repository.GetByIdAsync(id);
            if (bloodRegistration == null || bloodRegistration.IsApproved == false)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Blood registration not suitable.";
                return apiResponse;
            }

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }
            // Xác thực người dùng hiện tại có đang là chủ của đơn đăng ký này hay không
            if (bloodRegistration.MemberId != creatorId)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "This member not own blood registration.";
                return apiResponse;
            }

            // Check đơn đã khám hay chưa
            if (bloodRegistration.HealthId != null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Health procedure being completed.";
                return apiResponse;
            }

            // Không được hủy vào ngày diễn ra sự kiện
            var checkedEvent = await _repoEvent.GetEventByIdAsync(bloodRegistration.EventId);
            if (checkedEvent != null && checkedEvent.EventTime == DateOnly.FromDateTime(DateTime.Now))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Event being started.";
                return apiResponse;
            }

            bloodRegistration.IsApproved = false;
            bloodRegistration.UpdateAt = DateTime.Now;

            await _repository.UpdateAsync(bloodRegistration);

            apiResponse.IsSuccess = true;
            apiResponse.Message = "Cancel own registration successfully.";
            return apiResponse;
        }

        public async Task<PaginatedResultBloodRegis?> GetBloodRegistrationsByPaged(int eventId, int pageNumber, int pageSize)
        {
            var eventExists = await _repoEvent.GetEventByIdAsync(eventId);
            if (eventExists == null)
                return null;

            var pagedBloodRegisRaw = _repository.GetPagedAsync(eventId, pageNumber, pageSize);

            var pagedBloodRegis = new PaginatedResultBloodRegis
            {
                PageNumber = pagedBloodRegisRaw.Result.PageNumber,
                PageSize = pagedBloodRegisRaw.Result.PageSize,
                TotalItems = pagedBloodRegisRaw.Result.TotalItems,
                EventTime = eventExists.EventTime,
                Items = new List<BloodRegistrationResponse>()
            };

            foreach (var bloodRegis in pagedBloodRegisRaw.Result.Items)
            {
                var member = await _repoUser.GetUserByIdAsync(bloodRegis.MemberId);
                if (member == null)
                    continue; // Skip if member not found

                var bloodType = await _repoBloodType.GetBloodTypeByIdAsync(member.BloodTypeId);

                pagedBloodRegis.Items.Add(new BloodRegistrationResponse()
                {
                    Id = bloodRegis.Id,
                    MemberName = member.LastName + " " + member.FirstName,
                    Phone = member.Phone,
                    Dob = member.Dob,
                    Type = bloodType?.Type
                });
            }

            return pagedBloodRegis;
        }

        public async Task<int> GetBloodRegistrationExpiredAsync()
        {
            return await _repository.BloodRegistrationExpiredAsync();
        }

        public async Task<PaginatedResultWithEventTime<BloodRegistrationResponse>?> SearchBloodRegistrationsByPhoneOrName(int pageNumber, int pageSize, string keyword, int? eventId = null)
        {
            if(string.IsNullOrEmpty(keyword))
            {
                return null; // Return null if keyword is empty or null
            }

            var pagedBloodRegisRaw = await _repository.SearchBloodRegistration(pageNumber, pageSize, keyword, eventId);

            var eventTime = pagedBloodRegisRaw.FirstOrDefault()?.Event?.EventTime;

            var dto = pagedBloodRegisRaw.Select(br => new BloodRegistrationResponse
            {
                Id = br.Id,
                MemberName = br.Member.LastName + " " + br.Member.FirstName,
                Phone = br.Member.Phone,
                Type = br.Member.BloodType.Type,
            }).ToList();

            var totalItems = await _repository.CountAsync(br =>
                                               (br.Member.FirstName.Contains(keyword)
                                               || br.Member.LastName.Contains(keyword)
                                               || br.Member.Phone.Contains(keyword))
                                               && br.IsApproved == null);

            return new PaginatedResultWithEventTime<BloodRegistrationResponse>
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