using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class Game : BaseEntity
    {
        [Key]
        public int GameId { get; set; }

        public long DurationOfGame { get; set; }

        public int NumberOfTeamsInGame { get; set; }

        public int ThisGamesMovieId { get; set; }

        public Team FirstTeam { get; set; } = new Team();

        public Team SecondTeam { get; set; } = new Team();

        public IList<GameTeamJoin> GameTeamJoin { get; set; }

        public Game ()
        {
            GameTeamJoin = new List<GameTeamJoin> ();
        }

    }
}