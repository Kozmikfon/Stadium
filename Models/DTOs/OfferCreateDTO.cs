namespace Stadyum.API.Models.DTOs
{
    public class OfferCreateDTO
    {
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public int MatchId { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
