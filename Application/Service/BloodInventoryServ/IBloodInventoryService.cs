using Application.DTO;
using Application.DTO.BloodInventoryDTO;
using Domain.Entities;
using Infrastructure.Helper;

namespace Application.Service.BloodInventoryServ
{
    public interface IBloodInventoryService
    {
        Task<PaginatedResult<BloodInventoryResponse>> GetBloodUnitsByPagedAsync(int pageNumber, int pageSize);  
        Task<ApiResponse<BloodInventory>> DeleteABloodUnitAsync(int id);
    }
}
