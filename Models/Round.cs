using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class Round : BaseEntity
    {
       [Key]
       public int RoundId { get; set; }
       public int GameId { get; set; }
       public Game Game { get; set; } = new Game();
       public Movie Movie { get; set; } = new Movie();
       public Team FirstTeam { get; set; } = new Team();
       public Team SecondTeam { get; set; } = new Team();
       public int FirstTeamGuesses { get; set; }
       public int SecondTeamGuesses { get; set; }
       public bool FirstTeamWinFlag { get; set; }
       public bool SecondTeamWinFlag { get; set; }
       public bool BothTeamsQuitFlag { get; set; }
       public int FirstTeamPointsReceived { get; set; }
       public int SecondTeamPointsReceived { get; set; }
       public int ClueGameIsWonAt { get; set; }
    }
}