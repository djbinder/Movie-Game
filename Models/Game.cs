using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class Game : BaseEntity
    {
        [Key]
        public int GameId { get; set; }

        public Team FirstTeam { get; set; } = new Team();
        // public int FirstTeamId { get; set; }
        // public int FirstTeamTotalPoints { get; set; }


        public Team SecondTeam { get; set; } = new Team();
        // public int SecondTeamId { get; set; }
        // public int SecondTeamTotalPoints { get; set; }


        public List<Movie> ListOfMoviesGuessedCorrectly { get; set; } = new List<Movie>();
        public List<Movie> ListOfMoviesQuit { get; set; } = new List<Movie>();
        public List<Round> ListOfRounds { get; set; } = new List<Round>();
        public IList<GameTeamJoin> GameTeamJoin { get; set; } = new List<GameTeamJoin> ();

    }
}