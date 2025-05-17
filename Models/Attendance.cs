using System.ComponentModel.DataAnnotations.Schema;

namespace Stadyum.API.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        [ForeignKey("Player")]
        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        [ForeignKey("Match")]
        public int MatchId { get; set; }
        public Match? Match { get; set; }

        public bool IsPresent { get; set; }
        public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    }
}
