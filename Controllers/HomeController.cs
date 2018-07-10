// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Text;
// using System.Text.RegularExpressions;
// using ConsoleTables;


using System;
using System.Collections.Generic;       // <--- 'List'
using System.Linq;                      // <--- various db queries (e.g., 'FirstOrDefault', 'OrderBy', etc.)
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Mvc;         // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using Microsoft.EntityFrameworkCore;    // <--- 'include' in db queries
using Newtonsoft.Json.Linq;             // <--- 'JObject'
using movieGame.Models;
using System.Windows.Forms;


namespace movieGame.Controllers
{
    public class HomeController : Controller
    {
        private MovieContext _context;

        public HomeController (MovieContext context ) {
            _context = context;
        }


        // 'SPOTLIGHT' and 'THISMETHOD' EXTENSION METHODS VARIABLES
        String Start = "STARTED";
        String Complete = "COMPLETED";

        public int CheckSession()
        {
            Start.ThisMethod();
            int? id = HttpContext.Session.GetInt32("id");

            if(id == null)
            {
                Console.WriteLine("start new session with id {0}", id);
                return 0;
            }

            HttpContext.Session.SetInt32("id", (int)id);
            Console.WriteLine("continuing session with id {0}", id);

            return (int)id;
        }



        // view landing page
        [HttpGet]
        [Route ("")]
        public IActionResult Index ()
        {
            ViewBag.ErrorMessage = HttpContext.Session.GetString("message");

            // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
            var Leaders = _context.Players.OrderByDescending(t => t.Points).Take(10).ToList();

            ViewBag.Leaders = Leaders;

            return View ();
        }



        // enter name on index page; set session name and id; redirect to 'INITIATE GAME' method
        [HttpPost]
        [Route ("enterName")]
        public IActionResult EnterName (string NameEntered)
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

                // EXISTS ---> movieGame.Models.Player
                Player exists = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);

