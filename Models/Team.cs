using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace movieGame.Models
{
    public class Team : BaseEntity
    {
        [Key]
        public int TeamId { get; set; }

        public string TeamName { get; set; }

        public int TeamPoints { get; set; }

        public int GamesPlayed { get; set; }

        public int CountOfMoviesGuessedCorrectly { get; set; }

        public int CountOfMoviesGuessedIncorrectly { get; set; }

        public IList<MovieTeamJoin> MovieTeamJoin { get; set; } = new List<MovieTeamJoin>();

        public IList<GameTeamJoin> GameTeamJoin { get; set; } = new List<GameTeamJoin>();


        public static implicit operator Team(ActionResult<Team> v)
        {
            throw new NotImplementedException();
        }
    }
}