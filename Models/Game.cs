using System.Collections.Generic;

namespace movieGame.Models
{
    public class Game : BaseEntity
    {
        public int GameId { get; set; }

        public IList<Player> Players { get; set; }

        public long DurationOfGame { get; set; }

        public int MoviesPlayed { get; set; }
    }
}