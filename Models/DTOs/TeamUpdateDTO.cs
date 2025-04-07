
namespace Stadyum.API.Models.DTOs
{
    public class TeamUpdateDTO
    {
        public string? Name { get; set; }   // Takım adı güncellenebilir
        public int? CaptainId { get; set; } // Kaptan atanabilir veya değiştirilebilir
    }
}
