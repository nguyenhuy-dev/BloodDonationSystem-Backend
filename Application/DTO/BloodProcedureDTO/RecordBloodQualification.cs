using Domain.Enums;

namespace Application.DTO.BloodProcedureDTO
{
    public class RecordBloodQualification
    {
        public bool IsQualified { get; set; }
        public bool HIV { get; set; }
        public bool HCV { get; set; }
        public bool Syphilis { get; set; }
        public float Hematocrit { get; set; }
        public int BloodTypeId { get; set; }
        public BloodComponent BloodComponent { get; set; }
    }
}
