using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using movieGame.Infrastructure;
using movieGame.Models;
using C = System.Console;

namespace movieGame.Controllers.ModelControllers
{
    public class PlayerController : Controller
    {
        private readonly Helpers      _helpers;
        private readonly MovieGameContext _context;

        public PlayerController(Helpers helpers, MovieGameContext context)
        {
            _helpers = helpers;
            _context = context;
        }


        public PlayerController() {}



        // POST: User/Create
        [HttpPost]
        public void Create([Bind("UserId, UserFirstName, UserLastName, UserEmail, UserPassword")] User user)
        {
            _helpers.StartMethod();
            C.WriteLine(user.UserFirstName);

        }
    }
}

// if(ModelState.IsValid)
// {
//     _context.Add(user);
//     await _context.SaveChangesAsync();
// }
// return Ok();