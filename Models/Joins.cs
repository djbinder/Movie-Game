using System;
using System.Collections.Generic;

namespace movieGame.Models
{
    public class MovieUserJoin : BaseEntity
    {
        public int MovieUserJoinId { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int MovieId { get; set; }

        public Movie Movie { get; set; }

        public int AttemptCount { get; set; }

        public bool WinFlag { get; set; }

        public int PointsReceived { get; set; }

        public int ClueGameWonAt { get; set; }
    }

    public class MovieTeamJoin : BaseEntity
    {
        public int MovieTeamJoinId { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }

        public int MovieId { get; set; }

        public Movie Movie { get; set; }

        public bool WinFlag { get; set; }

        public int PointsReceived { get; set; }

        public int ClueGameWonAt { get; set; }

        public int GameId { get; set; }
    }

    public class GameTeamJoin : BaseEntity
    {
        public int GameTeamJoinId { get; set; }

        public int GameId { get; set; }

        public Game Game { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }

        public TimeSpan TotalTimeTakenForGuesses { get; set; }

        public int OpponentId { get; set; }

        public bool WinFlag { get; set; }

        public int TotalPoints { get; set; }

        public List<Movie> ListOfMoviesGuessedCorrectly { get; set; } = new List<Movie>();
    }

}
