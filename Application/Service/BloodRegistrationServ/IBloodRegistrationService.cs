using Application.DTO.BloodRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BloodRegistrationServ
{
    public interface IBloodRegistrationService
    {
        Task<int> RegisterDonation(BloodRegistrationRequest request);
    }
}
