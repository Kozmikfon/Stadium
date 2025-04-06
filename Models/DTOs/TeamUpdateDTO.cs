
namespace Stadyum.API.Models.DTOs
{
    public class TeamUpdateDTO
    {
        public string Name { get; set; } = null!;
        public int? CaptainId { get; set; }  // ❗ Nullable, yani kaptan olmayabilir
    }
}
