using System.ComponentModel.DataAnnotations.Schema;

namespace Stadyum.API.Models
{
    public class Offer
    {
        public int Id { get; set; }

        [ForeignKey("Sender")]
        public int SenderId { get; set; }
        public Player? Sender { get; set; }

        [ForeignKey("Receiver")]
        public int ReceiverId { get; set; }
        public Player? Receiver { get; set; }

        [ForeignKey("Match")]
        public int MatchId { get; set; }
        public Match? Match { get; set; }

        public string Status { get; set; } = "Pending"; // Varsayılan
    }
}
