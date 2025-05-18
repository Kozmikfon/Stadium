using Microsoft.EntityFrameworkCore;
using Stadyum.API.Models;

namespace Stadyum.API.Data
{
    public class StadyumDbContext : DbContext
    {
        public StadyumDbContext(DbContextOptions<StadyumDbContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Player> Players { get; set; }
        
        public DbSet<Like> Likes { get; set; } 
        public DbSet<Attendance> Attendances { get; set; } 
        public DbSet<MatchStat> MatchStats { get; set; } 
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>().ToTable("Matches");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Team>().ToTable("Teams");
            modelBuilder.Entity<Player>().ToTable("Players");
            modelBuilder.Entity<Offer>().ToTable("Offers");
            modelBuilder.Entity<Review>().ToTable("Reviews");
            modelBuilder.Entity<TeamMember>().ToTable("TeamMembers");
            modelBuilder.Entity<Like>().ToTable("Likes");
            modelBuilder.Entity<Attendance>().ToTable("Attendances");
            modelBuilder.Entity<MatchStat>().ToTable("MatchStats");
            modelBuilder.Entity<Notification>().ToTable("Notifications");
            modelBuilder.Entity<CommentLike>().ToTable("CommentLikes");

            // Player <-> Team ilişkisi
            modelBuilder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            // Team-Captain (Player) ilişkisi
            modelBuilder.Entity<Team>()
                .HasOne(t => t.Captain)
                .WithMany()
                .HasForeignKey(t => t.CaptainId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
             .HasOne(m => m.Team1)
                .WithMany()
                 .HasForeignKey(m => m.Team1Id)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.Team2)
                .WithMany()
                .HasForeignKey(m => m.Team2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamMember>()
             .HasOne(tm => tm.Player)
                .WithMany()
                .HasForeignKey(tm => tm.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany()
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Offer>()
             .HasOne(o => o.Sender)
            .WithMany()
            .HasForeignKey(o => o.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Receiver)
                .WithMany()
                .HasForeignKey(o => o.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Match)
                .WithMany()
                .HasForeignKey(o => o.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Like ilişkilendirmesi
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Player)
                .WithMany()
                .HasForeignKey(l => l.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Like>()
                .HasOne(l => l.Match)
                .WithMany()
                .HasForeignKey(l => l.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attendance ilişkilendirmesi
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Player)
                .WithMany()
                .HasForeignKey(a => a.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Match)
                .WithMany()
                .HasForeignKey(a => a.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // MatchStat ilişkilendirmesi
            modelBuilder.Entity<MatchStat>()
                .HasOne(ms => ms.Player)
                .WithMany()
                .HasForeignKey(ms => ms.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MatchStat>()
                .HasOne(ms => ms.Match)
                .WithMany()
                .HasForeignKey(ms => ms.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification ilişkilendirmesi
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Player)
                .WithMany()
                .HasForeignKey(n => n.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);





        }

    }
}
