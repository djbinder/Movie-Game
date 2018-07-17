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
    public class HomeController : Controller
    {
        private MovieContext _context;

        public HomeController (MovieContext context ) {
            _context = context;
        }

        String Start = "STARTED";
        String Complete = "COMPLETED";

        public int CheckSession()
        {
            Start.ThisMethod();
            int? id = HttpContext.Session.GetInt32("id");

            if(id == null)
            {
                Console.WriteLine($"start new session with id {id}");
                return 0;
            }

            HttpContext.Session.SetInt32("id", (int)id);
            Console.WriteLine($"continuing session with id {id}");

            return (int)id;
        }



        // view landing page
        [HttpGet]
        [Route ("")]
        public IActionResult Index ()
        {
            ViewBag.ErrorMessage = HttpContext.Session.GetString("message");

            GetTopTenLeaders();

            return View ();
        }

        public void GetTopTenLeaders ()
        {
            Start.ThisMethod();

            // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
            var TopTenLeaders = _context.Players.OrderByDescending(t => t.Points).Take(10).ToList();
            ViewBag.Leaders = TopTenLeaders;
        }


        [HttpGet]
        [Route("Leaderboard")]
        public IActionResult ViewLeaderBoard ()
        {
            // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
            var Leaders = ViewBag.Leaders = _context.Players.OrderByDescending(t => t.Points).ToList();
            // ViewBag.Leaders = Leaders;
            return View("leaderboard");
        }


        [HttpGet]
        [Route("PlayerProfile/{id}")]
        public IActionResult ViewPlayerProfile (int id)
        {
            Start.ThisMethod();

            // movieGame.Models.Player
            Player QueryPlayer = _context.Players.Include(m => m.MoviePlayerJoin).ThenInclude(n => n.Movie).SingleOrDefault(p => p.PlayerId == id);

            var GamesWon = ViewBag.GamesWon = QueryPlayer.GamesWon;

            var PlayerName = ViewBag.PlayerName = QueryPlayer.PlayerName;
            // PlayerName.Intro("getting profile for player");
            ViewBag.GamesPlayed = QueryPlayer.GamesAttempted;
            ViewBag.PlayerPoints = QueryPlayer.Points;

            var PlayerMovies = QueryPlayer.MoviePlayerJoin.ToList();

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
                JObject MovieJObject = _movieController.GetMovieJSON(_movie.Title, _movie.Year);


                string MoviePoster = ViewBag.MoviePoster = MovieJObject["Poster"].ToString();
                // MoviePoster.Intro("movie poster");
                // string MoviePoster = ViewBag.MoviePoster = (string)MovieJObject["Value"]["Poster"];
                _moviePoster.Add(MoviePoster);


                if(movie.MovieId <= GamesWon)
                {
                    _movieList.Add(_movie);
                }
            }

            ViewBag.PlayerMovies = _movieList;
            ViewBag.Posters = _moviePoster;

            var MoviesInDatabase = ViewBag.MovieCount = _context.Movies.Count();

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








            // var MovieJObject = _movieController.GetMovieJSON(_movie.Title, _movie.Year);

            // var MovieJSON = Json(MovieJObject);
            // string MovieString = Newtonsoft.Json.JsonConvert.SerializeObject(MovieJSON.Value);
            // var MovieJObject = JObject.Parse(MovieString);


            // var _movieController = new MovieController(_context);
            // _movieController.GetActorImage("Tom Cruise");

            // var MovieJObject = _movieController.GetActorImage("Tom Cruise");
            // MovieJObject.Dig();
            // var ActorJSON = Json(MovieJObject);
            // string ActorString = Newtonsoft.Json.JsonConvert.SerializeObject(ActorJSON.Value);
            // ActorString.Intro("actor string");
            // var ActorJObject = JObject.Parse(ActorString);
            // // ActorJObject.Intro("actor JObject");

            // // Extensions.TableIt(MovieJObject, ActorJSON, ActorString, ActorJObject);

            // string ActorName = ViewBag.ActorName = (string)ActorJObject["Value"]["results"][0]["name"];
            // ActorName.Intro("actor name");
            // // int ActorId = ViewBag.ActorId = (int)ActorJObject["results"][0]["id"];

            // string PictureBaseURL = "https://image.tmdb.org/t/p/w";
            // int PictureSizeSmallest = 92;
            // string ActorPictureURL = (string)ActorJObject["Value"]["results"][0]["profile_path"];
            // string ActorPicSmallest = ViewBag.ActorPicSmallest = PictureBaseURL + PictureSizeSmallest + ActorPictureURL;



            Complete.ThisMethod();
            return View ("test");


    }
}}