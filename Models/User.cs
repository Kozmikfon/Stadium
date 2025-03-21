﻿namespace Stadyum.API.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Position { get; set; }
        public int SkillLevel { get; set; }
        public float Rating { get; set; }


    }
}
