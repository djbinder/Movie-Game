using System;

namespace movieGame.Models
{

    public class MovieGenreJoin : BaseEntity
    {
        public int MovieGenreJoinId { get; set; }

        public int MovieId { get; set; }

        public Movie Movie { get; set; }

        public int GenreId { get; set; }

        public Genre Genre { get; set; }
    }

    public class MoviePlayerJoin : BaseEntity
    {
        public int MoviePlayerJoinId { get; set; }

        public int PlayerId { get; set; }

        public Player Player { get; set; }

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
    }

    public class GameTeamGameJoin : BaseEntity
    {
        public int GameTeamGameJoinId { get; set; }

        public int GameId { get; set; }

        public Game Game { get; set; }

        public int TeamId { get; set; }

        public Team Team { get; set; }

        public int NumberOfPlayersOnTeam { get; set; }

        public TimeSpan TotalTimeTakenForGuesses { get; set; }

        public bool ThisTeamWon { get; set; }

        public bool ThisTeamLost { get; set; }

        public int PointsReceived { get; set; }
    }



    public class MovieActorJoin : BaseEntity
    {
        public int MovieActorJoinId { get; set; }

        public int MovieId { get; set; }

        public Movie Movie { get; set; }

        public int ActorId { get; set; }

        public Actor Actor { get; set; }
    }
}
