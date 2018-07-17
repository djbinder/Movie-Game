using System;
using System.Collections;
using System.Collections.Generic;       // <--- 'List'
using System.Linq;                      // <--- various db queries (e.g., 'FirstOrDefault', 'OrderBy', etc.)
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Mvc;         // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using Microsoft.EntityFrameworkCore;    // <--- 'include' in db queries
using Newtonsoft.Json.Linq;             // <--- 'JObject'
using movieGame.Models;


namespace movieGame.Controllers
{
    public class InitiateGameController : Controller
    {
        private MovieContext _context;

        public InitiateGameController (MovieContext context ) {
            _context = context;
        }


        // 'SPOTLIGHT' and 'THISMETHOD' EXTENSION METHODS VARIABLES
        String Start = "STARTED";
        String Complete = "COMPLETED";


        [HttpGet]
        [Route("GameList")]
        public IActionResult ViewGameList ()
        {
            Start.ThisMethod();
            Complete.ThisMethod();

            return View("GameList");
        }


        // enter name on index page; set session name and id; redirect to 'INITIATE GAME' method
        [HttpPost]
        [Route ("SetGamePlayer")]
        public IActionResult SetGamePlayer (string NameEntered)
        {
            Start.ThisMethod();

            if(NameEntered == null)
            {
                Extensions.Spotlight("no name was entered");
                HttpContext.Session.SetString("message", "Please Enter a Name to Play!");
                return RedirectToAction("Index");
            }

            else
            {
                NameEntered.Intro("current player is");

                // EXISTS ---> checks if the player is already in the database; movieGame.Models.Player
                Player ExistingPlayer = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);

                if(ExistingPlayer == null)
                {
                    Extensions.Spotlight("this is a new player");

                    Player NewPlayer = new Player () {
                        PlayerName = NameEntered,
                        Points = 0,
                        GamesAttempted = 0,
                        GamesWon = 0,
                        Movies = new List<Movie>(),
                        MoviePlayerJoin = new List<MoviePlayerJoin>(),
                    };

                    _context.Add(NewPlayer);
                    _context.SaveChanges();

                    // QUERY PLAYER --> movieGame.Models.Player
                    Player QueryPlayer = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);
                    HttpContext.Session.SetInt32("id", QueryPlayer.PlayerId);
                }

                // if the player is not a new player, go this way
                else
                {
                    Player CurrentPlayer = new Player () {
                        PlayerName = ExistingPlayer.PlayerName,
                        Points = ExistingPlayer.Points,
                        GamesAttempted = ExistingPlayer.GamesAttempted,
                        GamesWon = ExistingPlayer.GamesWon,
                        Movies = ExistingPlayer.Movies,
                        MoviePlayerJoin = ExistingPlayer.MoviePlayerJoin,
                    };

                    Extensions.Spotlight("this is an existing player");

                    HttpContext.Session.SetInt32("id", ExistingPlayer.PlayerId);
                }
            }

            HttpContext.Session.SetString("player", NameEntered);

