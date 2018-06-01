using Microsoft.EntityFrameworkCore;

namespace movieGame.Models
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options) : base(options) { }

        public DbSet<Movie> Movies {get;set;}
        public DbSet<Clue> Clues {get;set;}
        public DbSet<Player> Players { get; set; }
        public DbSet<MoviePlayerJoin> MoviePlayers { get; set; }


    }
}