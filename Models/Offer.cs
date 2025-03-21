namespace Stadyum.API.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int MatchId { get; set; }
        public string Status { get; set; }
    }
}
