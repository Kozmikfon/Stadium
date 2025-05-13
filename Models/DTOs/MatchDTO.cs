// MatchDTO.cs
namespace Stadyum.API.Models.DTOs
{
    public class MatchDTO
    {
        public int Id { get; set; }
        public int Team1Id { get; set; }
        public string Team1Name { get; set; } = string.Empty;

        public int Team2Id { get; set; }
        public string Team2Name { get; set; } = string.Empty;

        public string FieldName { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
       
        public int Team1CaptainId { get; set; }
        public int Team2CaptainId { get; set; }
    }
}
