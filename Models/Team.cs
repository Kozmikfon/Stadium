using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stadyum.API.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        // Captain ilişkisi
        public int? CaptainId { get; set; }

        [ForeignKey("CaptainId")]
        public Player? Captain { get; set; }

        // Oyuncular listesi
        public ICollection<Player> Players { get; set; } = new List<Player>();

        public bool IsInTournament { get; set; } = false;

    }
}
