using Application.DTO.UserDTO;
using Domain.Entities;
using Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Users
{
    public interface IUserService
    {
        Task<ProfileDTO?> GetUserByIdAsync(Guid userId);
        Task<PaginatedResult<ListUserDTO>> GetAllUserAsync(int pageNumber, int pageSize);

        //Task<User> AssignUserRole(Guid userId, int roleId);
        Task<ProfileDTO> UpdateUserProfileAsync(Guid userId, UserDTO updateUser);
        Task<bool> DeactiveUserAsync();
        Task<bool> BanUserAsync(Guid userId);

        Task<UpdateUserDTO> UpdateUserAsync(Guid userId, UpdateUserDTO update);
        Task<UserDTO> AddStaffAsync (UserDTO request);
    }
}
