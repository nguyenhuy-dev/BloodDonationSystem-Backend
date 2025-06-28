using Application.DTO.BloodRegistration;
using Domain.Entities;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.Users;
using Microsoft.AspNetCore.Http;
using Infrastructure.Helper;
using Application.DTO.BloodRegistrationDTO;

namespace Application.Service.BloodRegistrationServ
{
    public class BloodRegistrationService(IBloodRegistrationRepository _repository, IHttpContextAccessor _contextAccessor,
        IEventRepository _repoEvent, IUserRepository _repoUser) : IBloodRegistrationService
    {
        public async Task<BloodRegistration?> RegisterDonation(int id, BloodRegistrationRequest request)
        {
            // Kiểm tra event tương ứng với đơn đăng ký máu có tồn tại
            var existingEvent = await _repoEvent.GetEventByIdAsync(id);
            if (existingEvent == null)
                return null;

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }
            var user = await _repoUser.GetUserByIdAsync(creatorId);
            if (user == null)
                return null;

            // Kiểm tra xem lần cuối hiến máu có phù hợp
            if (request.LastDonation >= DateTime.Now.AddDays(-90))
                return null;  // Request xuống đều có LastDonation nên không cần xét trong hệ thống

            if (user.LastDonation == null)
                user.LastDonation = request.LastDonation;

            var registration = new BloodRegistration
            {
                CreateAt = DateTime.Now,
                MemberId = creatorId,
                EventId = id
            };
            await _repository.AddAsync(registration);
            return registration;
        }

        public async Task<BloodRegistration?> RejectBloodRegistration(int bloodRegisId)
        {
            var bloodRegistration = await _repository.GetByIdAsync(bloodRegisId);
            if (bloodRegistration == null || bloodRegistration.IsApproved == false)
                return null;

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            bloodRegistration.IsApproved = false;
            bloodRegistration.UpdateAt = DateTime.Now;
            bloodRegistration.StaffId = creatorId;
            await _repository.UpdateAsync(bloodRegistration);
            
            return bloodRegistration;
        }

        public async Task<BloodRegistration?> CancelOwnRegistration(int id)
        {
            // Check đơn có tồn tại, bị hủy, hay bị từ chối, hoặc đã khám hay chưa
            var bloodRegistration = await _repository.GetByIdAsync(id);
            if (bloodRegistration == null || bloodRegistration.IsApproved == false ||
                bloodRegistration.HealthId != null)
                return null;

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
            {
                throw new UnauthorizedAccessException("User not found or invalid");
            }

            // Xác thực người dùng hiện tại có đang là chủ của đơn đăng ký này hay không
            if (bloodRegistration.MemberId != creatorId)
                return null;

            bloodRegistration.IsApproved = false;
            bloodRegistration.UpdateAt = DateTime.Now;

            await _repository.UpdateAsync(bloodRegistration);
            return bloodRegistration;
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

                pagedBloodRegis.Items.Add(new BloodRegistrationResponse()
                {
                    Id = bloodRegis.Id,
                    MemberName = member.LastName + " " + member.FirstName,
                    Phone = member.Phone,
                    EventTime = eventExists.EventTime
                });
            }

            return pagedBloodRegis;
        }
    }
}