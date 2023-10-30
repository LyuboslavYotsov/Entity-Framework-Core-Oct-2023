using Microsoft.EntityFrameworkCore;
using P02_FootballBetting.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {

        }

        public FootballBettingContext(DbContextOptions<FootballBettingContext> options)
            : base(options)
        {

        }
        public DbSet<Team> Teams { get; set; } = null!;

        public DbSet<Color> Colors { get; set; } = null!;

        public DbSet<Town> Towns { get; set; } = null!;

        public DbSet<Country> Countries { get; set; } = null!;

        public DbSet<Player> Players { get; set; } = null!;

        public DbSet<Position> Positions { get; set; } = null!;

        public DbSet<PlayerStatistic> PlayersStatistics { get; set; } = null!;

        public DbSet<Game> Games { get; set; } = null!;

        public DbSet<Bet> Bets { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerStatistic>(entity =>
            {
                entity.HasKey(pk => new { pk.GameId, pk.PlayerId });

            });
            //MigrationProblem Solutions
            //modelBuilder.Entity<Team>(entity =>
            //{
            //    entity
            //        .HasOne(t => t.PrimaryKitColor)
            //        .WithMany(c => c.PrimaryKitTeams)
            //        .HasForeignKey(t => t.PrimaryKitColorId)
            //        .OnDelete(DeleteBehavior.NoAction);

            //    entity
            //        .HasOne(t => t.SecondaryKitColor)
            //        .WithMany(c => c.SecondaryKitTeams)
            //        .HasForeignKey(t => t.SecondaryKitColorId)
            //        .OnDelete(DeleteBehavior.NoAction);
            //});

            //modelBuilder.Entity<Game>(entity =>
            //{
            //    entity
            //        .HasOne(g => g.HomeTeam)
            //        .WithMany(t => t.HomeGames)
            //        .HasForeignKey(g => g.HomeTeamId)
            //        .OnDelete(DeleteBehavior.NoAction);

            //    entity
            //        .HasOne(g => g.AwayTeam)
            //        .WithMany(t => t.AwayGames)
            //        .HasForeignKey(g => g.AwayTeamId)
            //        .OnDelete(DeleteBehavior.NoAction);
            //});

            //modelBuilder.Entity<Player>(entity =>
            //{
            //    entity
            //        .HasOne(g => g.Town)
            //        .WithMany(t => t.Players)
            //        .HasForeignKey(g => g.TownId)
            //        .OnDelete(DeleteBehavior.NoAction);
            //});
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-V8T5092\\SQLEXPRESS;Database=FootballBookmakerSystem;Integrated Security=True; TrustServerCertificate=True");
            }
        }
    }
}
