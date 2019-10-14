using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration;

namespace movieGame.Models
{
    [DataContract]
    public class Movie : BaseEntity
    {
        [Key]
        [DataMember]
        [Display(Name="Movie Id")]
        public int? MovieId            { get; set;}


        [Display(Name="IMDB Id")]
        [DataMember]
        public string ImdbId           { get; set; }


        [Display(Name="Movie Title")]
        [DataMember]
        public string Title            { get; set;}


        [Display(Name="Release Date")]
        [DataMember]
        public DateTime Released       { get; set; }


        [Display(Name="Movie Run Time")]
        [DataMember]
        public string Runtime              { get; set; }


        [Display(Name="Movie Release Year")]
        [DataMember]
        public int? Year                     { get; set;}


        [Display(Name="Movie Director")]
        [DataMember]
        public string Director               { get; set; }


        [Display(Name="Poster")]
        [DataMember]
        public string Poster                 { get; set; }


        [Display(Name="Genre(s)")]
        [DataMember]
        public string Genre                  { get; set; }


        [Display(Name="Movie Release Decade")]
        [DataMember]
        public string Decade                      { get; set; }


        [Display(Name="List Movie Clues")]
        [DataMember]
        public IList<Clue> Clues                  { get; set; }


        // [Display(Name="List Movie Posters")]
        // [DataMember]
        // public IList<Poster> Posters           { get; set; }

        // [DataMember]
        // public IList<MovieTeamJoin> MovieTeamJoin { get; set; }

        // [DataMember]
        // public IList<MovieUserJoin> MovieUserJoin { get; set; }

        public Hint Hints                         { get; set; }

        public DateTime DateCreated { get; set; }  // from IBaseEntity interface
        public DateTime DateUpdated { get; set; }  // from IBaseEntity interface

        // private DateTime _now = new DateTime();

        // public DateTime DateCreated 
        // { 
        //     get => new DateTime();
        //     set => _ = _now;
        // }
        // public DateTime DateUpdated 
        // { 
        //     get => new DateTime();
        //     set => _ = _now;
        // }

        public Movie ()
        {
            Clues = new List<Clue>();
            // Posters = new List<Poster>();
            // MovieTeamJoin = new List<MovieTeamJoin>();
            // MovieUserJoin = new List<MovieUserJoin>();
            Hints = new Hint();
        }

    }

    public sealed class MovieMap : ClassMap<Movie>
    {
        public MovieMap()
        {
            Map(movie => movie.MovieId)     .Index(0);
            // Map(movie => movie.DateCreated) .Index(1);
            Map(movie => movie.Title)       .Index(2);
            // Map(movie => movie.DateUpdated) .Index(3);
            Map(movie => movie.Year)        .Index(4);
            Map(movie => movie.Decade)      .Index(5);
            Map(movie => movie.Director)    .Index(6);
            Map(movie => movie.Poster)      .Index(7);
            Map(movie => movie.ImdbId)      .Index(8);
            Map(movie => movie.Released)    .Index(9);
            Map(movie => movie.Runtime)     .Index(10);
            Map(movie => movie.Genre)       .Index(11);
        }
    }
}