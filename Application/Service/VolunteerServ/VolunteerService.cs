using Application.DTO.VolunteerDTO;
using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Blood;
using Infrastructure.Repository.Users;
using Infrastructure.Repository.VolunteerRepo;
using Microsoft.AspNetCore.Http;

namespace Application.Service.VolunteerServ
{
    public class VolunteerService(IVolunteerRepository _repoVolun, IHttpContextAccessor _contextAccessor, 
        IUserRepository _repoUser, IBloodTypeRepository _repoBloodType) : IVolunteerService
    {
        public async Task<Volunteer?> RegisterVolunteerDonation(RegisterVolunteerDonation request)
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid creatorId))
                throw new UnauthorizedAccessException("User not found or invalid");

            if (request.LastDonation >= DateTime.Now.AddDays(-90))
                return null;

            var user = await _repoUser.GetUserByIdAsync(creatorId);
            if (user != null && user.LastDonation == null)
            {
                user.LastDonation = request.LastDonation;
                await _repoUser.UpdateUserProfileAsync(user);
            }

            var volunteer = new Volunteer
            {
                CreateAt = DateTime.Now,
                StartVolunteerDate = request.StartVolunteerDate,
                EndVolunteerDate = request.EndVolunteerDate,
                IsExpired = false,
                MemberId = creatorId
            };
            return await _repoVolun.AddAsync(volunteer);
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
                var bloodType = await _repoBloodType.GetBloodTypeByIdAsync(member.BloodTypeId);

                pagedVolunteer.Items.Add(new VolunteersResponse
                {
                    Id = volunteer.Id,
                    BloodTypeName = bloodType.Type,
                    Longitude = member.Longitude,
                    Latitude = member.Latitude,
                    FullName = member.LastName + " " + member.FirstName,
                    Phone = member.Phone,
                    Gmail = member.Gmail
                });
            }

            return pagedVolunteer;
        }

        public async Task<int> VolunteerEndDateExpiredAsync()
        {
            return await _repoVolun.EndVolunteerDateExpired();
        }
    }
}
