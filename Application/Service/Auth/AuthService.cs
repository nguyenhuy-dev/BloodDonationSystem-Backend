using Application.DTO.LoginDTO;
using Infrastructure.Repository.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Auth
{
    public class AuthService(IAuthRepository _repository) : IAuthService
    {
        public async Task<LoginResponse> LoginAsync(string phone, string password)
        {
            var user = await _repository.LoginAsync(phone, password);
            if (user == null)
            {
                return new LoginResponse // Invalid login response
                {
                    IsSuccess = false,
                    Message = "Invalid phone or password."
                };
            }
            return new LoginResponse // Successful login response
            {
                IsSuccess = true,
                Message = "Login successful.",
                //Token = null, // Cai nay de sau
                Phone = user.Phone,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
        }
    }
}
