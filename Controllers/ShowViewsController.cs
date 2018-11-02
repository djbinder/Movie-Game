using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movieGame.Controllers.Game.MixedControllers;
using movieGame.Controllers.PlayerControllers;
using System;

namespace movieGame.Controllers {
    public class ShowViewsController : Controller {
        private static MovieContext _context;

        public ShowViewsController (MovieContext context)
        {
            _context = context;
        }
        private static GetMovieInfoController _getMovie = new GetMovieInfoController(context: _context);
        private static GetPlayerInfoController _getPlayer = new GetPlayerInfoController(context: _context);


        [HttpGet]
        [Route("links")]
        public IActionResult ViewAllLinksPage()
        {
            Console.WriteLine("viewing all links");
            return View("AllLinks");
        }


        [HttpGet]
        [Route ("movies")]
        public IActionResult ViewAllMovies ()
        {
            ViewBag.Movies = _context.Movies.Include (w => w.Clues).OrderBy (d => d.MovieId).ToList ();
            return View ("AllMovies");
        }


        [HttpGet]
        [Route ("error")]
        public IActionResult ViewErrorPage ()
        {
            Console.WriteLine("viewing error page");
            return View ("Error");
        }


        [HttpGet]
        [Route ("games")]
        public IActionResult ViewGameListPage ()
        {
            string thisGamesPlayersName = ViewBag.PlayerName = HttpContext.Session.GetString ("playername");
            return View ("GameList");
        }


        [HttpGet]
        [Route ("")]
        public IActionResult ViewHomePage ()
        {
            ViewBag.ErrorMessage = HttpContext.Session.GetString ("message");
            return View ("Index");
        }


        [HttpGet]
        [Route ("instructions")]
        public IActionResult ViewInstructions ()
        {
            return View ("instructions");
        }


        [HttpGet]
        [Route ("leaderboard")]
        public IActionResult ViewLeaderBoard ()
        {
            // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
            var leaders = ViewBag.Leaders = _context.Users.OrderByDescending (t => t.Points).ToList ();
            // ViewBag.Leaders = Leaders;
            return View ("leaderboard");
        }


        [HttpGet]
        [Route("addteam")]
        public IActionResult ViewAddTeamPage()
        {
            Console.WriteLine("viewing new team form");
            return View("newteamform");
        }


        [HttpGet]
        [Route("nogame")]
        public IActionResult ViewNoGamePage()
        {
            Console.WriteLine("viewing all links");
            return View("nogame");
        }


        [HttpGet]
        [Route ("player/{id}")]
        public IActionResult ViewPlayerProfilePage (int id)
        {
            Console.WriteLine("trying to view 1 players profile");
            var playersMovies = ViewBag.PlayerMovies = _getPlayer.UsersMovies (id);
            var moviesInDatabase = ViewBag.MovieCount = _context.Movies.Count ();
            return View ("PlayerProfile");
        }


        [HttpGet]
        [Route("group")]
        public IActionResult ViewGroupPlayPage()
        {
            return View("playgroup");
        }


        [HttpGet]
        [Route("single")]
        public IActionResult ViewSinglePlayPage()
        {
            return View("PlaySingle");
        }


        [HttpGet]
        [Route("movie/{id}")]
        public IActionResult ViewSingleMoviePage ()
        {
            Console.WriteLine("trying to view 1 movies details");
            return View("SingleMovie");
        }


        [HttpGet]
        [Route ("net")]
        public IActionResult ViewTheNet ()
        {
            return View ("thenet");
        }


        [HttpGet]
        [Route ("test")]
        public IActionResult ViewTestPage ()
        {
            return View ("test");
        }

    }
}