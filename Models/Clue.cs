using System;
using System.ComponentModel.DataAnnotations;
using CsvHelper.Configuration;

namespace movieGame.Models
{
    public class Clue : IBaseEntity
    {
        [Key]
        public int ClueId          { get; set; }

        public int? ClueDifficulty { get; set; }

        public int? CluePoints     { get; set; }

        public string ClueText     { get; set; }

        public int? MovieId        { get; set; }

        private DateTime _now = DateTime.Now;

        public DateTime DateCreated 
        { 
            get => new DateTime();
            set => _ = _now;
        }


        public DateTime DateUpdated 
        { 
            get => new DateTime();
            set => _ = _now;
        }
    }

    public sealed class ClueMap : ClassMap<Clue>
    {
        public ClueMap()
        {
            Map(clue => clue.ClueId         ).Index(0);
            Map(clue => clue.ClueDifficulty ).Index(1);
            Map(clue => clue.CluePoints     ).Index(2);
            Map(clue => clue.ClueText       ).Index(3);
            Map(clue => clue.MovieId        ).Index(5);
        }
    }

}
