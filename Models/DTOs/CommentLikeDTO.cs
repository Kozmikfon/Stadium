namespace Stadyum.API.Models.DTOs
{
    public class CommentLikeDTO
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int PlayerId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
