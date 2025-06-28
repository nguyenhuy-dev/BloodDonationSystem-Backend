using Infrastructure.Helper;

namespace Application.DTO.BloodProcedureDTO
{
    public class PaginatedResultBloodProce : PaginatedResult<BloodCollectionResponse>
    {
        public DateOnly EventTime { get; set; }
    }
}
