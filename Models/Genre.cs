using System;
using System.Collections.Generic;   // this allows you to use 'List< >'

namespace movieGame.Models
{
    public class Genre : BaseEntity {
        public int GenreId { get; set; }

        public string GenreName { get; set; }

        // public IList<Movie> Movies { get; set; }

        public IList<MovieGenreJoin> MovieGenreJoin { get; set; }

        public Genre ()
        {
            // Movies = new List<Movie>();
            MovieGenreJoin = new List<MovieGenreJoin>();
        }
    }
}