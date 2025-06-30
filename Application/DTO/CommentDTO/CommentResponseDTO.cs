namespace Application.DTO.CommentDTO
{
    public class CommentResponseDTO
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Member { get; set; }
    }
}