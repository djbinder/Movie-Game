using System;
using System.Collections;
using System.Collections.Generic;       // <--- 'List'
using System.Diagnostics;
using System.Linq;                      // <--- various db queries (e.g., 'FirstOrDefault', 'OrderBy', etc.)
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
// using System.Windows.Forms;
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Mvc;         // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using Microsoft.EntityFrameworkCore;    // <--- 'include' in db queries
using Newtonsoft.Json;                  // <--- 'JsonConvert'
using Newtonsoft.Json.Linq;             // <--- 'JObject'
using RestSharp;                        // <--- REST related things: 'RestClient', 'RestRequest', 'IRestResponse'
using movieGame.Models;

namespace movieGame.Controllers
{
    public class HomeController : Controller
    {
        private MovieContext _context;

        public HomeController (MovieContext context) {
            _context = context;
        }

        public int CheckSession()
        {
            int? id = HttpContext.Session.GetInt32("id");
            if(id == null)
            {
                return 0;
            }
            return (int)id;
        }


        String Open = "STARTED";
        String Close = "COMPLETED";



        #region InitiateGame
            // view landing page
            [HttpGet]
            [Route ("")]
            public IActionResult Index ()
            {
                Open.MarkMethod();

                GetBackgroundPosters();

                ViewBag.ErrorMessage = HttpContext.Session.GetString("message");

                // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
                var Leaders = _context.Players.OrderByDescending(t => t.Points).ToList();
                Leaders.Intro("leaders");

                ViewBag.Leaders = Leaders;

                Close.MarkMethod();

                return View ();
            }


            // enter name on index page; set session name and id; redirect to 'INITIATE GAME' method
            [HttpPost]
            [Route ("enterName")]
            public IActionResult EnterName (string NameEntered)
            {
                Console.WriteLine("---------------'ENTER NAME' METHOD STARTED---------------");

                Console.WriteLine("NAME ENTERED ---> " + NameEntered);

                if(NameEntered == null)
                {
                    Console.WriteLine("NO NAME ENTERED");
                    ViewBag.ErrorMessage = "Please Enter a Name to Play!";
                    HttpContext.Session.SetString("message", "Please Enter a Name to Play!");
                    return RedirectToAction("Index");
                }

                else
                {
                    // EXISTS ---> movieGame.Models.Player
                    Player exists = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);

                    if(exists == null)
                    {
                        Console.WriteLine("USER NAME ENTERED DOES NOT EXIST");
                        HttpContext.Session.SetString("player", NameEntered);

                        Player newPlayer = new Player () {
                            PlayerName = NameEntered,
                            Points = 0,
                            GamesPlayed = 0,
                            Movies = new List<Movie>(),
                            MoviePlayerJoin = new List<MoviePlayerJoin>(),
                        };

                        _context.Add(newPlayer);
                        _context.SaveChanges();

                        // QUERY PLAYER --> QUERY PLAYER ---> movieGame.Models.Player
                        Player queryPlayer = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);
                        HttpContext.Session.SetInt32("id", queryPlayer.PlayerId);
                    }

                    // if the player is not a new player, go this way
                    else
                    {
                        Console.WriteLine("USER NAME ALREADY EXISTS");

                        HttpContext.Session.SetString("player", NameEntered);
                        HttpContext.Session.SetInt32("id", exists.PlayerId);

                        // exists.GamesPlayed = exists.GamesPlayed + 1;

                        // _context.SaveChanges();
                    }
                }
                Console.WriteLine("---------------'ENTER NAME' METHOD COMPLETED---------------");

