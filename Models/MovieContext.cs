using Microsoft.EntityFrameworkCore;

namespace movieGame.Models
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options) : base(options) { }

        public DbSet<Movie> Movies {get;set;}
        public DbSet<Clue> Clues {get;set;}
        public DbSet<Player> Players { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<MovieGenreJoin> MovieGenreJoin { get; set; }
        public DbSet<MoviePlayerJoin> MoviePlayerJoin { get; set; }
        public DbSet<MovieActorJoin> MovieActorJoin { get; set; }


    }
}


// Database Migrations
// dotnet ef migrations add YourMigrationName
// dotnet ef database update