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

        Task<bool> UserExistsAsync(string phone);
        Task<User?> GetUserByPhoneAsync(string phone);

        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
        Task<RefreshToken> GetRefreshTokenAsync(string refreshToken);
    }
}
