namespace Stadyum.API.Models.DTOs
{
    public class NotificationCreateDTO
    {
        public int PlayerId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
