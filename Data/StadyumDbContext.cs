using Microsoft.EntityFrameworkCore;
using Stadyum.API.Models;


namespace Stadyum.API.Data
{
    public class StadyumDbContext:DbContext
    {
        public StadyumDbContext(DbContextOptions<StadyumDbContext> options) : base(options)
        {}
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Offer> Offers { get; set; }

    }

    
}
