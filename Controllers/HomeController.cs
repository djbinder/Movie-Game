using System;
using System.Collections;
using System.Collections.Generic;       // <--- 'List'
using System.Linq;                      // <--- various db queries (e.g., 'FirstOrDefault', 'OrderBy', etc.)
using System.Windows;
using System.Windows.Forms;
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Mvc;         // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using Microsoft.EntityFrameworkCore;    // <--- 'include' in db queries
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
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



        #region MongoDB
            private MongoClient _mongoClient;


            private MongoClient CreateClient()
            {
                return new MongoClient(
                    new MongoClientSettings
                    {
                        ClusterConfigurator = builder => {
                            builder.Subscribe(new SingleEventSubscriber<CommandStartedEvent>(CmdStartHandlerForFindCommand));
                            builder.Subscribe(new SingleEventSubscriber<CommandSucceededEvent>(CmdSuccessHandlerForFindCommand));
                        }
                    });
            }

            private void WithCoreEventsSubscription(Movie movie)
            {
                MongoClient client = CreateClient();
                IMongoDatabase db = client.GetDatabase("movies");

                IMongoCollection<Movie> MovieCollection = db.GetCollection<Movie>("movies");
                MovieCollection.InsertOne(movie);

                //Here our "CmdStartHandlerForFindCommand" will be triggered for Find request
                //and as response our "CmdSuccessHandlerForFindCommand" will be triggered as well
                movie = MovieCollection.Find(FilterDefinition<Movie>.Empty).Single();
            }

            private void CmdStartHandlerForFindCommand(CommandStartedEvent cmdStart)
            {
                if (cmdStart.CommandName == "find")
                {
                    WriteToConsole(cmdStart.Command, "request");
                }
            }

            private void CmdSuccessHandlerForFindCommand(CommandSucceededEvent cmdSuccess)
            {
                if (cmdSuccess.CommandName == "find")
                {
                    WriteToConsole(cmdSuccess.Reply, "response");
                }
            }


            private void WriteToConsole(BsonDocument data, string type)
            {
                Console.WriteLine($"--------------- Find {type} ---------------");
                Console.WriteLine(data.ToJson(
                    new JsonWriterSettings
                    {
                    Indent = true
                    }));
            }
        #endregion MongoDB



        #region InitiateGame
            // view landing page
            [HttpGet]
            [Route ("")]
            public IActionResult Index ()
            {
                Console.WriteLine("---------------'INDEX METHOD' STARTED---------------");

                HttpContext.Session.SetString("player", "null");
                HttpContext.Session.SetInt32("id", 0);

                Console.WriteLine("---------------'INDEX METHOD' COMPLETED---------------");

                return View ();
            }


            // enter name on index page; set session name and id; redirect to 'INITIATE GAME' method
            [HttpPost]
            [Route ("enterName")]
            public IActionResult EnterName (string NameEntered)
            {
                Console.WriteLine("---------------'ENTER NAME' METHOD STARTED---------------");

                Console.WriteLine("NAME ENTERED ---> " + NameEntered);

                // EXISTS ---> movieGame.Models.Player
                Player exists = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);
                    int existsID = exists.PlayerId;
                    existsID.Intro("EXISTS ID");


                if(exists == null)
                {
                    Console.WriteLine("IF ---> " + "NEW USER NAME ENTERED");
                    HttpContext.Session.SetString("player", NameEntered);
                    HttpContext.Session.SetInt32("id", existsID);

                    Player newPlayer = new Player () {
                        PlayerName = NameEntered,
                        Points = 0,
                        GamesPlayed = 0,
                    };

                    _context.Add(newPlayer);
                    _context.SaveChanges();
                }

                else
                {
                    // ELSE ---> USER NAME ALREADY EXISTS
                    Console.WriteLine("ELSE ---> " + "USER NAME ALREADY EXISTS");

                    HttpContext.Session.SetString("player", NameEntered);
                    HttpContext.Session.SetInt32("id", existsID);

                    exists.GamesPlayed = exists.GamesPlayed + 1;

                    _context.SaveChanges();

                    // NEW GAMES PLAYED ---> '1' OR '2' etc.
                    exists.GamesPlayed.Intro("NEW GAMES PLAYED");
                }

                Console.WriteLine("---------------'ENTER NAME' METHOD COMPLETED---------------");

                return RedirectToAction("initiateGame");
            }


            // initiate game; select movie that will be guessed
            [HttpGet]
            [Route ("initiateGame")]
            public IActionResult InitiateGame ()
            {
                Console.WriteLine("----------------'INITIATE GAME' METHOD STARTED---------------");

                // PLAYER ID ---> '1' OR '2' etc.
                int? PlayerId = ViewBag.PlayerId = HttpContext.Session.GetInt32("id");


                // PLAYERNAME---> retrieves the current players name
                string PlayerName = ViewBag.PlayerName = HttpContext.Session.GetString("player");

                if(CheckSession() == 0)
                {
                    Console.WriteLine("CHECK SESSION NONE ---> " + User);
                }

                else
                {
                    // CHECK SESSION YES ---> System.Security.Claims.ClaimsPrincipal
                    Console.WriteLine("CHECK SESSION YES ---> " + User);
                }

                // MOVIES IN DATABASE ---> '2' OR '3' etc.
                var moviesInDatabase = _context.Movies.Count();



                // RANDOM R ---> generates a 'System.Random' variable
                Random r = new Random();

                // SET MOVIE ID ---> uses 'r' to pick random movie id inclusive of the first value, exclusive of the second value
                var SetMovieId = r.Next(1,3);

                // ONE MOVIE ---> movieGame.Models.Movie
                var SetMovieObject = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == SetMovieId);

                // VIEWBAG.SETMOVIE ---> used to send information to the HTML page
                // ViewBag.SetMovieObject = SetMovieObject;

                // SESSION MOVIE ID & NAME ---> sets the name and id for the movie that was selected
                HttpContext.Session.SetString("sessionMovieObject", SetMovieObject.ToString());
                HttpContext.Session.SetString("sessionMovieTitle", SetMovieObject.Title);
                HttpContext.Session.SetInt32("sessionMovieId", SetMovieId);

                // NOTE: these are only included for testing purposes
                ViewBag.SessionMovieObject_testing = HttpContext.Session.GetString("sessionMovieObject");
                ViewBag.SessionMovieTitle_testing = HttpContext.Session.GetString("sessionMovieTitle");
                ViewBag.SessionMovieId_testing = HttpContext.Session.GetInt32("sessionMovieId");

                // GUESS RESPONSE ---> sends a blank string as a placeholder; the 'response' tells the user whether they were right or wrong
                ViewBag.SessionMovieObject = "";
                ViewBag.GuessResponse = "";
                ViewBag.RemainingGuesses = ("YOU HAVE " + 3 + " REMAINING GUESSES");

                ViewBag.Movies = _context.Movies.OrderBy(d => d.MovieId).ToList();

                Console.WriteLine("----------------'INITIATE GAME' METHOD COMPLETED---------------");
                Console.WriteLine("----------------'PLAY GAME' PAGE LOADED---------------");

                return View ("playgame");
            }
        #endregion InitiateGame



        #region PlayGame
        // get another clue during game; 10 clues per movie
            [HttpGet]
            [Route("/getClue")]
            public JsonResult GetClue()
            {
                Console.WriteLine("---------------'GET CLUE' METHOD STARTED---------------");

                // ATTEMPT ---> if previous attempts, it's attempt number; if not, it's blank
                int? attempt = HttpContext.Session.GetInt32("attempt");
                    attempt.Intro("attempt");

                    // ATTEMPT ---> if first attempt, set 'attempt' to 1; if not, increment 'attempt' by 1
                    if(attempt == null)
                    {
                        HttpContext.Session.SetInt32("attempt", 1);
                    }
                    else
                    {
                        attempt += 1;
                        HttpContext.Session.SetInt32("attempt", (int)attempt);
                    }

                    attempt.Intro("attempt");

                // SESSION MOVIE ID ---> retrieved to be used set JSON info below
                int? SessionMovieId = HttpContext.Session.GetInt32("sessionMovieId");

                // ONE MOVIE ---> movieGame.Models.Movie
                var oneMovie = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == SessionMovieId);

                // MOVIE ---> returns array of all movies objects
                    // Clues comes back as 'System.Collections.Generic.List`1[movieGame.Models.Clue]'
                Movie movie = new Movie ()
                {
                    Title = oneMovie.Title,
                    Description = oneMovie.Description,
                    Director = oneMovie.Director,
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

                CountUp.Intro("COUNT UP");

                // MOVIE CLUES ---> System.Collections.Generic.List`1[movieGame.Models.Clue]
                    Console.WriteLine("MOVIE CLUES ---> " + movie.Clues);

                Console.WriteLine("---------------'GET CLUE' METHOD COMPLETED---------------");

                return Json(movie);
            }


            // guess movie based on the clues given
            [HttpGet]
            [Route("guessMovie")]
            public JsonResult GuessMovie (string movieGuessInput)
            // public IActionResult GuessMovie (string movieGuessInput, string movieGuessSelect)
            {
                Console.WriteLine("---------------'GUESS MOVIE' METHOD STARTED---------------");

                // NOTE: these are only included for testing purposes
                // ViewBag.SessionMovieObject_testing = HttpContext.Session.GetString("sessionMovieObject");
                ViewBag.SessionMovieTitle_testing = HttpContext.Session.GetString("sessionMovieTitle");
                ViewBag.SessionMovieId_testing = HttpContext.Session.GetInt32("sessionMovieId");
                ViewBag.PlayerName = HttpContext.Session.GetString("player");


                // GUESS COUNT ---> if previous guesses, it's guess number; if not, it's blank
                int? guessCount = HttpContext.Session.GetInt32("guesscount");
                    // guessCount.Intro("ORIGINAL GUESS COUNT");

                    // GUESS COUNT ---> if first attempt, set 'guessCount' to 1; if not, increment 'guessCount' by 1
                    if(guessCount == null)
                    {
                        HttpContext.Session.SetInt32("guesscount", 2);
                        guessCount = 2;
                            Console.WriteLine("NULL GUESS COUNT ---> " + guessCount);

                    }
                    else
                    {
                        guessCount -= 1;

                        if(guessCount == 0)
                        {
                            Console.WriteLine("GUESS COUNT ZERO ---> " + guessCount);
                            Console.WriteLine("GAME OVER----------------");
                            // string GameOverMessage = "Game Over";
                            // MessageBox.Show(text: GameOverMessage);

                        }

                        else {

                        HttpContext.Session.SetInt32("guesscount", (int)guessCount);
                            Console.WriteLine("SUCCESS GUESS COUNT ---> " + guessCount);
                        }
                    }

                // SESSION MOVIE TITLE ---> retrieves the title of current movie being guessed
                string SessionMovieTitle = HttpContext.Session.GetString("sessionMovieTitle");
                    Console.WriteLine("SESSION MOVIE TITLE ---> " + SessionMovieTitle);

                // SESSION MOVIE ID ---> retrieved to be used set JSON info below
                int? SessionMovieId = HttpContext.Session.GetInt32("sessionMovieId");

                // MOVIE GUESS ---> gets the text of the guess inputed in HTML form
                string movieGuess = movieGuessInput;
                    movieGuess.Intro("controller movie guess");




                // NOTE: viewbag.guess stuff
                if(movieGuess == SessionMovieTitle)
                {
                    Console.WriteLine("CORRECT GUESS ---> " + movieGuess + " and " + SessionMovieTitle);
                }
                else
                {
                    Console.WriteLine("WRONG GUESS ---> " + movieGuess + " and " + SessionMovieTitle);
                }


                // ViewBag.Movies = _context.Movies.OrderBy(d => d.MovieId).ToList();


                // MOVIEGUESSITEMS ---> System.Collections.ArrayList
                ArrayList MovieGuessItems = new ArrayList();
                MovieGuessItems.Add(SessionMovieTitle);
                MovieGuessItems.Add(guessCount);

                // MOVIEGUESSITEMS ---> System.Collections.ArrayList
                MovieGuessItems.Intro("MovieGuessItems");

                foreach(var item in MovieGuessItems)
                {
                    Console.WriteLine("ITEM ---> " + item);
                }

                Console.WriteLine("---------------'GUESS MOVIE' METHOD COMPLETED---------------");


                return Json(MovieGuessItems);
            }


            // clear session
            [HttpGet]
            [Route("/clear")]
            public IActionResult Clear()
            {
                HttpContext.Session.Clear();
                return RedirectToAction("InitiateGame");
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
                // #endregion API QUERIES


                // #region API MOVIE INFO
                    string MovieActors = ViewBag.Actors = (string)movieJSON["Value"]["Actors"];
                    string MovieWriter = (string)movieJSON["Value"]["Writer"];
                    ViewBag.MovieWriter = (string)movieJSON["Value"]["Writer"];

                    string MovieTitle = (string)movieJSON["Value"]["Title"];
                    string MovieDirector = (string)movieJSON["Value"]["Director"];
                    string MovieRating = (string)movieJSON["Value"]["Rated"];
                    string MovieYear = (string)movieJSON["Value"]["Year"];
                // #endregion API MOVIE INFO


                Console.WriteLine("---------------'SHOW MOVIE' METHOD COMPLETED--------------------");
                return View("Movie");
            }
        #endregion ViewMovieInfo


        #region Add new movie and clues
            // go to the page to add new movie and clues
            [HttpGet]
            [Route("addMoviePage")]
            public IActionResult AddMoviePage ()
            {
                Console.WriteLine("---------------'ADD MOVIE PAGE' METHOD STARTED---------------");

                Console.WriteLine("---------------'ADD MOVIE PAGE' METHOD COMPLETED---------------");
                return View ("generate");
            }


            // add a new movie
            [HttpPost]
            [Route ("addmovie")]
            public IActionResult AddMovie (Movie movie)
            {
                Movie newMovie = new Movie {

                    Title = "Goodfellas",

                    Description = "The story of Henry Hill and his life in the mob, covering his relationship with his wife Karen Hill and his mob partners Jimmy Conway and Tommy DeVito in the Italian-American crime syndicate. ",

                    Director = "Martin Scorsese",

                    Year = 1990,
                };

                // var MongoClient = new MongoClient();


                // newMovie.Dump();
                // Console.WriteLine(typeof(Movie));

                _context.Add(newMovie);
                _context.SaveChanges();

                return View("Index");
            }


            // add clues to a movie
            [HttpPost]
            [Route("addClue")]
            public IActionResult AddClue()
            {
                Console.WriteLine("---------------ADD CLUE METHOD EXECUTED---------------");

                int MovieId = 2;

                String Xclue1 =  "Helicopter";
                String Xclue2 =  "Trunk";
                String Xclue3 =  "Shovel";
                String Xclue4 =  "Toupee";
                String Xclue5 =  "Garlic";
                String Xclue6 =  "Coke";
                String Xclue7 =  "Fur coat";
                String Xclue8 =  "Shine box";
                String Xclue9 =  "Lufthansa";
                String Xclue10 =  "Henry Hill";

                #region 10clues
                    Clue clue = new Clue
                    {
                        MovieId = MovieId,
                        ClueDifficulty = 10,
                        CluePoints = 10,
                        ClueText = Xclue1
                    };

                    _context.Add(clue);
                    _context.SaveChanges();


                    Clue clue2 = new Clue
                    {
                        MovieId = MovieId,
                        ClueDifficulty = 9,
                        CluePoints = 9,
                        ClueText = Xclue2

                    };

                    _context.Add(clue2);
                    _context.SaveChanges();


                    Clue clue3 = new Clue
                    {
                        MovieId = MovieId,
                        ClueDifficulty = 8,
                        CluePoints = 8,
                        ClueText = Xclue3

                    };

                    _context.Add(clue3);
                    _context.SaveChanges();


                    Clue clue4 = new Clue
                    {
                        MovieId = MovieId,
                        ClueDifficulty = 7,
                        CluePoints = 7,
                        ClueText = Xclue4
                    };

                    _context.Add(clue4);
                    _context.SaveChanges();


                    Clue clue5 = new Clue
                    {
                        MovieId = MovieId,
                        ClueDifficulty = 6,
                        CluePoints = 6,
                        ClueText = Xclue5
                    };

                    _context.Add(clue5);
                    _context.SaveChanges();


                    Clue clue6 = new Clue
                    {
                        MovieId = MovieId,
                        ClueDifficulty = 5,
                        CluePoints = 5,
                        ClueText = Xclue6
                    };

                    _context.Add(clue6);
                    _context.SaveChanges();

                    Clue clue7 = new Clue
                    {
                        MovieId = MovieId,
                        ClueDifficulty = 4,
                        CluePoints = 4,
                        ClueText = Xclue7
                    };

                    _context.Add(clue7);
                    _context.SaveChanges();


                    Clue clue8 = new Clue
                    {
                        MovieId = MovieId,
                        ClueDifficulty = 3,
                        CluePoints = 3,
                        ClueText = Xclue8
                    };

                    _context.Add(clue8);
                    _context.SaveChanges();


                    Clue clue9 = new Clue
                    {
                        MovieId = MovieId,
                        ClueDifficulty = 2,
                        CluePoints = 2,
                        ClueText = Xclue9
                    };

                    _context.Add(clue9);
                    _context.SaveChanges();


                    Clue clue10 = new Clue
                    {
                        MovieId = MovieId,
                        ClueDifficulty = 1,
                        CluePoints = 1,
                        ClueText = Xclue10
                    };

                    _context.Add(clue10);
                    _context.SaveChanges();
                #endregion

                return View ("Index");
            }
        #endregion Add new movie and clues







        #region SetDropDownList

        //     private IEnumerable<SelectListItem> GetMovies()
        //     {
        //         // var dbMovieNames = new DbUserRoles();

        //         var allMovies = _context.GetMovies().Select(x => new SelectListItem
        //         {
        //             Value = x.MovieId.ToString();
        //             Text = x.MovieTitle;
        //         });

        //         Movies.Include(w => w.Clues).OrderBy(d => d.MovieId).ToList();

        //         // var roles = dbMovieNames
        //         //             .GetRoles()
        //         //             .Select(x =>
        //         //                     new SelectListItem
        //         //                         {
        //         //                             Value = x.UserRoleId.ToString(),
        //         //                             Text = x.UserRole
        //         //                         });

        //         return new SelectList(allMovies, "Value", "Text");
        //     }

        //     public ActionResult AddNewUser()
        //     {
        //         var model = new MovieView
        //                         {
        //                             MovieNameItems = GetMovies()
        //                         };
        //         return View(model);
        //     }

        #endregion SetDropDownList



    }
}