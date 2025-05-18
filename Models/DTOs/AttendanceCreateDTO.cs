namespace Stadyum.API.Models.DTOs
{
    public class AttendanceCreateDTO
    {
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public bool IsPresent { get; set; }
    }
}
