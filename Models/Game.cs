using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class Game : BaseEntity
    {
        [Key]
        public int GameId { get; set; }

        public Team FirstTeam { get; set; }
        // public int FirstTeamId { get; set; }
        // public int FirstTeamTotalPoints { get; set; }


        public Team SecondTeam { get; set; }
        // public int SecondTeamId { get; set; }
        // public int SecondTeamTotalPoints { get; set; }


        public List<Movie> ListOfMoviesGuessedCorrectly { get; set; }
        public List<Movie> ListOfMoviesQuit             { get; set; }
        public List<Round> ListOfRounds                 { get; set; }
        public List<GameTeamJoin> GameTeamJoin          { get; set; }


        public Game()
        {
            FirstTeam = new Team();
            SecondTeam = new Team();
            ListOfMoviesGuessedCorrectly = new List<Movie>();
            ListOfMoviesQuit  = new List<Movie>();
            ListOfRounds = new List<Round>();
            GameTeamJoin = new List<GameTeamJoin> ();
        }

    }
}
