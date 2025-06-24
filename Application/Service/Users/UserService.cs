using Application.DTO.UserDTO;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Helper;
using Infrastructure.Repository.Auth;
using Infrastructure.Repository.Blood;
using Infrastructure.Repository.Users;
using Microsoft.AspNetCore.Http;

namespace Application.Service.Users
{
    public class UserService(IUserRepository _userRepository,
                             IHttpContextAccessor _contextAccessor,
                             IBloodTypeRepository _bloodRepository) : IUserService
    {
        public async Task<User> AssignUserRole(Guid userId, int roleId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            user.RoleId = roleId;

            var assignedRole = await _userRepository.AssignUserRole(user);
            return assignedRole;
        }

        public async Task<bool> BanUserAsync(Guid userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user.RoleId == 1)
            {
                return false;
            }

            var banUser = await _userRepository.BanUserAsync(userId);
            if (banUser <= 0)
            {
                // Log or handle the case where no user was banned
                return false;
            }
            return banUser > 0;
        }

        public async Task<bool> DeactiveUserAsync(Guid userId)
        {
            var user = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (user == null || !Guid.TryParse(user, out Guid parsedUserId) || parsedUserId != userId)
            {
                // Log or handle the case where the user ID is invalid or does not match
                return false; // Unauthorized access or invalid user ID
            }
            var deactiveUser = await _userRepository.DeactiveUserAsync(userId);
            if (deactiveUser <= 0)
            {
                // Log or handle the case where no user was deactivated
                return false;
            }

            return deactiveUser > 0;
        }

        public async Task<PaginatedResult<ListUserDTO>> GetAllUserAsync(int pageNumber, int pageSize)
        {
            var totalItems = await _userRepository.CountAllAsync();
            var users = await _userRepository.GetAllUserAsync(pageNumber, pageSize);

            var userDtos = users.Select(u => new ListUserDTO
            {
                Name = $"{u.LastName} {u.FirstName}",
                Email = u.Gmail,
                Status = u.Status.ToString(),
                Dob = u.Dob,
                Role = u.Role.RoleName
            }).ToList();

            return new PaginatedResult<ListUserDTO>
            {
                Items = userDtos,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<ProfileDTO?> GetUserByIdAsync(Guid userId)
        {
            var id = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (id == null || !Guid.TryParse(id, out Guid parsedUserId) || parsedUserId != userId)
            {
                // Log or handle the case where the user ID is invalid or does not match
                return null; // Unauthorized access or invalid user ID
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                // Log or handle the case where the user was not found
                return null; // User not found
            }

            var bloodType = await _bloodRepository.GetBloodTypeByIdAsync(user.BloodTypeId);

            return new ProfileDTO
            {
                Name = $"{user.LastName} {user.FirstName}",
                Phone = user.Phone,
                Gmail = user.Gmail,
                Gender = user.Gender,
                Dob = user.Dob,
                BloodType = bloodType.Type
            };
        }

        public async Task<ProfileDTO> UpdateUserProfileAsync(Guid userId, UserDTO updateUser)
        {
            var id = _contextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (id == null || !Guid.TryParse(id, out Guid parsedUserId) || parsedUserId != userId)
            {
                // Log or handle the case where the user ID is invalid or does not match
                return null; // Unauthorized access or invalid user ID
            }

            var existingUser = await _userRepository.GetUserByIdAsync(userId);
            //var bloodType = await _bloodRepository.GetBloodTypeByNameAsync(updateUser.BloodTypeId);

            existingUser.FirstName = updateUser.FirstName;
            existingUser.LastName = updateUser.LastName;
            existingUser.Phone = updateUser.Phone;
            existingUser.Gmail = updateUser.Gmail;
            existingUser.Gender = updateUser.Gender;
            existingUser.Dob = updateUser.Dob;
            existingUser.BloodTypeId = updateUser.BloodTypeId;

            var bloodType = await _bloodRepository.GetBloodTypeByIdAsync(updateUser.BloodTypeId);

            var updatedUser = await _userRepository.UpdateUserProfileAsync(existingUser);

            return new ProfileDTO
            {
                Name = $"{existingUser.LastName} {existingUser.FirstName}",
                Phone = existingUser.Phone,
                Gmail = existingUser.Gmail,
                Gender = existingUser.Gender,
                Dob = existingUser.Dob,
                BloodType = bloodType.Type
            };
        }
    }
}