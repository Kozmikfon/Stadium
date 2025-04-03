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
        public string Name { get; set; }

        [Required]
        public int CaptainId { get; set; }

        [ForeignKey("CaptainId")]
        public Player Captain { get; set; }

        public ICollection<Player> Players { get; set; } = new List<Player>();
    }
}
