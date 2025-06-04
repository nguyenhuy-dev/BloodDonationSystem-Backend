using Application.DTO;
using Application.DTO.LoginDTO;
using Domain.Entities;

namespace Application.Service.Auth
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(string phone, string password);
        Task<User?> RegisterAsync(UserDTO userDTO);
    }
}
