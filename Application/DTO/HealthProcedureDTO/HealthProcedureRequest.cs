namespace Application.DTO.HealthProcedureDTO
{
    public class HealthProcedureRequest
    {
        public int Systolic { get; set; }
        public int Diastolic { get; set; }
        public float Temperature { get; set; }
        public float Hb { get; set; }
        public bool HBV { get; set; }
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public bool IsHealth { get; set; }
        public string? Description { get; set; }
    }
}
