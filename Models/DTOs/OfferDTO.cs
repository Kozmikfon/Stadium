namespace Stadyum.API.Models.DTOs
{
    public class OfferDTO
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int? ReceiverId { get; set; }
        public int MatchId { get; set; }
        public string Status { get; set; } = string.Empty;

        // name
        public string? ReceiverName { get; set; }
        // 👇 Eklenecek bilgiler
        public string? FieldName { get; set; }
        public string? CaptainName { get; set; }
        public DateTime MatchDate { get; set; }
    }
}