                if(exists == null)
                {
                    Extensions.Spotlight("this is a new player");
                    // HttpContext.Session.SetString("player", NameEntered);

                    Player newPlayer = new Player () {
                        PlayerName = NameEntered,
                        Points = 0,
                        GamesAttempted = 0,
                        GamesWon = 0,
                        Movies = new List<Movie>(),
                        MoviePlayerJoin = new List<MoviePlayerJoin>(),
                    };

                    _context.Add(newPlayer);
                    _context.SaveChanges();

                    // QUERY PLAYER --> movieGame.Models.Player
                    Player queryPlayer = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);
                    HttpContext.Session.SetInt32("id", queryPlayer.PlayerId);
                }

                // if the player is not a new player, go this way
                else
                {
                    Player currentPlayer = new Player () {
                        PlayerName = exists.PlayerName,
                        Points = exists.Points,
                        GamesAttempted = exists.GamesAttempted,
                        GamesWon = exists.GamesWon,
                        Movies = exists.Movies,
                        MoviePlayerJoin = exists.MoviePlayerJoin,
                    };

                    Extensions.Spotlight("this is an existing player");

                    HttpContext.Session.SetInt32("id", exists.PlayerId);
                }
            }

            HttpContext.Session.SetString("player", NameEntered);

            Complete.ThisMethod();
            return RedirectToAction("initiateGame");
        }



        // initiate game; select movie that will be guessed
        [HttpGet]
        [Route ("/initiateGame")]
        public IActionResult InitiateGame ()
        {
            Start.ThisMethod();
            CheckSession();

            #region set player info
                // PLAYER ID ---> '1' OR '2' etc.
                // PLAYERNAME---> retrieves the current players name
                int? PlayerId = ViewBag.PlayerId = HttpContext.Session.GetInt32("id");
                // string PlayerName = ViewBag.PlayerName = HttpContext.Session.GetString("player");

                // QUERY PLAYER ---> movieGame.Models.Player
                Player queryPlayer = _context.Players.Include(m => m.MoviePlayerJoin).SingleOrDefault(p => p.PlayerId == PlayerId);
                // var gamesAttempted = ViewBag.GamesAttempted = queryPlayer.GamesAttempted;
                var gamesWon = ViewBag.GamesWon = queryPlayer.GamesWon;
                // var totalPoints = ViewBag.TotalPoints = queryPlayer.Points;
            #endregion

            #region SET MOVIE INFO
                // int SetMovieId = gamesWon + 1;
                int SetMovieId = gamesWon + 1;

                // MOVIES IN DATABASE ---> '2' OR '3' etc.
                var moviesInDatabase = _context.Movies.Count();
                // moviesInDatabase.Intro("movies in database");

                if(SetMovieId > moviesInDatabase)
                {
                    Extensions.Spotlight("user is caught up; no new movies to guess");
                    ViewBag.MovieCount = moviesInDatabase;
                    return View("NoGame");
                }

                else {
                    // ONE MOVIE ---> movieGame.Models.Movie
                    var SetMovieObject = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == SetMovieId);
                    SetMovieObject.Title.Intro("this games movie");

                    // SESSION MOVIE ID & NAME ---> sets the name and id for the movie that was selected
                    HttpContext.Session.SetString("sessionMovieTitle", SetMovieObject.Title);
                    HttpContext.Session.SetInt32("sessionMovieId", SetMovieId);
                }
            #endregion


            // #region set guess count
                HttpContext.Session.SetInt32("guesscount", 3);
                int? guessCount = HttpContext.Session.GetInt32("guesscount");
            // #endregion

            List<object> NewList = new List<object>();

            var mGenre = "horror, comedy, fantasy, documentary, sci-fi";
            NewList.Add(mGenre);

            var mDecade = "1990's";
            NewList.Add(mDecade);

            var mDirector = "Christopher Nolan";
            NewList.Add(mDirector);

            HttpContext.Session.SetObjectAsJson("TheList", NewList);

            // Notice that we specify the type ( List ) on retrieval
            List<object> Retrieve = HttpContext.Session.GetObjectFromJson<List<object>>("TheList");

            Complete.ThisMethod();
            return View ("PlayGame");
        }



        [HttpGet]
        [Route("viewLeaderboard")]

        public IActionResult ViewLeaderBoard ()
        {
            // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
            var Leaders = _context.Players.OrderByDescending(t => t.Points).ToList();
            ViewBag.Leaders = Leaders;
            return View("leaderboard");
        }



        [HttpGet]
        [Route("PlayerProfile/{id}")]

        public IActionResult ViewPlayerProfile (int id)
        {
            Start.ThisMethod();

            Player queryPlayer = _context.Players.Include(m => m.MoviePlayerJoin).ThenInclude(n => n.Movie).SingleOrDefault(p => p.PlayerId == id);

            queryPlayer.Intro("query player");
            var GamesWon = ViewBag.GamesWon = queryPlayer.GamesWon;

            ViewBag.PlayerName = queryPlayer.PlayerName;
            ViewBag.GamesPlayed = queryPlayer.GamesAttempted;
            ViewBag.PlayerPoints = queryPlayer.Points;

            // Extensions.TableIt(PlayerName, GamesWon, GamesPlayed, PlayerPoints);

            var PlayerMovies = queryPlayer.MoviePlayerJoin.ToList();

            foreach(var item in PlayerMovies)
            {
                item.Intro("item");
            }

            List<Movie> _movieList = new List<Movie>();
            List<String> _moviePoster = new List<string>();

            foreach(var movie in PlayerMovies)
            {
                Movie _movie = new Movie
                {
                    MovieId = movie.MovieId,
                    Title = movie.Movie.Title,
                    Year = movie.Movie.Year,
                };

                ViewBag.Attempts = movie.AttemptCount;

                var _movieController = new MovieController(_context);
                var GetJSON = _movieController.GetMovieJSON(_movie.Title, _movie.Year);

                var MovieJSON = Json(GetJSON);
                string MovieString = Newtonsoft.Json.JsonConvert.SerializeObject(MovieJSON.Value);
                var MovieJObject = JObject.Parse(MovieString);

                string MoviePoster = ViewBag.MoviePoster = (string)MovieJObject["Value"]["Poster"];

                _moviePoster.Add(MoviePoster);

                if(movie.MovieId <= GamesWon)
                {
                    _movieList.Add(_movie);
                }
            }

            ViewBag.PlayerMovies = _movieList;
            ViewBag.Posters = _moviePoster;

            var moviesInDatabase = ViewBag.MovieCount = _context.Movies.Count();

            Complete.ThisMethod();
            return View("PlayerProfile");
        }



        [HttpGet]
        [Route("theNet")]
        public IActionResult TheNet ()
        {
            Start.ThisMethod();

            // Complete.ThisMethod();
            return View("thenet");
        }



        [HttpGet]
        [Route("TestPage")]
        public IActionResult ViewTestPage ()
        {
            Start.ThisMethod();

            // var _movieController = new MovieController(_context);
            // _movieController.GetActorImage("Tom Cruise");

            // var GetJSON = _movieController.GetActorImage("Tom Cruise");
            // GetJSON.Dig();
            // var ActorJSON = Json(GetJSON);
            // string ActorString = Newtonsoft.Json.JsonConvert.SerializeObject(ActorJSON.Value);
            // ActorString.Intro("actor string");
            // var ActorJObject = JObject.Parse(ActorString);
            // // ActorJObject.Intro("actor jobject");

            // // Extensions.TableIt(GetJSON, ActorJSON, ActorString, ActorJObject);

            // string ActorName = ViewBag.ActorName = (string)ActorJObject["Value"]["results"][0]["name"];
            // ActorName.Intro("actor name");
            // // int ActorId = ViewBag.ActorId = (int)ActorJObject["results"][0]["id"];

            // string PictureBaseURL = "https://image.tmdb.org/t/p/w";
            // int PictureSizeSmallest = 92;
            // string ActorPictureURL = (string)ActorJObject["Value"]["results"][0]["profile_path"];
            // string ActorPicSmallest = ViewBag.ActorPicSmallest = PictureBaseURL + PictureSizeSmallest + ActorPictureURL;



            // Complete.ThisMethod();
            return View ("test");
        }





        string test = "";

        protected void Button1_Click(object sender, EventArgs e)
        {
            Button2_Click(sender, e);
            Button3_Click(sender, e);
        }
        protected void Button2_Click(object sender, EventArgs e)
        {
            test = "Button2";
        }
        protected void Button3_Click(object sender, EventArgs e)
        {
            test = "Button3";
        }



    }
}