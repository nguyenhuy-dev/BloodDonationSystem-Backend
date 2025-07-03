using Domain.Entities;
using Domain.Enums;

namespace Application.DTO.BloodInventoryDTO
{
    public class BloodInventoryResponse
    {
        public int BloodUnitId { get; set; }
        public DateTime CreateAt { get; set; }
        public string? BloodTypeName { get; set; }
        public string BloodComponentName { get; set; }
        public int BloodRegisId { get; set; }
        public int BloodAge { get; set; }
        public bool IsAvailable { get; set; }
        public string ExpiredDate { get; set; }
        public string Description { get; set; }
    }
}
