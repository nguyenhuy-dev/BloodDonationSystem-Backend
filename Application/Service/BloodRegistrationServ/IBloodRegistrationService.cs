using Application.DTO.BloodRegistration;
using Application.DTO.BloodRegistrationDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BloodRegistrationServ
{
    public interface IBloodRegistrationService
    {
        Task<BloodRegistration?> RegisterDonation(BloodRegistrationRequest request);
        Task<BloodRegistration?> EvaluateRegistration(int bloodRegisId, EvaluateBloodRegistration evaluate);
    }
}
