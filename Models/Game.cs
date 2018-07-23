using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class Game : BaseEntity
    {
        [Key]
        [Display(Name="Game Id")]
        public int GameId { get; set; }


        [Display(Name="Game Length")]
        public long DurationOfGame { get; set; }


        [Display(Name="Number of Teams In Game")]
        public int NumberOfTeamsInGame { get; set; }


        [Display(Name="This Games Movie Id")]
        public int ThisGamesMovieId { get; set; }


        public IList<GameTeamJoin> GameTeamJoin { get; set; }

        public Game ()
        {
            GameTeamJoin = new List<GameTeamJoin> ();
        }

    }
}