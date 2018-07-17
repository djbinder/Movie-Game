using System;
using System.Collections.Generic;   // this allows you to use 'List< >'



namespace movieGame.Models
{
    public class Movie : BaseEntity
    {
        public int MovieId { get; set;}

        public string Title { get; set;}

        public int Year { get; set;}

        public string Decade { get; set; }

        public IList<Genre> Genres { get; set; }

        public IList<Clue> Clues { get; set; }

        public IList<Actor> Actors { get; set; }

        public IList<MoviePlayerJoin> MoviePlayerJoin { get; set; }

        public IList<MovieGenreJoin> MovieGenreJoin { get; set; }

        public IList<MovieActorJoin> MovieActorJoin { get; set; }

        public Movie ()
        {
            Genres = new List<Genre>();
            Clues = new List<Clue>();
            Actors = new List<Actor>();
            MoviePlayerJoin = new List<MoviePlayerJoin>();
            MovieGenreJoin = new List<MovieGenreJoin>();
            MovieActorJoin = new List<MovieActorJoin>();
        }

    }

}