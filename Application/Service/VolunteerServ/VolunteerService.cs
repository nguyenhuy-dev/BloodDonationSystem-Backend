using Application.DTO;
using Application.DTO.VolunteerDTO;
using Application.Service.EmailServ;
using Application.Service.Events;
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
        IUserRepository _repoUser, IBloodTypeRepository _repoBloodType, IBloodRegistrationRepository _repoRegis,
        IEmailService _servMail, IFacilityRepository _repoFacility, 
        IEventService _servEvent) : IVolunteerService
    {
        public async Task<ApiResponse<Volunteer>?> RegisterVolunteerDonationAsync(RegisterVolunteerDonation request)
        {
            ApiResponse<Volunteer> apiResponse = new();

            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
                throw new UnauthorizedAccessException("User not found or invalid");
            var user = await _repoUser.GetUserByIdAsync(creatorId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found or invalid");

            // Khi có tồn tại đơn tình nguyện mà chưa hết hạn thì không cho đăng ký mới
            var checkedVolunteerList = await _repoVolun.GetVolunteerByMemberIdAsync(creatorId);
            var checkedVolunteer = checkedVolunteerList?.FirstOrDefault(v => v.IsExpired == false);
            if (checkedVolunteer != null)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Having a registered volunteer.";
                return apiResponse;
            }

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

        public async Task<Volunteer?> UpdateVolunteerDonationAsync(int id, UpdateVolunteerDonation request)
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

        public async Task<PaginatedResult<VolunteersResponse>?> GetVolunteersByPagedAsync(int facilityId, int pageNumber, int pageSize)
        {
            var pagedVolunteerRaw = await _repoVolun.GetPagedAsync(pageNumber, pageSize);
            var facility = await _repoFacility.GetByIdAsync(facilityId);
            if (facility == null)
                return null;

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
                    throw new ArgumentNullException("Member cannot be null.");

                var bloodType = await _repoBloodType.GetBloodTypeByIdAsync(member.BloodTypeId);

                pagedVolunteer.Items.Add(new VolunteersResponse
                {
                    Id = volunteer.Id,
                    BloodTypeName = bloodType?.Type,
                    Distance = Math.Round((decimal)GeographyHelper.CalculateDistanceKm(facility.Latitude, facility.Longitude, member.Latitude, member.Longitude), 1),
                    Latitude = member.Latitude,
                    Longitude = member.Longitude, 
                    StartVolunteerDate = volunteer.StartVolunteerDate,
                    EndVolunteerDate = volunteer.EndVolunteerDate,
                    FullName = member.LastName + " " + member.FirstName,
                    Phone = member?.Phone,
                    Gmail = member?.Gmail
                });
            }

            return pagedVolunteer;
        }

        public async Task<ApiResponseFindDonors> AddDonationRegistrationWithVolunteersAsync(UrgentEventVolunteer urgentEventVolunteer)
        {
            ApiResponseFindDonors apiResponseFind = new();

            // Kiểm tra xem có volunteers nào tồn tại không
            // Nâng cấp: Unit of Work để kiểm tra có ít nhất 1 volunteer phù hợp
            if (!urgentEventVolunteer.VolunteerIds.Any())
            {
                apiResponseFind.IsSuccess = false;
                apiResponseFind.Message = "No volunteers at all.";
                return apiResponseFind;
            }

            // Kiểm tra xem có chọn số Volunteer hợp lệ với MaxDonor hay không
            if (urgentEventVolunteer.MaxOfDonor < urgentEventVolunteer.VolunteerIds.Count())
            {
                apiResponseFind.IsSuccess = false;
                apiResponseFind.Message = "Already reached max of donor.";
                return apiResponseFind;
            }

            apiResponseFind.Data = new();
            foreach (var volunteerId in urgentEventVolunteer.VolunteerIds)
            {
                var apiResponseAdd = await AddDonationRegistrationWithVolunteerAsync(urgentEventVolunteer, volunteerId);

                // Volunteer không hợp lệ với nhiều lí do ở hàm private
                if (apiResponseAdd.IsSuccess == false)
                {
                    apiResponseFind.FailedVolunteersCount++;
                    apiResponseFind.Data.Add(new VolunteerFindDonorsResponse
                    {
                        Id = volunteerId,
                        IsSucceded = false,
                        Message = apiResponseAdd.Message
                    });
                    continue;
                }

                var urgentEvent = await _servEvent.AddUrgentEventAsync(urgentEventVolunteer);
                apiResponseFind.SucceededVolunteersCount++;
                apiResponseFind.Data.Add(new VolunteerFindDonorsResponse
                {
                    Id = volunteerId,
                    IsSucceded = true,
                    Message = apiResponseAdd.Message
                });
                apiResponseAdd.Data.BloodRegistration.EventId = urgentEvent.Id;
                await _repoRegis.AddAsync(apiResponseAdd.Data.BloodRegistration);
                await _repoVolun.UpdateAsync(apiResponseAdd.Data.Volunteer);
                await _servMail.SendEmailFindDonorsAsync(apiResponseAdd.Data.BloodRegistration);
            }

            if (apiResponseFind.SucceededVolunteersCount == 0)
            {
                apiResponseFind.IsSuccess = false;
                apiResponseFind.Message = "Find donor(s) unsuccessfully.";
                return apiResponseFind;
            }

            apiResponseFind.IsSuccess = true;
            apiResponseFind.Message = "Find donor(s) successfully.";
            return apiResponseFind;
        }

        private async Task<ApiResponse<VolunteerAddBloodRegis>> AddDonationRegistrationWithVolunteerAsync(UrgentEventVolunteer urgentEvent, int id)
        {
            ApiResponse<VolunteerAddBloodRegis> apiResponse = new();

            //// Kiểm tra xem event tồn tại, hay đã hết hạn, hay có phải là urgent hay không
            //var existingEvent = await _repoEvent.GetEventByIdAsync(eventId);
            //if (existingEvent == null ||
            //    existingEvent.IsExpired == true ||
            //    existingEvent.IsUrgent == false)
            //{
            //    apiResponse.IsSuccess = false;
            //    apiResponse.Message = "Event not found or be expired or not urgent.";
            //    return apiResponse;
            //}

            //// Kiểm tra event đã đạt tối đa số người hiến máu  (ở trên) ***
            //var bloodRegistrations = await _repoRegis.GetByEventAsync(eventId);
            //if (existingEvent.MaxOfDonor <= bloodRegistrations.Count())
            //{
            //    apiResponse.IsSuccess = false;
            //    apiResponse.Message = "Event reached max of donor.";
            //    return apiResponse;
            //}

            var existingEvent = urgentEvent;

            // Kiểm tra volunteer có tồn tại hay không, hoặc đã quá hạn
            var existingVolunteer = await _repoVolun.GetByIdAsync(id);
            if (existingVolunteer == null || existingVolunteer.IsExpired == true)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Volunteer not existed or be expired.";
                return apiResponse;
            }

            var member = await _repoUser.GetUserByIdAsync(existingVolunteer.MemberId);
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            // Kiểm tra blood type của volunteer có phù hợp với blood type của Event
            if (existingEvent.BloodTypeId != member.BloodTypeId)
            {
                apiResponse.IsSuccess = false;
                apiResponse.Message = "Not suitable blood type for urgent event.";
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

            //// Kiểm tra volunteer đã được add hay chưa
            //var checkedVolunteer = bloodRegistrations.FirstOrDefault(br => br.VolunteerId == id);
            //if (checkedVolunteer != null)
            //{
            //    apiResponse.IsSuccess = false;
            //    apiResponse.Message = "Volunteer was existed in event.";
            //    return apiResponse;
            //}

            var staffId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(staffId) || !Guid.TryParse(staffId, out Guid staffIdOut))
                throw new UnauthorizedAccessException("User not found or invalid.");

            var bloodRegis = new BloodRegistration
            {
                CreateAt = DateTime.Now,
                VolunteerId = id,
                MemberId = existingVolunteer.MemberId,
                StaffId = staffIdOut,
                //EventId = existingEvent.Id (ở trên) ***
            };
            //await _repoRegis.AddAsync(bloodRegis);

            existingVolunteer.IsExpired = true;
            existingVolunteer.UpdateAt = DateTime.Now;
            //await _repoVolun.UpdateAsync(existingVolunteer);

            //await _servMail.SendEmailFindDonorsAsync(bloodRegis);

            apiResponse.IsSuccess = true;
            apiResponse.Message = "Find donor(s) successfully.";
            apiResponse.Data = new VolunteerAddBloodRegis
            {
                BloodRegistration = bloodRegis,
                Volunteer = existingVolunteer,
            };
            return apiResponse;
        }

        public async Task<int> VolunteerEndDateExpiredAsync()
        {
            return await _repoVolun.EndVolunteerDateExpired();
        }
    }
}
