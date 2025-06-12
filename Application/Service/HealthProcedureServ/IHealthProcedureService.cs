using Application.DTO.HealthProcedureDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.HealthProcedureServ
{
    public interface IHealthProcedureService
    {
        Task<HealthProcedure?> RecordHealthProcedureAsync(HealthProcedureRequest request);
    }
}
