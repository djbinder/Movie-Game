using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace movieGame.Controllers.Game.SingleControllers
{
    public class PlaySingleController : Controller
    {
        private MovieContext _context;
        // private GetMovieInfoController _getMovieInfoController;

        public PlaySingleController (MovieContext context)
        {
            _context = context;
        }


        public Movie GetSessionMovie ()
        {
            int? thisGamesMovieId = HttpContext.Session.GetInt32 ("SessionMovieId");
            Movie thisGamesMovie = _context.Movies.Include (w => w.Clues).SingleOrDefault (x => x.MovieId == thisGamesMovieId);
            Console.WriteLine($"GetSessionMovie() : {thisGamesMovie.Title}");

            Movie movie = new Movie ()
            {
                Title = thisGamesMovie.Title,
                Year = thisGamesMovie.Year,
                Clues = new List<Clue> ()
            };

            foreach(var clue in thisGamesMovie.Clues)
            {
                Console.WriteLine($"Clue: {clue.ClueText}");
            }
            return movie;
        }


        // get another clue during game; 10 clues per movie
        [HttpGet]
        [Route ("GetClue")]
        public JsonResult GetClue ()
        {
            Console.WriteLine("clicked GetClue() button");

            // // retrieved to be used set JSON info below
            // int? thisGamesMovieId = HttpContext.Session.GetInt32 ("SessionMovieId");

            // Movie thisGamesMovie = _context.Movies.Include (w => w.Clues).SingleOrDefault (x => x.MovieId == thisGamesMovieId);
            // Console.WriteLine($"GetClue() : {thisGamesMovie.Title}");

            // // MOVIE ---> returns array of all movies objects
            // Movie movie = new Movie ()
            // {
            //     Title = thisGamesMovie.Title,
            //     Year = thisGamesMovie.Year,
            //     Clues = new List<Clue> ()
            // };

            Movie movie = GetSessionMovie();

            IList<Clue> clues = movie.Clues;
            foreach(var clue in clues)
            {
                Console.WriteLine(clue.ClueText);
            }
            int countUp = 0;

            foreach (var item in clues)
            {
                Clue oneClue = new Clue
                {
                    ClueDifficulty = clues[countUp].ClueDifficulty,
                    CluePoints = clues[countUp].CluePoints,
                    ClueText = clues[countUp].ClueText,
                    MovieId = clues[countUp].MovieId,
                    ClueId = clues[countUp].ClueId
                };
                movie.Clues.Add (oneClue);
                countUp++;
            }

            movie.Dig ();
            return Json (movie);
        }

        // guess movie based on the clues given
        [HttpGet]
        [Route ("GuessMovie")]
        public IActionResult GuessMovie ()
        {
            Console.WriteLine("CLICKED GUESS MOVIE BUTTON");
            // if previous guesses, it's guess number; if not, it's blank
            int? currentGuessCount = HttpContext.Session.GetInt32 ("Guesscount");

            currentGuessCount = currentGuessCount - 1;
            HttpContext.Session.SetInt32 ("Guesscount", (int) currentGuessCount);

            currentGuessCount.Intro ("new guess count");

            // retrieves the title of current movie being guessed
            string thisGamesMovieTitle = HttpContext.Session.GetString ("SessionMovieTitle");

            // System.Collections.ArrayList
            ArrayList movieGuessItems = new ArrayList ();
            movieGuessItems.Add (thisGamesMovieTitle);
            movieGuessItems.Add (currentGuessCount);

            return Json (movieGuessItems);
        }

        [HttpGet]
        [Route ("GetMovieHint")]
        public JsonResult GetMovieHint ()
        {
            List<object> movieHints = HttpContext.Session.GetObjectFromJson<List<object>> ("MovieHints");
            // Hashtable movieHintsTable = HttpContext.Session.GetObjectFromJson<Hashtable> ("MovieHintsTable");
            // Dictionary<string, string> movieHintsDictionary = HttpContext.Session.GetObjectFromJson<Dictionary<string, string>> ("MovieHintsDictionary");
            // List<object> allGroupingTypes = new List<object> ();

            // allGroupingTypes.Add (movieHints);
            // allGroupingTypes.Add (movieHintsTable);
            // allGroupingTypes.Add (movieHintsDictionary);

            return Json (movieHints);
        }

        [HttpGet]
        [Route ("GetClueFromJavaScript")]
        public JsonResult GetClueFromJavaScript (Clue clueInfo)
        {
            string currentClue = clueInfo.ClueText;
            currentClue.Intro ("current clue");
            return Json (clueInfo);
        }

    }
}