                return RedirectToAction("initiateGame");
            }


            // initiate game; select movie that will be guessed
            [HttpGet]
            [Route ("/initiateGame")]
            public IActionResult InitiateGame ()
            {
                Console.WriteLine("----------------'INITIATE GAME' METHOD STARTED---------------");

                // PLAYER ID ---> '1' OR '2' etc.
                int? PlayerId = ViewBag.PlayerId = HttpContext.Session.GetInt32("id");

                HttpContext.Session.SetInt32("guesscount", 3);
                int? guessCount = HttpContext.Session.GetInt32("guesscount");
                guessCount.Intro("guess count");

                // PLAYERNAME---> retrieves the current players name
                string PlayerName = ViewBag.PlayerName = HttpContext.Session.GetString("player");
                PlayerName.Intro("player name");

                Player queryPlayer = _context.Players.FirstOrDefault(p => p.PlayerName == PlayerName);
                queryPlayer.Intro("query player");

                var gamesPlayed = ViewBag.GamesPlayed = queryPlayer.GamesPlayed;
                Console.WriteLine("GAMES PLAYED A --> ", gamesPlayed);

                var newGamesPlayed = gamesPlayed + 1;
                Console.WriteLine("GAMES PLAYED B --> ", newGamesPlayed);

                queryPlayer.GamesPlayed = newGamesPlayed;
                _context.SaveChanges();

                var totalPoints = ViewBag.TotalPoints = queryPlayer.Points;
                Console.WriteLine("TOTAL POINTS --> ", totalPoints);

                if(CheckSession() == 0)
                {
                    Console.WriteLine("THIS IS THE START OF A NEW SESSION --> " + User);
                }

                else
                {
                    // CHECK SESSION YES ---> System.Security.Claims.ClaimsPrincipal
                    Console.WriteLine("THIS IS A CONTINUATION OF EXISTING SESSION ---> " + User);
                }

                // MOVIES IN DATABASE ---> '2' OR '3' etc.
                var moviesInDatabase = _context.Movies.Count();

                // RANDOM R ---> generates a 'System.Random' variable
                Random r = new Random();

                // SET MOVIE ID ---> uses 'r' to pick random movie id inclusive of the first value, exclusive of the second value
                var SetMovieId = r.Next(1,moviesInDatabase + 1);

                // ONE MOVIE ---> movieGame.Models.Movie
                var SetMovieObject = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == SetMovieId);

                SetMovieObject.Title.Intro("this games movie");

                // SESSION MOVIE ID & NAME ---> sets the name and id for the movie that was selected
                HttpContext.Session.SetString("sessionMovieObject", SetMovieObject.ToString());
                HttpContext.Session.SetString("sessionMovieTitle", SetMovieObject.Title);
                HttpContext.Session.SetInt32("sessionMovieId", SetMovieId);

                // NOTE: these are only included for testing purposes
                ViewBag.SessionMovieObject_testing = HttpContext.Session.GetString("sessionMovieObject");
                ViewBag.SessionMovieTitle_testing = HttpContext.Session.GetString("sessionMovieTitle");
                ViewBag.SessionMovieId_testing = HttpContext.Session.GetInt32("sessionMovieId");

                // ViewBag.Movies = _context.Movies.OrderBy(d => d.MovieId).ToList();

                Console.WriteLine("----------------'INITIATE GAME' METHOD COMPLETED---------------");
                Console.WriteLine("----------------'PLAY GAME' PAGE LOADED---------------");

                return View ("PlayGame");
            }


            [HttpGet]
            [Route("/startOver")]

            public IActionResult StartOver ()
            {
                Console.WriteLine("----------------'START OVER' METHOD STARTED---------------");
                // PLAYER ID ---> the PlayerId of the current player (e.g., 8 OR 4 OR 12 etc.)
                var playerId = HttpContext.Session.GetInt32("id");

                // PLAYER --> the name of the current player
                var player = HttpContext.Session.GetString("player");

                // RETRIEVED PLAYER ---> movieGame.Models.Player
                Player RetrievedPlayer = _context.Players.FirstOrDefault(p => p.PlayerId == playerId);
                RetrievedPlayer.Intro("retrieved player");

                var currGamesPlayed = RetrievedPlayer.GamesPlayed;
                currGamesPlayed.Intro("curr games played");

                var newGamesPlayed = currGamesPlayed + 1;
                newGamesPlayed.Intro("new games played");

                RetrievedPlayer.GamesPlayed = newGamesPlayed;

                _context.SaveChanges();

                Console.WriteLine("----------------'START OVER' METHOD COMPLETED---------------");

                return RedirectToAction("InitiateGame");
            }


        #endregion InitiateGame



        #region PlayGame
            // get another clue during game; 10 clues per movie
            [HttpGet]
            [Route("/getClue")]
            public JsonResult GetClue()
            {
                Console.WriteLine("---------------'GET CLUE' METHOD STARTED---------------");

                // SESSION MOVIE ID ---> retrieved to be used set JSON info below
                int? SessionMovieId = HttpContext.Session.GetInt32("sessionMovieId");

                // ONE MOVIE ---> movieGame.Models.Movie
                var oneMovie = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == SessionMovieId);

                // MOVIE ---> returns array of all movies objects
                    // Clues comes back as 'System.Collections.Generic.List`1[movieGame.Models.Clue]'
                Movie movie = new Movie ()
                {
                    Title = oneMovie.Title,
                    Year = oneMovie.Year,
                    Clues = new List<Clue>()
                };

                // CLUES ---> System.Collections.Generic.List`1[movieGame.Models.Clue]
                var Clues = oneMovie.Clues;
                int CountUp = 0;

                foreach(var item in Clues)
                {
                    // ONE CLUE ---> movieGame.Models.Clue
                    Clue oneClue = new Clue
                    {

                        // CLUE DIFFICULTY --> actual #; 1 is highest difficulty
                        ClueDifficulty = Clues[CountUp].ClueDifficulty,

                        // CLUE POINTS --> from 10 to 1
                        CluePoints = Clues[CountUp].CluePoints,

                        // CLUE TEXT ---> actual clue text
                        ClueText = Clues[CountUp].ClueText,

                        // MOVIE ID ---> number of movie clue is associated with
                        MovieId = Clues[CountUp].MovieId,

                        // CLUE ID ---> actual clueId number
                        ClueId = Clues[CountUp].ClueId
                    };

                    movie.Clues.Add(oneClue);
                    CountUp++;
                }

                // MOVIE CLUES ---> System.Collections.Generic.List`1[movieGame.Models.Clue]
                    // Console.WriteLine("MOVIE CLUES ---> " + movie.Clues);

                Console.WriteLine("---------------'GET CLUE' METHOD COMPLETED---------------");

                return Json(movie);
            }


            // guess movie based on the clues given
            [HttpGet]
            [Route("/guessMovie")]
            public JsonResult GuessMovie ()
            {
                Console.WriteLine("---------------'GUESS MOVIE' METHOD STARTED---------------");

                // NOTE: these are only included for testing purposes
                // ViewBag.SessionMovieObject_testing = HttpContext.Session.GetString("sessionMovieObject");
                ViewBag.SessionMovieTitle_testing = HttpContext.Session.GetString("sessionMovieTitle");
                ViewBag.SessionMovieId_testing = HttpContext.Session.GetInt32("sessionMovieId");
                ViewBag.PlayerName = HttpContext.Session.GetString("player");


                // GUESS COUNT ---> if previous guesses, it's guess number; if not, it's blank
                int? guessCount = HttpContext.Session.GetInt32("guesscount");
                    guessCount.Intro("ORIGINAL GUESS COUNT");

                guessCount = guessCount - 1;
                    guessCount.Intro("NEW GUESS COUNT");

                HttpContext.Session.SetInt32("guesscount", (int)guessCount);

                // SESSION MOVIE TITLE ---> retrieves the title of current movie being guessed
                string SessionMovieTitle = HttpContext.Session.GetString("sessionMovieTitle");

                // MOVIEGUESSITEMS ---> System.Collections.ArrayList
                ArrayList MovieGuessItems = new ArrayList();
                MovieGuessItems.Add(SessionMovieTitle);
                MovieGuessItems.Add(guessCount);


                // MOVIEGUESSITEMS ---> System.Collections.ArrayList
                    // MovieGuessItems.Intro("MovieGuessItems");

                Console.WriteLine("---------------'GUESS MOVIE' METHOD COMPLETED---------------");


                return Json(MovieGuessItems);
            }

            List<Clue> _clueList = new List<Clue>();


            [HttpPost]
            [Route("/updatePlayerPoints")]

            public JsonResult UpdatePlayerPoints (Clue clueInfo)
            {
                Console.WriteLine("---------------'UPDATE PLAYER POINTS' METHOD STARTED---------------");

                // PLAYER ID ---> the PlayerId of the current player (e.g., 8 OR 4 OR 12 etc.)
                var playerId = HttpContext.Session.GetInt32("id");

                // PLAYER --> the name of the current player
                var player = HttpContext.Session.GetString("player");

                // SESSION MOVIE --> title of current movie user was guessing
                var sessionMovie = HttpContext.Session.GetString("sessionMovieTitle");

                // SESSION MOVIE ID --> id of current movie user was guessing
                var sessionMovieId = HttpContext.Session.GetInt32("sessionMovieId");

                // MOVIE LIST ---> System.Collections.Generic.List`1[movieGame.Models.Movie]
                _clueList.Add(clueInfo);

                // RETRIEVED PLAYER ---> movieGame.Models.Player
                Player RetrievedPlayer = _context.Players.FirstOrDefault(p => p.PlayerId == playerId);
                RetrievedPlayer.Dig();

                // CURRENT POINTS --> players current points pulled from the database
                var currentPoints = RetrievedPlayer.Points;

                // NEW POINTS ---> the value of the clue the movie was correctly guessed on; retrieved from 'UpdatePlayerPoints' javascript function
                int newPoints = clueInfo.CluePoints;

                // RETRIEVED PLAYER POINT --> adds current points and new points; then saves them to the database
                RetrievedPlayer.Points = newPoints + currentPoints;

                var ExistingJoins = RetrievedPlayer.MoviePlayerJoin;
                ExistingJoins.DigDeep();

                // MPJ --> create new many-to-many of player and movie
                MoviePlayerJoin MPJ = new MoviePlayerJoin
                {
                    PlayerId = (int)playerId,
                    MovieId = (int)sessionMovieId,
                };

                _clueList.LogQuery("clue list");

                _context.Add(MPJ);

                _context.SaveChanges();

                Console.WriteLine("---------------'UPDATE PLAYER POINTS' METHOD COMPLETED---------------");

                return Json(clueInfo);
            }


            // clear session
            [HttpGet]
            [Route("/clear")]
            public IActionResult Clear()
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index");
            }
        #endregion PlayGame



        #region ViewMovieInfo
            // view table of all movies including Id, Title, Description, Year, and list of Clues
            [HttpGet]
            [Route ("allMovies")]
            public IActionResult AllMovies ()
            {
                Console.WriteLine("---------------'ALL MOVIES' METHOD STARTED---------------");

                ViewBag.Movies = _context.Movies.Include(w => w.Clues).OrderBy(d => d.MovieId).ToList();

                Console.WriteLine("---------------'ALL MOVIES' METHOD COMPLETED---------------");
                return View ();
            }


            // get all of a movie's JSON info based on movie title and release year
            [HttpGet]
            [Route("/getMovieJSON")]
            public JsonResult GetMovieJSON(string movieName, int movieYear)
            {
                Console.WriteLine("---------------'GET MOVIE JSON' METHOD STARTED---------------");

                // var exampleURL = "https://www.omdbapi.com/?t=Guardians+of+the+Galaxy+Vol.+2&y=2017&apikey=4514dc2d";

                // var APIqueryTitleYear = "https://www.omdbapi.com/?t=" + "Guardians of the Galaxy Vol. 2" + "&y=" + 2017 + "&apikey=4514dc2d";
                var APIqueryTitleYear = "https://www.omdbapi.com/?t=" + movieName + "&y=" + movieYear + "&apikey=4514dc2d";

                // #region POSTMAN VARIABLES
                    // CLIENT ---> 'RestSharp.RestClient'
                    var client = new RestClient(APIqueryTitleYear);

                    // REQUEST ---> 'RestSharp.RestRequest'
                    var request = new RestRequest(Method.GET);

                    request.AddHeader("Postman-Token", "688d818e-29a5-498f-9c1c-907c7cb77c7f");
                    request.AddHeader("Cache-Control", "no-cache");

                    // RESPONSE ---> 'RestSharp.RestResponse'
                    IRestResponse response = client.Execute(request);
                // #endregion POSTMAN VARIABLES

                // MESSY JSON ---> all movie JSON all garbled up (i.e., not parsed)
                var responseJSON = response.Content;

                // CLEAN JSON ---> all movie JSON presented more cleanly (i.e., it has been parsed)
                JObject movieJSON= JObject.Parse(responseJSON);
                    Console.WriteLine("CLEAN JSON ---> " + movieJSON);

                // MOVIE TITLE ---> the title of the movie
                string MovieTitle = (string)movieJSON["Title"];

                // IMDB ID ---> imdb id number (.e.g, tt3896198)
                string imdbID = (string)movieJSON["imdbID"];

                Console.WriteLine("---------------'GET MOVIE JSON' METHOD COMPLETED---------------");

                return Json(movieJSON);
            }


            // get information about one movie
            [HttpGet]
            [Route("getMovie/{id}")]
            public IActionResult ShowMovie(int id)
            {
                Console.WriteLine("---------------'SHOW MOVIE' METHOD STARTED--------------------");

                // #region DATABASE QUERIES
                    ViewBag.Movies = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == id);

                    // CURRENT MOVIE ---> movieGame.Models.Movie
                    var currentMovie = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == id);

                    // CURRENT MOVIE TITLE ---> the title of the movie pulled from the database
                    var currentMovieTitle = currentMovie.Title;
                    // currentMovieTitle.Intro("current movie title");

                    // CURRENT MOVIE YEAR ---> the release year of the movie pulled from the database
                    var currentMovieYear = currentMovie.Year;
                // #endregion DATABASE QUERIES


                // #region API QUERIES
                    // API QUERY ---> 'Microsoft.AspNetCore.Mvc.JsonResult'
                    var APIquery = GetMovieJSON(currentMovieTitle, currentMovieYear);

                    // API QUERY ---> 'Microsoft.AspNetCore.Mvc.JsonResult'; has a different type than APIquery
                    var jsonResult = Json(APIquery);

                    // JSON STRING ---> all JSON data; movie JSON data is nested within this data;
                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult.Value);

                    // MOVIE JSON ---> JSON STRING but cleaner/parsed
                    JObject movieJSON = JObject.Parse(jsonString);
                    Console.WriteLine(movieJSON);
                // #endregion API QUERIES


                // #region API MOVIE INFO
                    string MovieActors = ViewBag.Actors = (string)movieJSON["Value"]["Actors"];
                    string MovieWriter = (string)movieJSON["Value"]["Writer"];
                    ViewBag.MovieWriter = (string)movieJSON["Value"]["Writer"];

                    string MovieTitle = (string)movieJSON["Value"]["Title"];
                    string MovieDirector = (string)movieJSON["Value"]["Director"];
                    string MovieRating = (string)movieJSON["Value"]["Rated"];
                    string MovieYear = (string)movieJSON["Value"]["Year"];
                    string MoviePoster = ViewBag.MoviePoster = (string)movieJSON["Value"]["Poster"];
                // #endregion API MOVIE INFO

                MovieTitle.Intro("movie title");


                Console.WriteLine("---------------'SHOW MOVIE' METHOD COMPLETED--------------------");
                return View("Movie");
            }

            [HttpGet]
            [Route("test")]
            public IActionResult Test ()
            {
                Console.WriteLine("---------------'TEST METHOD' STARTED---------------");

                // GetBackgroundPosters();

                Console.WriteLine("---------------'TEST METHOD' COMPLETED---------------");
                return View ("test");
            }


            // [HttpGet]
            // private async Task<JObject> GetBackgroundAsync()
            // {
            //     var posters = await GetBackgroundPosters();

            //     return Ok(posters);
            // }


            [HttpGet]
            [Route("getBackgroundPosters")]

            public JsonResult GetBackgroundPosters()
            {
                Console.WriteLine("---------------'GET BACKGROUND POSTERS' METHOD STARTED---------------");

                // MOVIE DB PAGES --> set the number of api pages you want to query; 20 posters per page
                int movieDBpages = 2;

                // TOP MOVIE POSTERS ---> System.Collections.ArrayList
                ArrayList TopMoviePosters = new ArrayList();

                ArrayList AllPosterURLs = new ArrayList();

                for(var x = 1; x <= movieDBpages; x++)
                {

                    var client = new RestClient("https://api.themoviedb.org/3/movie/popular?api_key=1a1ef1aa4b51f19d38e4a7cb134a5699&language=en-US&page=" + x + "&region=us");

                    var request = new RestRequest(Method.GET);

                    request.AddHeader("Postman-Token", "1348f187-6d98-4453-ad72-0f795b433479");
                    request.AddHeader("Cache-Control", "no-cache");

                    // RESPONSE ---> 'RestSharp.RestResponse'
                    IRestResponse response = client.Execute(request);

                    // MESSY JSON ---> all movie JSON all garbled up (i.e., not parsed)

                    var responseJSON = response.Content;

                    // BACKGROUND JSON ---> all movie JSON presented more cleanly (i.e., it has been parsed)
                    JObject backgroundJSON= JObject.Parse(responseJSON);

                    // BACKGROUND COUNT --> 20 (i.e., the number of posters on each api page)
                    var backgroundCount = backgroundJSON["results"].Count();

                    for(var i = 0; i <= backgroundCount - 1; i++)
                    {
                        // ONE POSTER ---> '/inVq3FRqcYIRl2la8iZikYYxFNR.jpg' etc.
                        var onePoster = (string)backgroundJSON["results"][i]["poster_path"];

                        // POSTER URL ---> e.g., https://image.tmdb.org/t/p/w200/wridRvGxDqGldhzAIh3IcZhHT5F.jpg
                        var posterURL = "https://image.tmdb.org/t/p/w200" + onePoster;

                        // ONE TITLE ---> 'Deadpool' OR 'Justice League' etc.
                        var oneTitle = (string)backgroundJSON["results"][i]["title"];

                        TopMoviePosters.Add(onePoster);
                        AllPosterURLs.Add(posterURL);
                    };

                    ViewBag.TopMoviePosters = TopMoviePosters;

                }
                Console.WriteLine("---------------'GET BACKGROUND POSTERS' METHOD COMPLETED---------------");

                return Json(TopMoviePosters);
            }


        #endregion ViewMovieInfo





    }
}