// MatchCreateDTO.cs
namespace Stadyum.API.Models.DTOs
{
    public class MatchCreateDTO
    {
        public int Team1Id { get; set; }
        public int Team2Id { get; set; }
        public string FieldName { get; set; } = null!;
        public DateTime MatchDate { get; set; }
    }
}
