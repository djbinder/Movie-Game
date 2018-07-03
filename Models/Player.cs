using System;
using System.Collections.Generic;   // this allows you to use 'List< >'

namespace movieGame.Models
{
    public class Player : BaseEntity
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public int Points { get; set; }

        public int GamesAttempted { get; set; }

        public int GamesWon { get; set; }

        public IList<Movie> Movies {get; set; }

        public IList<MoviePlayerJoin> MoviePlayerJoin { get; set; }

        public Player()
        {
            Movies = new List<Movie>();
            MoviePlayerJoin = new List<MoviePlayerJoin>();
        }
    }
}