            // Complete.ThisMethod();
            return RedirectToAction("initiateGame");
        }



        // initiate game; select movie that will be guessed
        [HttpGet]
        [Route ("InitiateGame")]
        public IActionResult InitiateGame ()
        {
            Start.ThisMethod();

            // PLAYER ID ---> '1' OR '2' etc.
            // PLAYERNAME---> retrieves the current players name
            int? PlayerId = ViewBag.PlayerId = HttpContext.Session.GetInt32("id");
            // string PlayerName = ViewBag.PlayerName = HttpContext.Session.GetString("player");

            // QUERY PLAYER ---> movieGame.Models.Player
            Player QueryPlayer = _context.Players.Include(m => m.MoviePlayerJoin).SingleOrDefault(p => p.PlayerId == PlayerId);

            // GAMES WON --> how many games has the player won; the next movie served is based off of this
            var GamesWon = ViewBag.GamesWon = QueryPlayer.GamesWon;

            int SetMovieId = GamesWon + 1;

            // MOVIES IN DATABASE ---> '2' OR '3' etc.
            var MoviesInDatabase = _context.Movies.Count();

            if(SetMovieId > MoviesInDatabase)
            {
                Extensions.Spotlight("user is caught up; no new movies to guess");
                ViewBag.MovieCount = MoviesInDatabase;
                return View("NoGame");
            }

            SetGamesMovie(SetMovieId);
            SetGameGuessCount();
            SetMovieHints();

            // Complete.ThisMethod();
            return View ("PlayGame");
        }


        public void SetGamesMovie(int MovieId)
        {
            Start.ThisMethod();
            int SetMovieId = MovieId;

            // MOVIES IN DATABASE ---> '2' OR '3' etc.
            var MoviesInDatabase = _context.Movies.Count();

            // ONE MOVIE ---> movieGame.Models.Movie
            var SetMovieObject = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == SetMovieId);
            SetMovieObject.Title.Intro("this games movie");

            // SESSION MOVIE ID & NAME ---> sets the name and id for the movie that was selected
            HttpContext.Session.SetString("sessionMovieTitle", SetMovieObject.Title);
            HttpContext.Session.SetInt32("sessionMovieYear", SetMovieObject.Year);
            HttpContext.Session.SetInt32("sessionMovieId", SetMovieId);

            Hashtable SessionMovieInfoTable = new Hashtable();
            SessionMovieInfoTable.Add("SessionMovieTitle", SetMovieObject.Title);
            SessionMovieInfoTable.Add("SessionMovieYear", SetMovieObject.Year);
            HttpContext.Session.SetObjectAsJson("SessionMovieInfo", SessionMovieInfoTable);
        }

        public void SetGameGuessCount()
        {
            Start.ThisMethod();

            HttpContext.Session.SetInt32("guesscount", 3);
            int? guessCount = HttpContext.Session.GetInt32("guesscount");
        }



        public void SetMovieHints()
        {
            Start.ThisMethod();

            var SessionMovieTitle = HttpContext.Session.GetString("sessionMovieTitle");
            int SessionMovieYear = (int)HttpContext.Session.GetInt32("sessionMovieYear");

            var _movieController = new MovieController(_context);

            // Microsoft.AspNetCore.Mvc.JsonResult
            JObject OneMovieJObject = _movieController.GetMovieJSON(SessionMovieTitle, SessionMovieYear);

            // System.Collections.Hashtable
            Hashtable OneMovieHashTable = _movieController.GetMovieInfo(OneMovieJObject);

            string MovieGenre = ViewBag.MovieGenre = OneMovieHashTable["MovieGenre"].ToString();
            string MovieDirector = ViewBag.MovieDirector = OneMovieHashTable["MovieDirector"].ToString();
            string MovieTitle = ViewBag.MovieTitle = OneMovieHashTable["MovieTitle"].ToString();
            string MovieReleaseYear = ViewBag.MovieReleaseYear = OneMovieHashTable["MovieReleaseYear"].ToString();

            // MovieGenre.Intro("movie genre");
            // MovieDirector.Intro("movie Director");
            // MovieTitle.Intro("movie Title");
            // MovieReleaseYear.Intro("movie ReleaseYear");

            // the below represents three options to set movie info in JSON object; only one is truly needed

            // option #1
            List<object> SetHintsList = new List<object>();
            SetHintsList.Add(MovieGenre);
            SetHintsList.Add(MovieReleaseYear);
            SetHintsList.Add(MovieDirector);

            // option #2
            Hashtable SetHintsHashTable = new Hashtable();
            SetHintsHashTable.Add("Genre", MovieGenre);
            SetHintsHashTable.Add("Decade", MovieReleaseYear);
            SetHintsHashTable.Add("Director", MovieDirector);

            // option #3
            Dictionary<string, string> SetHintsDictionary = new Dictionary<string, string>();
            SetHintsDictionary.Add("Genre", MovieGenre);
            SetHintsDictionary.Add("Decade", MovieReleaseYear);
            SetHintsDictionary.Add("Director", MovieDirector);

            // these set each of the above options as session objects/json
            HttpContext.Session.SetObjectAsJson("MovieHints", SetHintsList);
            HttpContext.Session.SetObjectAsJson("MovieHintsTable", SetHintsHashTable);
            HttpContext.Session.SetObjectAsJson("MovieHintsDictionary", SetHintsDictionary);
        }

    }
}