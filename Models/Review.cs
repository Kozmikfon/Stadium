using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stadyum.API.Models
{
    public class Review
    {
        public int Id { get; set; }

        [ForeignKey("Match")]
        public int MatchId { get; set; }
        public int ReviewerId { get; set; }
        public int? ReviewedUserId { get; set; }
        public int? ReviewedTeamId { get; set; }

        [Required]
        [MaxLength(500)]
        public required string Comment { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
        public Match Match { get; set; } = null!;

    }
}
