namespace Stadyum.API.Models.DTOs
{
    public class PlayerDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Position { get; set; } = null!;
        public int SkillLevel { get; set; }
        public double Rating { get; set; }
        public DateTime CreateAd { get; set; }

        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
    }


    // İç içe detay göstermemek için sadeleştirilmiş Team

}
