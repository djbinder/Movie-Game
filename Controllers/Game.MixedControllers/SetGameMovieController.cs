using System.Collections;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using movieGame.Controllers.Game.MixedControllers;

namespace movieGame.Controllers.Game.MixedControllers
{
    [Route("game")]
    public class SetGameMovieController : Controller
    {
        private static MovieContext _context;

        public SetGameMovieController (MovieContext context)
        {
            _context = context;
        }

        private static GetMovieInfoController _getMovie = new GetMovieInfoController(context: _context);

        public static GetMovieInfoController GetMovie { get => _getMovie; set => _getMovie = value; }

        public string Welcome()
        {
            return "this is the welcome action method";
        }

        public int GetCountOfMoviesInDb ()
        {
            int movieCount = _context.Movies.Count ();
            Console.WriteLine($"there are {movieCount} movies in the database");
            return movieCount;
        }


        // initiate game; select movie that will be guessed
        [HttpGet]
        [Route ("single")]
        public IActionResult InitiateSinglePlayerGame ()
        {
            int currentMovieId = IdentifyThisGamesMovie();
            SetThisGamesMovie (currentMovieId);
            return View ("PlaySingle");
        }

        public int IdentifyThisGamesMovie()
        {
            Console.WriteLine("hit game/single action result");
            int? playerId = HttpContext.Session.GetInt32 ("id");

            User player = _context.Users.Include (m => m.MovieUserJoin).SingleOrDefault (p => p.UserId == playerId);

            // GAMES WON --> how many games has the player won; the next movie served is based off of this
            var playersGamesWon = ViewBag.GamesWon = player.GamesWon;
            int currentMovieId = playersGamesWon + 1;
            int numberOfMoviesInDatabase = GetCountOfMoviesInDb();

            if (currentMovieId > numberOfMoviesInDatabase)
                RedirectPlayersOutOfMovies(numberOfMoviesInDatabase);

            return currentMovieId;
        }

        public IActionResult RedirectPlayersOutOfMovies(int moviesInDb)
        {
            ExtensionsD.Spotlight ("user is caught up; no new movies to guess");
            ViewBag.MovieCount = moviesInDb;
            return View ("NoGame");
        }

        public void SetThisGamesMovie (int movieId)
        {
            int currentMovieId = movieId;

            // var numberOfMoviesInDatabase = GetCountOfMoviesInDb();

            // ONE MOVIE ---> movieGame.Models.Movie
            Movie thisGamesMovie = _context.Movies.Include (w => w.Clues).SingleOrDefault (x => x.MovieId == currentMovieId);
            Console.WriteLine($"SetThisGamesMovie() : {thisGamesMovie.Title}");

            SetMovieInfoInSession(thisGamesMovie);
            SetGameGuessCount ();

            Hints thisMoviesHints = GetMovie.GetMoviesHints(thisGamesMovie);


            // HttpContext.Session.SetInt32 ("SessionMovieYear", thisGamesMovie.Year);
            // HttpContext.Session.SetInt32 ("SessionMovieId", currentMovieId);

            // Hashtable sessionMovieInfoTable = new Hashtable ();
            // sessionMovieInfoTable.Add ("SessionMovieTitle", thisGamesMovie.Title);
            // sessionMovieInfoTable.Add ("SessionMovieYear", thisGamesMovie.Year);
            // HttpContext.Session.SetObjectAsJson ("SessionMovieInfo", sessionMovieInfoTable);
        }

        public void SetMovieInfoInSession(Movie m)
        {
            SetMovieIdInSession(m);
            SetMovieIdInSession(m);
            SetMovieReleaseYearInSession(m);
            SetMovieGenreInSession(m);
            SetMovieDirectorInSession(m);
        }

        public void SetMovieIdInSession(Movie m)
        {
            HttpContext.Session.SetInt32("SessionMovieId", m.MovieId);
        }
        public void SetMovieTitleInSession(Movie m)
        {
            HttpContext.Session.SetString("SessionMovieTitle", m.Title);
        }
        public void SetMovieReleaseYearInSession(Movie m)
        {
            HttpContext.Session.SetInt32("SessionMovieReleaseYear", m.Year);
        }

        public void SetMovieGenreInSession(Movie m)
        {
            HttpContext.Session.SetString("SessionMovieGenre", m.Genre);
        }

        public void SetMovieDirectorInSession(Movie m)
        {
            HttpContext.Session.SetString("SessionMovieDirector", m.Director);
        }

        public void SetGameGuessCount ()
        {
            HttpContext.Session.SetInt32 ("Guesscount", 3);
            int? guessCount = HttpContext.Session.GetInt32 ("Guesscount");
        }

        // public void SetMoviesHints (Movie m)
        // {
        //     Hints hints = GetMovie.GetMoviesHints(m);


        //     var sessionMovieTitle = HttpContext.Session.GetString ("SessionMovieTitle");
        //     Console.WriteLine($"SetThisGamesHints() : {sessionMovieTitle}");
        //     int sessionMovieYear = (int) HttpContext.Session.GetInt32 ("SessionMovieYear");

        //     JObject sessionMovieJObject = GetMovie.GetMovieJSON (sessionMovieTitle, sessionMovieYear);

        //     Hashtable oneMovieHashTable = GetMovie.GetMovieInfoHashTable (sessionMovieJObject);

        //     string movieGenre = ViewBag.movieGenre = oneMovieHashTable["MovieGenre"].ToString ();
        //     string movieDirector = ViewBag.movieDirector = oneMovieHashTable["MovieDirector"].ToString ();
        //     string movieReleaseYear = ViewBag.movieReleaseYear = oneMovieHashTable["MovieReleaseYear"].ToString ();

        //     // option #1
        //     List<object> listOfMoviesHints = new List<object> ();
        //     listOfMoviesHints.Add (movieGenre);
        //     listOfMoviesHints.Add (movieReleaseYear);
        //     listOfMoviesHints.Add (movieDirector);
        //     // these set each of the above options as session objects/json
        //     HttpContext.Session.SetObjectAsJson ("MovieHints", listOfMoviesHints);
        //     Console.WriteLine(listOfMoviesHints[0]);
        //     Console.WriteLine(listOfMoviesHints[1]);
        //     Console.WriteLine(listOfMoviesHints[2]);

        //     // // option #3
        //     // Dictionary<string, string> dictionaryOfMoviesHints = new Dictionary<string, string> ();
        //     // dictionaryOfMoviesHints.Add ("Genre", movieGenre);
        //     // dictionaryOfMoviesHints.Add ("Decade", movieReleaseYear);
        //     // dictionaryOfMoviesHints.Add ("Director", movieDirector);
        //     // HttpContext.Session.SetObjectAsJson ("MovieHintsDictionary", dictionaryOfMoviesHints);
        // }

    }
}