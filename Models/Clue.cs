using System;
using System.Collections.Generic;   // this allows you to use 'List< >'




namespace movieGame.Models
{
    public class Clue : BaseEntity
    {
        public int ClueId { get; set; }

        public string ClueText { get; set; }

        public int ClueDifficulty { get; set; }

        public int CluePoints { get; set; }

        public int MovieId { get; set; }
    }
}