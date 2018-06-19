using System;
using System.Collections.Generic;   // this allows you to use 'List< >'
// using MongoDB.Bson.Serialization.Attributes;


namespace movieGame.Models
{
    public class Movie : BaseEntity
    {
        public int MovieId { get; set;}

        public string Title { get; set;}

        // public string Description { get; set;}

        // public string Director { get; set;}

        public int Year { get; set;}

        public IList<Clue> Clues { get; set; }

        // public IList<Player> Players {get; set; }

        public IList<MoviePlayerJoin> MoviePlayerJoin { get; set; }

        public Movie ()
        {
            Clues = new List<Clue>();
            // Players = new List<Player>();
            MoviePlayerJoin = new List<MoviePlayerJoin>();
        }

    }


    public class Clue : BaseEntity
    {
        public int ClueId { get; set; }

        public string ClueText { get; set; }

        public int ClueDifficulty { get; set; }

        public int CluePoints { get; set; }

        public int MovieId { get; set; }
    }



    public class Player : BaseEntity
    {
        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public int Points { get; set; }

        public int GamesPlayed { get; set; }

        public IList<Movie> Movies {get; set; }

        public IList<MoviePlayerJoin> MoviePlayerJoin { get; set; }

        public Player()
        {
            Movies = new List<Movie>();
            MoviePlayerJoin = new List<MoviePlayerJoin>();
        }
    }

    public class MoviePlayerJoin : BaseEntity
    {
        public int MoviePlayerJoinId { get; set; }

        public int PlayerId { get; set; }

        public Player Players { get; set; }

        public int MovieId { get; set; }

        public Movie Movies { get; set; }
    }
}