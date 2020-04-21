using System;
using DevelopmentCardsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DevelopmentCardsApi.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options)
            : base(options)
        {
        }
        public DbSet<DevelopmentCard> DevelopmentCards { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<GameCard> GameCards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=DevelopmentCardsApi.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DevelopmentCard>().ToTable("DevelopmentCards");
            modelBuilder.Entity<Game>().ToTable("Games");
            modelBuilder.Entity<Player>().ToTable("Players");
            modelBuilder.Entity<GameCard>().ToTable("GameCards");
        }
    }
}
