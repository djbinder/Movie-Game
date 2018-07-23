using System;
using System.Collections.Generic;   // this allows you to use 'List< >'
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace movieGame.Models
{
    public class Player : BaseEntity

    {
        public int PlayerId { get; set; }

        [Display(Name = "Player Name")]
        public string PlayerName { get; set; }

        [Display(Name = "Player First Name")]
        public string PlayerFirstName { get; set; }

        [Display(Name = "Player Last Name")]
        public string PlayerLastName { get; set; }

        [Display(Name = "Player Email")]
        public string PlayerEmail { get; set; }

        [Display(Name = "Player Password")]
        public string PlayerPassword { get; set; }

        [Display(Name = "Player Points")]
        public int Points { get; set; }

        [Display(Name = "Games Played")]
        public int GamesAttempted { get; set; }

        [Display(Name = "Games Won")]
        public int GamesWon { get; set; }

        // [Display(Name = "Games Lost")]
        // public int GamesLost { get { return GamesAttempted - GamesWon; } }

        [Display(Name = "Player Coins")]
        public int PlayerCoins { get; set; }

        public IList<Movie> Movies {get; set; }

        public IList<MoviePlayerJoin> MoviePlayerJoin { get; set; }

        public Player()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            Movies = new List<Movie>();
            MoviePlayerJoin = new List<MoviePlayerJoin>();
        }
    }
}