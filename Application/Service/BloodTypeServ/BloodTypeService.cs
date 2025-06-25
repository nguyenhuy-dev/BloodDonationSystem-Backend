using Application.DTO.BloodTypeDTO;
using Domain.Entities;
using Infrastructure.Repository.Blood;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.BloodTypeServ
{
    public class BloodTypeService(IBloodTypeRepository _bloodTypeRepository) : IBloodTypeService
    {
        public async Task<List<BloodTypeDTO>> GetAllBloodTypesAsync()
        {
            var bloodType = await _bloodTypeRepository.GetAllBloodTypeAsync();
            var bloodTypeDTO = bloodType.Select(bt => new BloodTypeDTO
            {
                Id = bt.Id,
                Type = bt.Type,
            }).ToList();

            return bloodTypeDTO;
        }
    }
}
