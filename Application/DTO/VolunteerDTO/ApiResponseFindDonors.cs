namespace Application.DTO.VolunteerDTO
{
    public class ApiResponseFindDonors : ApiResponse<List<VolunteerFindDonorsResponse>>
    {
        public int SucceededVolunteersCount { get; set; } = 0;
        public int FailedVolunteersCount { get; set; } = 0;
    }
}
