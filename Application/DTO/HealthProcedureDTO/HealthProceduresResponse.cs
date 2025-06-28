namespace Application.DTO.HealthProcedureDTO
{
    public class HealthProceduresResponse
    {
        public int Id { get; set; }
        public bool IsHealth { get; set; }
        public DateTime PerformedAt { get; set; }
        public string FullName { get; set; }
        public string BloodTypeName { get; set; }
    }
}
