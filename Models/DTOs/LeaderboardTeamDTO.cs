namespace Stadyum.API.Models.DTOs
{
    public class LeaderboardTeamDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MatchCount { get; set; }
    }
}
