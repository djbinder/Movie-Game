using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace movieGame.Controllers
{
    public class ShowViewsController : Controller {
        private static MovieContext _context;

        public ShowViewsController (MovieContext context)
        {
            _context = context;
        }


        [HttpGet("games")]
        public IActionResult ViewGameListPage ()
        {
            string thisGamesPlayersName = ViewBag.PlayerName = HttpContext.Session.GetString ("PlayerName");
            return View ("GameList");
        }

        [HttpGet("leaderboard")]
        public IActionResult ViewLeaderBoard ()
        {
            var leaders = ViewBag.Leaders = _context.Users.OrderByDescending (t => t.Points).ToList ();
            // ViewBag.Leaders = Leaders;
            return View ("leaderboard");
        }


        [HttpGet("links")]
        public IActionResult ViewAllLinksPage() { return View("AllLinks"); }

        [HttpGet("error")]
        public IActionResult ViewErrorPage () { return View ("Error"); }

        // [HttpGet("")]
        // public IActionResult ViewHomePage () { return View ("Index"); }

        [HttpGet("instructions")]
        public IActionResult ViewInstructions () { return View ("instructions"); }

        [HttpGet("nogame")]
        public IActionResult ViewNoGamePage() { return View("nogame"); }

        [HttpGet("the_net")]
        public IActionResult ViewTheNet () { return View ("thenet"); }

        [HttpGet("test")]
        public IActionResult ViewTestPage () { return View ("test"); }

    }
}