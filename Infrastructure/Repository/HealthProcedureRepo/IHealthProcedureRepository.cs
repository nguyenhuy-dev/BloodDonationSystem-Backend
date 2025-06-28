using Domain.Entities;
using Infrastructure.Helper;
using Infrastructure.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.HealthProcedureRepo
{
    public interface IHealthProcedureRepository : IGenericRepository<HealthProcedure>
    {
        Task<PaginatedResult<HealthProcedure>> GetHealthProceduresByPaged(int id, int pageNumber, int pageSize);
    }
}
