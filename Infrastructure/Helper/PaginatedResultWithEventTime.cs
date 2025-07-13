namespace Infrastructure.Helper
{
    public class PaginatedResultWithEventTime<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public DateOnly? EventTime { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
