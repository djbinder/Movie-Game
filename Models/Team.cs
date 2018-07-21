using System;
using System.Collections.Generic;

namespace movieGame.Models
{
    public class Team : BaseEntity
    {
        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public int NumberOfPlayersOnTeam { get; set; }

        public bool IsItThisTeamsTurn { get; set; }

        public int TeamNumberForThisGame { get; set; }

        public int TeamPoints { get; set; }

        public int GamesPlayed { get; set; }

        public int CountOfMoviesGuessedCorrectly { get; set; }

        public int CountOfMoviesGuessedIncorrectly { get; set; }

        public TimeSpan TotalTimeTakenForGuesses { get; set; }

        public IList<Player> Players { get; set; }

        public IList<Movie> MoviesTeamWon { get; set; }

        public IList<Movie> MoviesTeamLost { get; set; }

        public IList<MovieTeamJoin> MovieTeamJoin { get; set; }

        public IList<GameTeamGameJoin> GameTeamGameJoin { get; set; }


        public Team ()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Players = new List<Player>();
            MoviesTeamWon = new List<Movie>();
            MoviesTeamLost = new List<Movie>();
            MovieTeamJoin = new List<MovieTeamJoin>();
            GameTeamGameJoin = new List<GameTeamGameJoin>();
        }
    }
}