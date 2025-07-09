using Application.DTO;
using Application.DTO.VolunteerDTO;
using Application.Service.EmailServ;
using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Blood;
using Infrastructure.Repository.BloodRegistrationRepo;
using Infrastructure.Repository.Events;
using Infrastructure.Repository.Facilities;
using Infrastructure.Repository.Users;
using Infrastructure.Repository.VolunteerRepo;
using Microsoft.AspNetCore.Http;

namespace Application.Service.VolunteerServ
{
    public class VolunteerService(IVolunteerRepository _repoVolun, IHttpContextAccessor _contextAccessor,
        IUserRepository _repoUser, IBloodTypeRepository _repoBloodType,
        IEventRepository _repoEvent, IBloodRegistrationRepository _repoRegis,
        IEmailService _repoMail, IFacilityRepository _repoFacility) : IVolunteerService
    {
        public async Task<ApiResponse<Volunteer>?> RegisterVolunteerDonation(RegisterVolunteerDonation request)
        {
            ApiResponse<Volunteer> apiResponse = new();

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
                throw new UnauthorizedAccessException("User not found or invalid");
            var user = await _repoUser.GetUserByIdAsync(creatorId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found or invalid");

            //// Nếu đã đăng ký tình nguyện thì không được đăng ký nữa
            //var checkedVolunteer = await _repoVolun.GetVolunteerByMemberIdAsync(creatorId);
            //if (checkedVolunteer != null)
            //{
            //    apiResponse.IsSuccess = false;
            //    apiResponse.Message = "Having a registered volunteer.";
            //    return apiResponse;
            //}

            // Kiểm tra đã hiến máu ở hệ thống lần nào chưa
            bool changedLastDonation = false;
            if (user.LastDonation == null)
            {
                user.LastDonation = request.LastDonation;
                changedLastDonation = true;
            }

            // Kiểm tra lần cuối hiến máu có phù hợp
            if (user.LastDonation >= DateTime.Now.AddDays(-90))
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Last donation not suitable.";
                return apiResponse;
            }
            if (changedLastDonation == true)
                await _repoUser.UpdateUserProfileAsync(user);

            var volunteer = new Volunteer
            {
                CreateAt = DateTime.Now,
                StartVolunteerDate = request.StartVolunteerDate,
                EndVolunteerDate = request.EndVolunteerDate,
                IsExpired = false,
                MemberId = creatorId
            };
            await _repoVolun.AddAsync(volunteer);

            apiResponse.IsSuccess = true;
            apiResponse.Message = "Register volunteer successfully";
            return apiResponse;
        }

        public async Task<Volunteer?> UpdateVolunteerDonation(int id, UpdateVolunteerDonation request)
        {
            // Kiểm tra đơn tình nguyện có tồn tại
            var existingVolunteer = await _repoVolun.GetByIdAsync(id);
            if (existingVolunteer == null || existingVolunteer.IsExpired == true)
                return null;

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
                throw new UnauthorizedAccessException("User not found or invalid");

            // Kiểm tra xem có là thành viên sở hữu đơn tình nguyện
            var user = await _repoUser.GetUserByIdAsync(creatorId);
            if (user != null && user.Id != existingVolunteer.MemberId)
                return null;

            if (request.EndVolunteerDate < DateTime.Now)
                existingVolunteer.IsExpired = true;

            existingVolunteer.StartVolunteerDate = request.StartVolunteerDate;
            existingVolunteer.EndVolunteerDate = request.EndVolunteerDate;
            existingVolunteer.UpdateAt = DateTime.Now;
            await _repoVolun.UpdateAsync(existingVolunteer);

            return existingVolunteer;
        }

        public async Task<PaginatedResult<VolunteersResponse>?> GetVolunteersByPaged(int pageNumber, int pageSize)
        {
            var pagedVolunteerRaw = await _repoVolun.GetPagedAsync(pageNumber, pageSize);
            var facility = await _repoFacility.GetByIdAsync(1);  // hard code cho cơ sở y tế
            if (facility == null)
                throw new ArgumentNullException("Facility not existed");

            var pagedVolunteer = new PaginatedResult<VolunteersResponse>
            {
                Items = new List<VolunteersResponse>(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = pagedVolunteerRaw.TotalItems
            };

            foreach (var volunteer in pagedVolunteerRaw.Items)
            {
                var member = await _repoUser.GetUserByIdAsync(volunteer.MemberId);
                if (member == null)
                    throw new ArgumentNullException("Member cannot be null");

                var bloodType = await _repoBloodType.GetBloodTypeByIdAsync(member.BloodTypeId);

                pagedVolunteer.Items.Add(new VolunteersResponse
                {
                    Id = volunteer.Id,
                    BloodTypeName = bloodType?.Type,
                    Distance = Math.Round((decimal)GeographyHelper.CalculateDistanceKm(facility.Latitude, facility.Longitude, member.Latitude, member.Longitude), 1),
                    StartVolunteerDate = volunteer.StartVolunteerDate,
                    EndVolunteerDate = volunteer.EndVolunteerDate,
                    FullName = member.LastName + " " + member.FirstName,
                    Phone = member?.Phone,
                    Gmail = member?.Gmail
                });
            }

            return pagedVolunteer;
        }

        public async Task<ApiResponse<Volunteer>?> AddDonationRegistrationWithVolunteer(int eventId, int id)
        {
            ApiResponse<Volunteer> apiResponse = new();

            // Kiểm tra xem event tồn tại, hay đã hết hạn, hay có phải là urgent hay không
            var existingEvent = await _repoEvent.GetEventByIdAsync(eventId);
            if (existingEvent == null || 
                existingEvent.IsExpired == true || 
                existingEvent.IsUrgent == false)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Event not found or be expired or not urgent";
                return apiResponse;
            }

            // Kiểm tra event đã đạt tối đa số người hiến máu
            var bloodRegistrations = await _repoRegis.GetByEventAsync(eventId);
            if (existingEvent.MaxOfDonor <= bloodRegistrations.Count())
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Event reached max of donor";
                return apiResponse;
            }

            // Kiểm tra volunteer có tồn tại hay không, hoặc đã quá hạn
            var existingVolunteer = await _repoVolun.GetByIdAsync(id);
            if (existingVolunteer == null || existingVolunteer.IsExpired == true)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Volunteer not existed or be expired";
                return apiResponse;
            }

