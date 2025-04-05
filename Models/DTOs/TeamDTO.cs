namespace Stadyum.API.Models.DTOs
{
    public class TeamDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int CaptainId { get; set; } // KaptanId zorunlu

        public PlayerDTO? Captain { get; set; }
        public List<PlayerDTO>? Players { get; set; }
    }

    // Detay içermeyen sadeleştirilmiş oyuncu gösterimi
    public class PlayerBasicDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }

    public class TeamBasicDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
