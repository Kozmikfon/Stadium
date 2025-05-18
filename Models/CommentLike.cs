using System.ComponentModel.DataAnnotations.Schema;

namespace Stadyum.API.Models
{
    public class CommentLike
    {
        public int Id { get; set; }

        [ForeignKey("Player")]
        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        [ForeignKey("Review")]
        public int ReviewId { get; set; }  // ✅ Yorum yerine Review ID
        public Review? Review { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
