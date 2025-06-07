using Application.DTO;
using Application.DTO.LoginDTO;
using Domain.Entities;

namespace Application.Service.Auth
{
    public interface IAuthService
    {
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
        Task<LoginResponse> LoginAsync(string phone, string password);
        Task<User?> RegisterAsync(UserDTO userDTO);
        TokenModel GenerateToken(User user);
    }
}
