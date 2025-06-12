using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.HealthProcedureRepo
{
    public class HealthProcedureRepository : GenericRepository<HealthProcedure>, IHealthProcedureRepository
    {
        public HealthProcedureRepository(BloodDonationSystemContext context) : base(context)
        {

        }
    }
}
