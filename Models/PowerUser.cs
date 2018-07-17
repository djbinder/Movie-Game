using Microsoft.AspNetCore.Identity;

namespace movieGame.Models
{
    public class PowerUser : IdentityUser
    {
        public int PowerUserId { get; set; }

        public string PowerUserName { get; set; }
    }
}