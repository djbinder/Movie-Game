using System.ComponentModel.DataAnnotations;

namespace movieGame.Models
{
    public class Round : BaseEntity
    {
       [Key]
       public int RoundId                  { get; set; }
       public int GameId                   { get; set; }

       public Game Game                    { get; set; }
       public Movie Movie                  { get; set; }
       public Team FirstTeam               { get; set; }
       public Team SecondTeam              { get; set; }

       public int? FirstTeamGuesses         { get; set; }
       public int? SecondTeamGuesses        { get; set; }
       public bool FirstTeamWinFlag         { get; set; }
       public bool SecondTeamWinFlag        { get; set; }
       public bool BothTeamsQuitFlag        { get; set; }
       public int? FirstTeamPointsReceived  { get; set; }
       public int? SecondTeamPointsReceived { get; set; }
       public int? ClueGameIsWonAt          { get; set; }


       public Round()
       {
           Game       = new Game();
           Movie      = new Movie();
           FirstTeam  = new Team();
           SecondTeam = new Team();
       }
    }
}