            // Kiểm tra khi add thì StartDate & EndDate đã phù hợp hay chưa
            if (DateOnly.FromDateTime(existingVolunteer.StartVolunteerDate) > existingEvent.EventTime ||
                DateOnly.FromDateTime(existingVolunteer.EndVolunteerDate) < existingEvent.EventTime)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Volunteer not ready to donate.";
                return apiResponse;
            }

            // Kiểm tra volunteer đã được add hay chưa
            var checkedVolunteer = bloodRegistrations.FirstOrDefault(br => br.VolunteerId == id);
            if (checkedVolunteer != null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Volunteer was existed in event";
                return apiResponse;
            }

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
                throw new UnauthorizedAccessException("User not found or invalid");

            var bloodRegis = new BloodRegistration
            {
                IsApproved = true,
                CreateAt = DateTime.Now,
                VolunteerId = id,
                MemberId = existingVolunteer.MemberId,
                StaffId = creatorId,
                EventId = eventId
            };
            await _repoRegis.AddAsync(bloodRegis);

            existingVolunteer.IsExpired = true;
            await _repoVolun.UpdateAsync(existingVolunteer);

            await _repoMail.SendEmailFindDonorsAsync(bloodRegis);

            apiResponse.IsSuccess = true;
            apiResponse.Message = "Find donor(s) successfully";
            return apiResponse;
        }

        public async Task<int> VolunteerEndDateExpiredAsync()
        {
            return await _repoVolun.EndVolunteerDateExpired();
        }
    }
}
