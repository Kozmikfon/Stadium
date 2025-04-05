namespace Stadyum.API.Models.DTOs
{
    public class TeamDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int CaptainId { get; set; }
        public PlayerBasicDTO? Captain { get; set; }

        public List<PlayerBasicDTO> Players { get; set; } = new();
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
