using System;
using System.Collections.Generic;   // this allows you to use 'List< >'
using Microsoft.AspNetCore.Identity;

namespace movieGame.Models
{
    public class Player : IdentityUser
    // public class Player : BaseEntity
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string PlayerFirstName { get; set; }

        public string PlayerLastName { get; set; }

        public string PlayerEmail { get; set; }

        public string PlayerPassword { get; set; }

        public int Points { get; set; }

        public int GamesAttempted { get; set; }

        public int GamesWon { get; set; }

        public DateTime CreatedAt {get;set;}

        public DateTime UpdatedAt {get;set;}

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