namespace Stadyum.API.Models.DTOs
{
    public class TeamCompareDTO
    {
        public string TeamName { get; set; } = string.Empty;
        public int TotalMatches { get; set; }
        public double AverageRating { get; set; }
    }
}
