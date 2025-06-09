using Infrastructure.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Users
{
    public class UserService(IUserRepository _userRepository) : IUserService
    {
        public async Task<bool> DeactiveUserAsync(Guid userId)
        {
            var deactiveUser = await _userRepository.DeactiveUserAsync(userId);
            if (deactiveUser <= 0)
            {
                // Log or handle the case where no user was deactivated
                return false;
            }

            return deactiveUser > 0;
        }
    }
}
