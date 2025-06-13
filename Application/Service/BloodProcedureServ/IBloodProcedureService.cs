using Application.DTO.BloodProcedureDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BloodProcedureServ
{
    public interface IBloodProcedureService
    {
        Task<BloodProcedure?> RecordBloodCollectionAsync(BloodCollectionRequest request);
    }
}
