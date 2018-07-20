using System;
using System.Collections.Generic;   // this allows you to use 'List< >'
using System.Runtime.Serialization;
using Newtonsoft.Json;
using movieGame.Serializers;

namespace movieGame.Models
{
    [DataContract]
    public class Movie : BaseEntity
    {
        [DataMember]
        public int MovieId { get; set;}

        [DataMember]
        public string imdbId { get; set; }

        [DataMember]
        public string Title { get; set;}

        [DataMember]
        public DateTime Released { get; set; }

        [DataMember]
        [JsonConverter(typeof(RunTimeSerializer))]
        public TimeSpan Runtime { get; set; }

        [DataMember]
        public int Year { get; set;}

        [DataMember]
        public string Director { get; set; }

        [DataMember]
        public string Poster { get; set; }

        [DataMember]
        public string Genre { get; set; }

        [DataMember]
        public string Decade { get; set; }

        [DataMember]
        public IList<Genre> Genres { get; set; }

        [DataMember]
        public IList<Clue> Clues { get; set; }

        [DataMember]
        public IList<Actor> Actors { get; set; }

        [DataMember]
        public IList<Poster> Posters { get; set; }

        [DataMember]
        public IList<MoviePlayerJoin> MoviePlayerJoin { get; set; }

        [DataMember]
        public IList<MovieGenreJoin> MovieGenreJoin { get; set; }

        [DataMember]
        public IList<MovieActorJoin> MovieActorJoin { get; set; }


        public Movie ()
        {
            Genres = new List<Genre>();
            Clues = new List<Clue>();
            Actors = new List<Actor>();
            Posters = new List<Poster>();
            MoviePlayerJoin = new List<MoviePlayerJoin>();
            MovieGenreJoin = new List<MovieGenreJoin>();
            MovieActorJoin = new List<MovieActorJoin>();
        }

    }

}


//

// example of inverse property

// namespace YourNamespace.Models
// {
//     public class TwitterUser : BaseEntity
//     {
//         public TwitterUser()
//         {
//             Followers = new List<Connection>();
//             UsersFollowed = new List<Connection>();
//         }

//         public int TwitterUserId { get; set; }

//         [InverseProperty("UserFollowed")]
//         public List<Connection> Followers { get; set; }

//         [InverseProperty("Follower")]
//         public List<Connection> UsersFollowed { get; set; }
//     }
// }