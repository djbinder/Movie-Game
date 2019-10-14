using System;
using System.Linq;
using movieGame.Controllers.PlayerControllers.ManageUsersController;
using movieGame.Infrastructure;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace movieGame.Controllers
{
    public class ShowViewsController : Controller
    {
        public Helpers _helpers;
        private static MovieGameContext _context;
        private readonly SessionUser _sessionUser;
        private readonly ManageUsersController _manageUsers;



        public ShowViewsController (Helpers helpers, MovieGameContext context, SessionUser sessionUser, ManageUsersController manageUsers)
        {
            _helpers = helpers;
            _context = context;
            _sessionUser = sessionUser;
            _manageUsers = manageUsers;
        }


        public int CheckSession()
        {
            // _h.StartMethod();
            int? userId = HttpContext.Session.GetInt32("UserId");
            Console.WriteLine($"userId: {userId}");

            if (userId == null)
            {
                Console.WriteLine("NO ID IN SESSION");
                return 0;
            }

            HttpContext.Session.SetInt32("UserId", (int)userId);
            // Console.WriteLine($"continuing session with id {userId}");

            return (int)userId;
        }

        [HttpGet ("")]
        public IActionResult ViewHomePage ()
        {
            _helpers.StartMethod();
            // var currentUserId = HttpContext.Session.GetInt32("SessionUserId");
            // Console.WriteLine($"currentUserId: {currentUserId}");

            // CheckSession ();
            return View ("Index");
        }





        [HttpGet ("games")]
        public IActionResult ViewGameListPage ()
        {
            // _h.StartMethod ();

            // string thisGamesPlayerName = ViewBag.PlayerName = HttpContext.Session.GetString ("UserName");
            // Console.WriteLine ($"thisGamesPlayerName: {thisGamesPlayerName}");

            // var currentUserId = HttpContext.Session.GetInt32("SessionUserId");
            // Console.WriteLine($"currentUserId: {currentUserId}");

            CheckSession();

            return View ("GameList");
        }


        [HttpGet("single_player")]
        public IActionResult ViewSinglePlayerGamePage()
        {
            CheckSession();
            // SetThisGamesMovie ();
            // return View("PlaySingle");
            return RedirectToAction("SetThisGamesMovie","PlaySingle");
            // return RedirectToAction("PlaySingleController.SetThisGamesMovie","PlaySingle");
        }









        [HttpGet ("leaderboard")]
        public IActionResult ViewLeaderBoard ()
        {
            var leaders = ViewBag.Leaders = _context.Users.OrderByDescending (t => t.Points).ToList ();
            // ViewBag.Leaders = Leaders;
            return View ("leaderboard");
        }

        [HttpGet ("links")]
        public IActionResult ViewAllLinksPage () { return View ("AllLinks"); }

        [HttpGet ("error")]
        public IActionResult ViewErrorPage () { return View ("Error"); }

        // [HttpGet("")]
        // public IActionResult ViewHomePage () { return View ("Index"); }

        [HttpGet ("instructions")]
        public IActionResult ViewInstructions () { return View ("instructions"); }

        [HttpGet ("nogame")]
        public IActionResult ViewNoGamePage () { return View ("nogame"); }

        [HttpGet ("the_net")]
        public IActionResult ViewTheNet () { return View ("thenet"); }

        [HttpGet ("test")]
        public IActionResult ViewTestPage () { return View ("test"); }

    }
}
