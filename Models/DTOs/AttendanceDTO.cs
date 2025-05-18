namespace Stadyum.API.Models.DTOs
{
    public class AttendanceDTO
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public bool IsPresent { get; set; }
        public DateTime CheckedAt { get; set; }
    }
}
