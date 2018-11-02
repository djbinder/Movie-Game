using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace movieGame.Models
{

    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options) : base(options) { }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Clue> Clues {get;set;}
        public DbSet<Game> Games { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies {get;set;}
        // public DbSet<Player> Players { get; set; }
        public DbSet<Poster> Posters { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Hints> Hints { get; set; }

        public DbSet<GameTeamJoin> GameTeamJoin { get; set; }
        public DbSet<MovieActorJoin> MovieActorJoin { get; set; }
        public DbSet<MovieGenreJoin> MovieGenreJoin { get; set; }
        public DbSet<MovieUserJoin> MovieUserJoin { get; set; }
        public DbSet<MovieTeamJoin> MovieTeamJoin { get; set; }
        public DbSet<UserTeamJoin> UserTeamJoin { get; set; }

    }
}


// Database Migrations
// dotnet ef migrations add YourMigrationName
// dotnet ef database update