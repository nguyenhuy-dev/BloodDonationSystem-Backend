using Application.DTO;
using Application.DTO.GoogleDTO;
using Application.DTO.LoginDTO;
using Application.DTO.Token;
using Domain.Entities;

namespace Application.Service.Auth
{
    public interface IAuthService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
        Task<LoginResponse> LoginAsync(string phone, string password);
        Task<User?> RegisterAsync(UserDTO userDTO);
        Task<User> RegisterWithGoogleAsync(User user);
        TokenModel GenerateToken(User user);

        Task<User> UpdateGoogleLoginAsync(UpdateGoogleLogin request);
        Task<RefreshToken> UpdateRefreshTokenAsync(RefreshToken refreshToken);
    }
}
