using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace movieGame.Models
{
    public class MovieGameContext : DbContext
    {
        public MovieGameContext(DbContextOptions<MovieGameContext> options) : base(options) { }
        public DbSet<Clue> Clues                  { get; set; }
        public DbSet<Game> Games                  { get; set; }
        public DbSet<Movie> Movies                { get; set; }
        public DbSet<Team> Teams                  { get; set; }
        public DbSet<User> Users                  { get; set; }
        public DbSet<Hint> Hints                 { get; set; }
        public DbSet<GameTeamJoin> GameTeamJoin   { get; set; }
        public DbSet<MovieUserJoin> MovieUserJoin { get; set; }
        public DbSet<MovieTeamJoin> MovieTeamJoin { get; set; }
        public DbSet<Round> Rounds                { get; set; }
    }
}



// Database Migrations
// 1) update appsettings.json first
// 2) delete old Migrations folder
// 3) dotnet ef migrations add YourMigrationName
// 4) dotnet ef database update
