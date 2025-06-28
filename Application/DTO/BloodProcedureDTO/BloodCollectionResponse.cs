namespace Application.DTO.BloodProcedureDTO
{
    public class BloodCollectionResponse
    {
        public int Id { get; set; }
        public int DonationRegisId { get; set; }
        public float Volume { get; set; }
        public string FullName { get; set; }
        public string? BloodTypeName { get; set; }
        public DateTime PerformedAt { get; set; }
        public bool? IsQualified { get; set; }
    }
}
