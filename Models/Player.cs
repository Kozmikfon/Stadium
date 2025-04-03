using System.ComponentModel.DataAnnotations;

namespace Stadyum.API.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Position { get; set; }

        [Range(1, 100)]
        public int SkillLevel { get; set; }

        [Range(1, 5)]
        public float Rating { get; set; }

        public DateTime CreateAd { get; set; } = DateTime.UtcNow;

        public int? TeamId { get; set; } // Foreign key
        public Team? Team { get; set; }  // Navigation property
    }
}
