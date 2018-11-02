using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class Team : BaseEntity
    {
        [Key]
        [Display(Name="Team Id")]
        public int TeamId { get; set; }

        [Display(Name="Team Name")]
        public string TeamName { get; set; }


        [Display(Name="# of Players on Team")]
        public int NumberOfPlayersOnTeam { get; set; }


        [Display(Name="Team Points")]
        public int TeamPoints { get; set; }


        [Display(Name="Games Played")]
        public int GamesPlayed { get; set; }


        [Display(Name="Number of Movies Guessed Correctly")]
        public int CountOfMoviesGuessedCorrectly { get; set; }


        [Display(Name="Number of Movies Guessed Incorrectly")]
        public int CountOfMoviesGuessedIncorrectly { get; set; }



        public IList<MovieTeamJoin> MovieTeamJoin { get; set; }

        public IList<GameTeamJoin> GameTeamJoin { get; set; }

        public IList<UserTeamJoin> UserTeamJoin { get; set; }


        public Team ()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            MovieTeamJoin = new List<MovieTeamJoin>();
            GameTeamJoin = new List<GameTeamJoin>();
            UserTeamJoin = new List<UserTeamJoin>();
        }
    }
}