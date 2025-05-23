﻿namespace Stadyum.API.Models.DTOs
{
    public class PlayerCreateDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public int SkillLevel { get; set; }
        public float Rating { get; set; }
        public DateTime CreateAd { get; set; }
        public int? TeamId { get; set; } // Opsiyonel, takım olmayabilir
        public int UserId { get; set; }
    }
}
