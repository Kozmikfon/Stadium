﻿using Microsoft.EntityFrameworkCore;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>().ToTable("Matches");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Team>().ToTable("Teams");
            modelBuilder.Entity<Offer>().ToTable("Offers");
            modelBuilder.Entity<Review>().ToTable("Reviews");
            modelBuilder.Entity<TeamMember>().ToTable("TeamMembers");
        }

    }

}
