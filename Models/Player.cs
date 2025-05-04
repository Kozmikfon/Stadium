using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stadyum.API.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty;

        [Range(1, 100)]
        public int SkillLevel { get; set; }

        [Range(0, 5)]
        public float Rating { get; set; }

        public DateTime CreateAd { get; set; } = DateTime.UtcNow;

        // Takım ilişkisi
        public int? TeamId { get; set; }

        [ForeignKey("TeamId")]
        public Team? Team { get; set; }
        public int UserId { get; internal set; }
    }
}
