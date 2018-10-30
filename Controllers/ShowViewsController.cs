using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace movieGame.Controllers {
    public class ShowViewsController : Controller {
        private MovieContext _context;
        private Player _player;
        private GetPlayerInfoController _getPlayerInfoController;
        private GetMovieInfoController _getMovieInfoController;

        public ShowViewsController (MovieContext context) {
            _context = context;
        }

        public GetPlayerInfoController GetPlayer {
            get => _getPlayerInfoController;
            set => _getPlayerInfoController = value;
        }

        public Player Player {
            get => _player;
            set => _player = value;
        }
        public GetMovieInfoController GetMovie {
            get => _getMovieInfoController;
            set => _getMovieInfoController = value;
        }

        // [HttpGet]
        // [Route("LogInRegisterPage")]
        // public IActionResult ViewLogInRegisterPage()
        // {
        //     return View("LoginRegister");
        // }

        // view landing page
        [HttpGet]
        [Route ("")]
        public IActionResult ViewHomePage () {

            ViewBag.ErrorMessage = HttpContext.Session.GetString ("message");
            return View ("Index");
        }

        [HttpGet]
        [Route ("PlayerProfile/{id}")]
        public IActionResult ViewPlayerProfile (int id) {

            GetPlayer = new GetPlayerInfoController (_context);

            Player = ViewBag.Player = GetPlayer.GetPlayer (id);
            Player.PlayerName.Intro ("viewing player profile for");

            var PlayersMovies = ViewBag.PlayerMovies = GetPlayer.PlayersMovies (id);
            var MoviesInDatabase = ViewBag.MovieCount = _context.Movies.Count ();

            return View ("PlayerProfile");
        }

        [HttpGet]
        [Route ("Instructions")]

        public IActionResult ViewInstructions () {

            return View ("instructions");
        }

        [HttpGet]
        [Route ("GameList")]
        public IActionResult ViewGameList () {

            string ThisGamesPlayersName = HttpContext.Session.GetString ("player");
            ViewBag.PlayerName = ThisGamesPlayersName;
            ThisGamesPlayersName.Intro ("this games players name");

            int? ThisGamesPlayerId = HttpContext.Session.GetInt32 ("id");

            return View ("GameList");
        }

        // [HttpGet]
        // [Route("PlaySingle")]

        // public IActionResult ViewPlaySingle()
        // {
        //     Start.ThisMethod();
        //     Complete.ThisMethod();
        //     return View("PlaySingle");
        // }

        [HttpGet]
        [Route ("Leaderboard")]
        public IActionResult ViewLeaderBoard () {
            // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
            var Leaders = ViewBag.Leaders = _context.Players.OrderByDescending (t => t.Points).ToList ();
            // ViewBag.Leaders = Leaders;
            return View ("leaderboard");
        }

        // view table of all movies including Id, Title, Description, Year, and list of Clues
        [HttpGet]
        [Route ("AllMovies")]
        public IActionResult ViewAllMovies () {
            ViewBag.Movies = _context.Movies.Include (w => w.Clues).OrderBy (d => d.MovieId).ToList ();

            return View ("AllMovies");
        }

        // [HttpGet]
        // [Route("Movie/{id}")]
        // public IActionResult ViewSingleMovie ()
        // {
        //     Start.ThisMethod();
        //     Complete.ThisMethod();
        //     return View("SingleMovie");
        // }

        [HttpGet]
        [Route ("TheNet")]
        public IActionResult ViewTheNet () {
            return View ("thenet");
        }

        [HttpGet]
        [Route ("ErrorPage")]
        public IActionResult ViewErrorPage () {
            return View ("Error");
        }

    }
}