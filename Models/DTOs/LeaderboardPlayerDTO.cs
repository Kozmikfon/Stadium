namespace Stadyum.API.Models.DTOs
{
    public class LeaderboardPlayerDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int MatchCount { get; set; }
    }
}
