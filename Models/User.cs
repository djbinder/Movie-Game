using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace movieGame.Models
{
    public class User : BaseEntity
    {
            public int UserId {get; set; }
            public string UserFirstName {get;set;}
            public string UserLastName{get;set;}
            public string UserEmail{get;set;}
            public string UserPassword {get;set;}
            public string Confirm {get;set;}

            [Display(Name = "Player Points")]
            public int Points { get; set; }

            [Display(Name = "Games Attempted")]
            public int GamesAttempted { get; set; }

            [Display(Name = "Games Won")]
            public int GamesWon { get; set; }

            [Display(Name = "Games Lost")]
            public int GamesLost { get { return GamesAttempted - GamesWon; } }

            [Display(Name = "Player Coins")]
            public int UserCoins { get; set; }

            public IList<MovieUserJoin> MovieUserJoin { get; set; }

            public IList<UserTeamJoin> UserTeamJoin { get; set; }

            public User()
            {
                MovieUserJoin = new List<MovieUserJoin>();
                UserTeamJoin = new List<UserTeamJoin>();
            }
    }
}