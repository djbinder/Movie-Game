using System;
using System.Collections.Generic;   // this allows you to use 'List< >'
using System.Runtime.Serialization;
using Newtonsoft.Json;
using movieGame.Serializers;
using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    [DataContract]
    public class Movie : BaseEntity
    {
        [Key]
        [DataMember]
        [Display(Name="Movie Id")]
        public int MovieId { get; set;}

        [Display(Name="IMDB Id")]
        [DataMember]
        public string imdbId { get; set; }

        [Display(Name="Movie Title")]
        [DataMember]
        public string Title { get; set;}

        [Display(Name="Release Date")]
        [DataMember]
        public DateTime Released { get; set; }


        [Display(Name="Movie Run Time")]
        [DataMember]
        public TimeSpan Runtime { get; set; }

        [Display(Name="Movie Release Year")]
        [DataMember]
        public int Year { get; set;}

        [Display(Name="Movie Director")]
        [DataMember]
        public string Director { get; set; }

        [Display(Name="Poster")]
        [DataMember]
        public string Poster { get; set; }

        [Display(Name="Genre(s)")]
        [DataMember]
        public string Genre { get; set; }

        [Display(Name="Movie Release Decade")]
        [DataMember]
        public string Decade { get; set; }

        [Display(Name="List Movie Genres")]
        [DataMember]
        public IList<Genre> Genres { get; set; }

        [Display(Name="List Movie Clues")]
        [DataMember]
        public IList<Clue> Clues { get; set; }

        [Display(Name="List Movie Actors")]
        [DataMember]
        public IList<Actor> Actors { get; set; }

        [Display(Name="List Movie Posters")]
        [DataMember]
        public IList<Poster> Posters { get; set; }

        [DataMember]
        public IList<MovieTeamJoin> MovieTeamJoin { get; set; }

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
            MovieTeamJoin = new List<MovieTeamJoin>();
            MoviePlayerJoin = new List<MoviePlayerJoin>();
            MovieGenreJoin = new List<MovieGenreJoin>();
            MovieActorJoin = new List<MovieActorJoin>();
        }

    }

}


