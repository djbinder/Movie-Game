using Microsoft.AspNetCore.Identity;

namespace movieGame.Models
{
    public class User : IdentityUser
    {
            public int UserId {get; set; }
            public string UserFirstName {get;set;}
            public string UserLastName{get;set;}
            public string UserEmail{get;set;}
            public string UserPassword {get;set;}
            public string Confirm {get;set;}

        public User()
        {
        }

    }
}