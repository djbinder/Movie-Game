using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace movieGame.Models
{

    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options) : base(options) { }
        public DbSet<Clue> Clues {get;set;}
        public DbSet<Game> Games { get; set; }
        public DbSet<Movie> Movies {get;set;}
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Hints> Hints { get; set; }
        public DbSet<GameTeamJoin> GameTeamJoin { get; set; }
        public DbSet<MovieUserJoin> MovieUserJoin { get; set; }
        public DbSet<MovieTeamJoin> MovieTeamJoin { get; set; }
        public DbSet<Round> Rounds { get; set; }
    }
}



// Database Migrations
// update appsettings.json first
// delete old Migrations folder
// dotnet ef migrations add YourMigrationName
// dotnet ef database update