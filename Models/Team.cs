namespace Stadyum.API.Models
{
    public class Team
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string CaptainId { get; set; }

    }
}
