using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BloodProcedureRepo
{
    public class BloodProcedureRepository : GenericRepository<BloodProcedure>, IBloodProcedureRepository
    {
        public BloodProcedureRepository(BloodDonationSystemContext context) : base(context)
        {

        }
    }
}
