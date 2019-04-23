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

        public int AllTimePoints { get; set; }

        public int AllTimeGamesPlayed { get; set; }

        public int AllTimeCountOfMoviesGuessedCorrectly { get; set; }

        public int AllTimeCountOfMoviesGuessedIncorrectly { get; set; }

        public IList<MovieTeamJoin> MovieTeamJoin { get; set; } = new List<MovieTeamJoin>();

        public IList<GameTeamJoin> GameTeamJoin { get; set; } = new List<GameTeamJoin>();


        public static implicit operator Team(ActionResult<Team> v)
        {
            throw new NotImplementedException();
        }
    }
}