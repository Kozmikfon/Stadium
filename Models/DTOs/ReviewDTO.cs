namespace Stadyum.API.Models.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int ReviewerId { get; set; }
        public int? ReviewedUserId { get; set; }
        public int? ReviewedTeamId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
