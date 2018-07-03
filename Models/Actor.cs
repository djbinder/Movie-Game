using System;
using System.Collections.Generic;   // this allows you to use 'List< >'

namespace movieGame.Models
{
    public class Actor : BaseEntity
    {
        public int ActorId { get; set; }

        public string ActorName { get; set; }

        public List<String> ImageURLs { get; set; }

        // public IList<Movie> Movies { get; set; }

        public IList<MovieActorJoin> MovieActorJoin {get; set; }

        public Actor ()
        {
            // Movies = new List<Movie>();
            MovieActorJoin = new List<MovieActorJoin>();
        }
    }
}