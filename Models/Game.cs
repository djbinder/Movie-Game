using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class Game : IBaseEntity
    {
        [Key]
        public int GameId { get; set; }

        public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        public DateTime DateUpdated { get; set; }  // from IBaseEntity interface

        public List<GameTeamJoin> GameTeamJoin          { get; set; }
        

    }


    public class SinglePlayGame : Game
    {
        public User Player { get; set; }
        public List<Movie> ListOfMoviesGuessedCorrectly { get; set; }
        public List<Movie> ListOfMoviesQuit             { get; set; }

        public SinglePlayGame()
        {
            ListOfMoviesGuessedCorrectly = new List<Movie>();
            ListOfMoviesQuit  = new List<Movie>();
        }
    }


    public class TeamPlayGame : Game
    {
        public Team FirstTeam { get; set; }
        // public int FirstTeamId { get; set; }
        // public int FirstTeamTotalPoints { get; set; }


        public Team SecondTeam { get; set; }
        // public int SecondTeamId { get; set; }
        // public int SecondTeamTotalPoints { get; set; }

        public List<GameTeamJoin> GameTeamJoin          { get; set; }
        public List<Round> ListOfRounds                 { get; set; }


        public TeamPlayGame()
        {
            FirstTeam    = new Team();
            SecondTeam   = new Team();
            GameTeamJoin = new List<GameTeamJoin> ();
            ListOfRounds = new List<Round>();
        }
    }
}
