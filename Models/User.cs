namespace Stadyum.API.Models
{
    public class User
    {
        public int Id { get; set; }

        public  required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Position { get; set; }
        public int SkillLevel { get; set; }
        public float Rating { get; set; }


    }
}
