using System;
using System.Linq;
using movieGame.Controllers.PlayerControllers.ManageUsersController;
using movieGame.Infrastructure;
using movieGame.Interfaces;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace movieGame.Controllers
{
    public class ShowViewsController : Controller
    {
        // private readonly IDateTime _dateTime;
        // private readonly ISessionUser _sessionUser;
        private readonly SessionUser2 _sessionUser2;
        private static MovieContext _context;

        public Helpers _h = new Helpers ();
        private readonly ManageUsersController _manageUsers;

        public ShowViewsController (MovieContext context, SessionUser2 sessionUser2)
        {
            _context = context;
            _sessionUser2 = sessionUser2;
        }
        // public ShowViewsController (MovieContext context, IDateTime dateTime, ISessionUser sessionUser)
        // {
        //     _context = context;
        //     _dateTime = dateTime;
        //     _sessionUser = sessionUser;
        // }

        [HttpGet ("")]
        public IActionResult ViewHomePage ()
        {
            _h.StartMethod ();

            // var currentTime = _dateTime.Now;
            // Console.WriteLine ($"currentTime: {currentTime}");

            // var currentTimeString = currentTime.ToString ();
            // HttpContext.Session.SetString ("TIME", currentTimeString);
            // Console.WriteLine ($"currentTimeString: {currentTimeString}");

            // ISessionUser sUser = _sessionUser;
            // Console.WriteLine ($"sUser: {sUser}");
            // Console.WriteLine ($"sUser: {sUser.SessionUser}");

            // CheckSession ();
            return View ("Index");
        }




        [HttpGet ("games")]
        public IActionResult ViewGameListPage ()
        {
            _h.StartMethod ();

            // string currentTimeString = HttpContext.Session.GetString ("TIME");
            // Console.WriteLine ($"'currentTime' from session GAMES LIST: {currentTimeString}");

            // HttpContext.Session.SetString ("TIME", currentTimeString);

            string thisGamesPlayerName = ViewBag.PlayerName = HttpContext.Session.GetString ("UserName");
            Console.WriteLine ($"thisGamesPlayerName: {thisGamesPlayerName}");
            return View ("GameList");
        }


        [HttpGet("single_player")]
        public IActionResult ViewSinglePlayerGamePage()
        {
            _h.StartMethod();

            // var currentTime = HttpContext.Session.GetString ("TIME");
            // Console.WriteLine ($"'currentTime' from session PLAY SINGLE: {currentTime}");

            string thisGamesPlayerName = ViewBag.PlayerName = HttpContext.Session.GetString("UserName");
            Console.WriteLine($"thisGamesPlayerName: {thisGamesPlayerName}");

            Console.WriteLine($"sessionUser2Id: {_sessionUser2.SessionUser2Id}");

            // _manageUsers.CheckSession ();
            // SetThisGamesMovie ();
            return View("PlaySingle");
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
