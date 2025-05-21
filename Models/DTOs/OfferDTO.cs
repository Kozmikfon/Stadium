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
        public string? MatchTeamName { get; set; }   // Takım adı (örneğin Team1)
        public string? MatchFieldName { get; set; }  // Saha adı
        public string? MatchCaptainName { get; set; } // Kaptan adı
        public string? SenderName { get; set; }

    }
}
