using System;
using System.Collections;
using System.Collections.Generic;       // <--- 'List'
using System.Diagnostics;               // <-- 'StackFrame'
using System.Linq;                      // <--- various db queries (e.g., 'FirstOrDefault', 'OrderBy', etc.)
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Mvc;         // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using Microsoft.EntityFrameworkCore;    // <--- 'include' in db queries
using Newtonsoft.Json;                  // <--- 'JsonConvert'
using Newtonsoft.Json.Linq;             // <--- 'JObject'
using MarkdownLog;
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

        #region 'SPOTLIGHT' and 'THISMETHOD' EXTENSION METHODS VARIABLES
            String Start = "STARTED";
            String Complete = "COMPLETED";
        #endregion

        public int CheckSession()
        {
            int? id = HttpContext.Session.GetInt32("id");
            if(id == null)
            {
                Extensions.Spotlight("this is a new session");
                return 0;
            }
            Extensions.Spotlight("continuation of existing session");

            return (int)id;
        }


        #region InitiateGame
            // view landing page
            [HttpGet]
            [Route ("")]
            public IActionResult Index ()
            {
                Start.ThisMethod();
                CheckSession();
                GetBackgroundPosters();

                ViewBag.ErrorMessage = HttpContext.Session.GetString("message");

                // LEADERS ---> System.Collections.Generic.List`1[movieGame.Models.Player]
                var Leaders = _context.Players.OrderByDescending(t => t.Points).ToList();
                ViewBag.Leaders = Leaders;

                Complete.ThisMethod();
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
                    NameEntered.Intro("no name was entered");
                    Extensions.Spotlight("no name entered");
                    ViewBag.ErrorMessage = "Please Enter a Name to Play!";
                    HttpContext.Session.SetString("message", "Please Enter a Name to Play!");
                    return RedirectToAction("Index");
                }

                else
                {
                    NameEntered.Intro("name entered");
                    // EXISTS ---> movieGame.Models.Player
                    Player exists = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);

                    if(exists == null)
                    {
                        Extensions.Spotlight("USER NAME ENTERED DOES NOT EXIST");
                        HttpContext.Session.SetString("player", NameEntered);

                        Player newPlayer = new Player () {
                            PlayerName = NameEntered,
                            Points = 0,
                            GamesPlayed = 0,
                            Movies = new List<Movie>(),
                            MoviePlayerJoin = new List<MoviePlayerJoin>(),
                        };

                        Extensions.TableIt(newPlayer.PlayerName, newPlayer.Points, newPlayer.GamesPlayed);

                        _context.Add(newPlayer);
                        _context.SaveChanges();

                        // QUERY PLAYER --> QUERY PLAYER ---> movieGame.Models.Player
                        Player queryPlayer = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);
                        HttpContext.Session.SetInt32("id", queryPlayer.PlayerId);
                    }

                    // if the player is not a new player, go this way
                    else
                    {
                        Player currentPlayer = new Player () {
                            PlayerName = exists.PlayerName,
                            Points = exists.Points,
                            GamesPlayed = exists.GamesPlayed,
                            Movies = exists.Movies,
                            MoviePlayerJoin = exists.MoviePlayerJoin,
                        };

                        Extensions.Spotlight("USER NAME ALREADY EXISTS");

                        Extensions.TableIt(currentPlayer.PlayerName, currentPlayer.Points, currentPlayer.GamesPlayed, currentPlayer.Movies);

                        HttpContext.Session.SetString("player", NameEntered);
                        HttpContext.Session.SetInt32("id", exists.PlayerId);
                    }
                }

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
                    int? PlayerId = ViewBag.PlayerId = HttpContext.Session.GetInt32("id");

                    // PLAYERNAME---> retrieves the current players name
                    string PlayerName = ViewBag.PlayerName = HttpContext.Session.GetString("player");
                    PlayerName.Intro("player name");

                    // QUERY PLAYER ---> movieGame.Models.Player
                    Player queryPlayer = _context.Players.FirstOrDefault(p => p.PlayerName == PlayerName);

                    var gamesPlayed = ViewBag.GamesPlayed = queryPlayer.GamesPlayed;

                    var newGamesPlayed = gamesPlayed + 1;

                    queryPlayer.GamesPlayed = newGamesPlayed;
                    _context.SaveChanges();

                    var totalPoints = ViewBag.TotalPoints = queryPlayer.Points;
                #endregion


                #region set movie info
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
                #endregion


                #region set guess count
                    HttpContext.Session.SetInt32("guesscount", 3);
                    int? guessCount = HttpContext.Session.GetInt32("guesscount");
                    guessCount.Intro("guess count");
                #endregion

                // NOTE: these are only included for testing purposes
                ViewBag.SessionMovieObject_testing = HttpContext.Session.GetString("sessionMovieObject");
                ViewBag.SessionMovieTitle_testing = HttpContext.Session.GetString("sessionMovieTitle");
                ViewBag.SessionMovieId_testing = HttpContext.Session.GetInt32("sessionMovieId");

                Complete.ThisMethod();
                return View ("PlayGame");
            }


            [HttpGet]
            [Route("/startOver")]

            public IActionResult StartOver ()
            {
                Start.ThisMethod();

                // PLAYER ID ---> the PlayerId of the current player (e.g., 8 OR 4 OR 12 etc.)
                var playerId = HttpContext.Session.GetInt32("id");

                // PLAYER --> the name of the current player
                var player = @ViewBag.PlayerName = HttpContext.Session.GetString("player");

                // RETRIEVED PLAYER ---> movieGame.Models.Player
                Player RetrievedPlayer = _context.Players.FirstOrDefault(p => p.PlayerId == playerId);
                RetrievedPlayer.Intro("retrieved player");

                var currGamesPlayed = ViewBag.GamesPlayed = RetrievedPlayer.GamesPlayed;
                currGamesPlayed.Intro("curr games played");

                var newGamesPlayed = currGamesPlayed + 1;
                newGamesPlayed.Intro("new games played");

                RetrievedPlayer.GamesPlayed = newGamesPlayed;

                var totalPoints = ViewBag.TotalPoints = RetrievedPlayer.Points;
                totalPoints.Intro("total points");

                _context.SaveChanges();

                Complete.ThisMethod();
                return RedirectToAction("InitiateGame");
            }

        #endregion InitiateGame



        #region PlayGame
            // get another clue during game; 10 clues per movie
            [HttpGet]
            [Route("/getClue")]
            public JsonResult GetClue()
            {
                Start.ThisMethod();

                #region get current movie info
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
                #endregion

                #region get clues
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
                #endregion

                Complete.ThisMethod();
                return Json(movie);
            }


            // guess movie based on the clues given
            [HttpGet]
            [Route("/guessMovie")]
            public JsonResult GuessMovie ()
            {
                Start.ThisMethod();

                // NOTE: these are only included for testing purposes
                ViewBag.SessionMovieTitle_testing = HttpContext.Session.GetString("sessionMovieTitle");
                ViewBag.SessionMovieId_testing = HttpContext.Session.GetInt32("sessionMovieId");
                ViewBag.PlayerName = HttpContext.Session.GetString("player");

                // GUESS COUNT ---> if previous guesses, it's guess number; if not, it's blank
                int? guessCount = HttpContext.Session.GetInt32("guesscount");

                guessCount = guessCount - 1;
                HttpContext.Session.SetInt32("guesscount", (int)guessCount);

                guessCount.Intro("new guess count");

                // SESSION MOVIE TITLE ---> retrieves the title of current movie being guessed
                string SessionMovieTitle = HttpContext.Session.GetString("sessionMovieTitle");

                // MOVIEGUESSITEMS ---> System.Collections.ArrayList
                ArrayList MovieGuessItems = new ArrayList();
                MovieGuessItems.Add(SessionMovieTitle);
                MovieGuessItems.Add(guessCount);

                Complete.ThisMethod();
                return Json(MovieGuessItems);
            }

            List<Clue> _clueList = new List<Clue>();

            [HttpGet]
            [Route("/getClueFromJavaScript")]
            public JsonResult GetClueFromJavaScript (Clue clueInfo)
            {
                // Start.ThisMethod();
                string CurrentClue = clueInfo.ClueText;
                CurrentClue.Intro("current clue");
                // Complete.ThisMethod();
                return Json(clueInfo);
            }


            [HttpPost]
            [Route("/updatePlayerPoints")]
            public JsonResult UpdatePlayerPoints (Clue clueInfo)
            {
                Start.ThisMethod();

                #region retrieve player and movie info
                    // PLAYER ID ---> the PlayerId of the current player (e.g., 8 OR 4 OR 12 etc.)
                    var playerId = HttpContext.Session.GetInt32("id");

                    // PLAYER --> the name of the current player
                    var player = HttpContext.Session.GetString("player");

                    // SESSION MOVIE --> title of current movie user was guessing
                    var sessionMovie = HttpContext.Session.GetString("sessionMovieTitle");

                    // SESSION MOVIE ID --> id of current movie user was guessing
                    var sessionMovieId = HttpContext.Session.GetInt32("sessionMovieId");
                #endregion

                #region set everything needed to update player's points
                    // MOVIE LIST ---> System.Collections.Generic.List`1[movieGame.Models.Movie]
                    _clueList.Add(clueInfo);

                    // RETRIEVED PLAYER ---> movieGame.Models.Player
                    Player RetrievedPlayer = _context.Players.FirstOrDefault(p => p.PlayerId == playerId);

                    // CURRENT POINTS --> players current points pulled from the database
                    var currentPoints = RetrievedPlayer.Points;

                    // NEW POINTS ---> the value of the clue the movie was correctly guessed on; retrieved from 'UpdatePlayerPoints' javascript function
                    int newPoints = clueInfo.CluePoints;
                    newPoints.Intro("new points");

                    // RETRIEVED PLAYER POINT --> adds current points and new points; then saves them to the database
                    RetrievedPlayer.Points = newPoints + currentPoints;
                #endregion

                #region create and save many-to-many relationship
                    var ExistingJoins = RetrievedPlayer.MoviePlayerJoin;

                    // MPJ --> create new many-to-many of player and movie
                    MoviePlayerJoin MPJ = new MoviePlayerJoin
                    {
                        PlayerId = (int)playerId,
                        MovieId = (int)sessionMovieId,
                    };

                    _context.Add(MPJ);
                    _context.SaveChanges();
                #endregion

                Complete.ThisMethod();
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
                Start.ThisMethod();

                ViewBag.Movies = _context.Movies.Include(w => w.Clues).OrderBy(d => d.MovieId).ToList();

                Complete.ThisMethod();
                return View ();
            }


            // get all of a movie's JSON info based on movie title and release year
            [HttpGet]
            [Route("/getMovieJSON")]
            public JsonResult GetMovieJSON(string movieName, int movieYear)
            {
                Start.ThisMethod();

                // var APIqueryTitleYear = "https://www.omdbapi.com/?t=" + "Guardians of the Galaxy Vol. 2" + "&y=" + 2017 + "&apikey=4514dc2d";
                var APIqueryTitleYear = "https://www.omdbapi.com/?t=" + movieName + "&y=" + movieYear + "&apikey=4514dc2d";

                #region POSTMAN VARIABLES
                    // CLIENT ---> 'RestSharp.RestClient'
                    var client = new RestClient(APIqueryTitleYear);

                    // REQUEST ---> 'RestSharp.RestRequest'
                    var request = new RestRequest(Method.GET);

                    request.AddHeader("Postman-Token", "688d818e-29a5-498f-9c1c-907c7cb77c7f");
                    request.AddHeader("Cache-Control", "no-cache");

                    // RESPONSE ---> 'RestSharp.RestResponse'
                    IRestResponse response = client.Execute(request);
                #endregion POSTMAN VARIABLES

                // MESSY JSON ---> all movie JSON all garbled up (i.e., not parsed)
                var responseJSON = response.Content;

                // MOVIE JSON ---> all movie JSON presented more cleanly (i.e., it has been parsed)
                JObject movieJSON= JObject.Parse(responseJSON);

                // MOVIE TITLE ---> the title of the movie
                string MovieTitle = (string)movieJSON["Title"];

                // IMDB ID ---> imdb id number (.e.g, tt3896198)
                string imdbID = (string)movieJSON["imdbID"];

                Complete.ThisMethod();
                return Json(movieJSON);
            }


            // get information about one movie
            [HttpGet]
            [Route("getMovie/{id}")]
            public IActionResult ShowMovie(int id)
            {
                Start.ThisMethod();

                #region DATABASE QUERIES
                    ViewBag.Movies = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == id);

                    // CURRENT MOVIE ---> movieGame.Models.Movie
                    var currentMovie = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == id);

                    // CURRENT MOVIE TITLE ---> the title of the movie pulled from the database
                    var currentMovieTitle = currentMovie.Title;
                    // currentMovieTitle.Intro("current movie title");

                    // CURRENT MOVIE YEAR ---> the release year of the movie pulled from the database
                    var currentMovieYear = currentMovie.Year;
                #endregion DATABASE QUERIES

                #region API QUERIES
                    // API QUERY ---> 'Microsoft.AspNetCore.Mvc.JsonResult'
                    var APIquery = GetMovieJSON(currentMovieTitle, currentMovieYear);

                    // API QUERY ---> 'Microsoft.AspNetCore.Mvc.JsonResult'; has a different type than APIquery
                    var jsonResult = Json(APIquery);

                    // JSON STRING ---> all JSON data; movie JSON data is nested within this data;
                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult.Value);

                    // MOVIE JSON ---> JSON STRING but cleaner/parsed
                    JObject movieJSON = JObject.Parse(jsonString);
                #endregion API QUERIES

                #region API MOVIE INFO
                    string MovieTitle = (string)movieJSON["Value"]["Title"];
                    string MovieRating = (string)movieJSON["Value"]["Rated"];
                    string MovieYear = (string)movieJSON["Value"]["Year"];

                    string MovieActors = ViewBag.Actors = (string)movieJSON["Value"]["Actors"];
                    string MovieWriter = ViewBag.MovieWriter = (string)movieJSON["Value"]["Writer"];
                    string MovieDirector = ViewBag.MovieDirector = (string)movieJSON["Value"]["Director"];
                    string MovieGenre = ViewBag.MovieGenre = (string)movieJSON["Value"]["Genre"];
                    string MoviePoster = ViewBag.MoviePoster = (string)movieJSON["Value"]["Poster"];
                #endregion API MOVIE INFO

                Complete.ThisMethod();
                return View("Movie");
            }

            [HttpGet]
            [Route("/getActorImage")]
            public JsonResult GetActorImage(string actorName)
            {
                Start.ThisMethod();

                #region API REQUEST INFO
                    // var APIkey = "4cbdf8913d9628d339184a127d136d68";
                    var PersonsName = actorName;
                    var client = new RestClient("https://api.themoviedb.org/3/search/person?api_key=1a1ef1aa4b51f19d38e4a7cb134a5699&language=en-US&query=" + PersonsName + "&page=1&include_adult=false");
                    var request = new RestRequest(Method.GET);
                    request.AddHeader("Postman-Token", "dbfd1014-ebcb-4c79-80eb-1b0eac81a888");
                    request.AddHeader("Cache-Control", "no-cache");
                    IRestResponse response = client.Execute(request);

                    client.Intro("client");
                    Extensions.TableIt(request, response);
                #endregion

                #region JSON
                    var responseJSON = response.Content;
                    JObject actorJSON = JObject.Parse(responseJSON);
                #endregion

                #region QUERIES
                    string ActorName = ViewBag.ActorName = (string)actorJSON["results"][0]["name"];
                    int ActorId = ViewBag.ActorId = (int)actorJSON["results"][0]["id"];
                    ActorName.Intro("actor name");
                    ActorId.Intro("actor id");

                    string PictureBaseURL = "https://image.tmdb.org/t/p/w";

                    // these are picture sizes
                    int PictureSizeLarge = 400;
                    int PictureSizeMedium = 300;
                    int PictureSizeSmall = 200;
                    int PictureSizeSmallest = 92;

                    string ActorPictureURL = (string)actorJSON["results"][0]["profile_path"];

                    string ActorPicLarge = ViewBag.ActorPicLarge = PictureBaseURL + PictureSizeLarge + ActorPictureURL;
                    string ActorPicMedium = ViewBag.ActorPicMedium = PictureBaseURL + PictureSizeMedium + ActorPictureURL;
                    string ActorPicSmall = ViewBag.ActorPicSmall = PictureBaseURL + PictureSizeSmall + ActorPictureURL;
                    string ActorPicSmallest = ViewBag.ActorPicSmallest = PictureBaseURL + PictureSizeSmallest + ActorPictureURL;

                    ActorPicSmallest.Intro("smallest");
                #endregion

                Complete.ThisMethod();
                return Json (actorJSON);
            }

            [HttpGet]
            [Route("test")]
            public IActionResult Test ()
            {
                Start.ThisMethod();

                GetActorImage("Tom Cruise");

                Complete.ThisMethod();
                return View ("test");
            }


            [HttpGet]
            [Route("getBackgroundPosters")]
            public JsonResult GetBackgroundPosters()
            {
                Start.ThisMethod();

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
                Complete.ThisMethod();
                return Json(TopMoviePosters);
            }
        #endregion ViewMovieInfo
    }
}