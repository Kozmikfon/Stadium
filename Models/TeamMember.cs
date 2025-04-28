using System.ComponentModel.DataAnnotations.Schema;

namespace Stadyum.API.Models
{
    public class TeamMember
    {
        public int Id { get; set; }

        [ForeignKey("Player")]
        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        [ForeignKey("Team")]
        public int TeamId { get; set; }
        public Team? Team { get; set; }
    }
}
