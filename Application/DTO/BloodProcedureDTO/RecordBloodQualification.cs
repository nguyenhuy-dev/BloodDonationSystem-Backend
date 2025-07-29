using Domain.Enums;

namespace Application.DTO.BloodProcedureDTO
{
    public class RecordBloodQualification
    {
        public bool IsQualified { get; set; }
        public bool HIV { get; set; } = false;
        public bool HCV { get; set; } = false;
        public bool Syphilis { get; set; } = false;
        public float Hematocrit { get; set; }
        public int BloodTypeId { get; set; }
        public BloodComponent BloodComponent { get; set; }
    }
}
