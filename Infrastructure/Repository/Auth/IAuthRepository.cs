using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Auth
{
    public interface IAuthRepository
    {
        Task<User?> LoginAsync(string phone, string password);
        Task<User?> RegisterAsync(User user);

        Task<User> UpdateGoogleLogin(User user);

        Task<bool> UserExistsByPhoneAsync(string phone);
        Task<bool> UserExistsByEmailAsync(string email);
        Task<User?> GetUserByPhoneAsync(string phone);
        Task<User?> GetUserByEmailAsync(string email);

        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
        Task<RefreshToken> UpdateRefreshTokenAsync(RefreshToken refreshToken);
    }
}
