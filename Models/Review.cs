namespace Stadyum.API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public int ReviewerId { get; set; }
        public int? ReviewedUserId { get; set; }
        public int? ReviewedTeamId { get; set; }
        public required string Comment { get; set; }
        public int Rating { get; set; }

    }
}
