using System;
using System.Collections;
using System.Collections.Generic;       // <--- 'List'
using System.Linq;                      // <--- various db queries (e.g., 'FirstOrDefault', 'OrderBy', etc.)
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Mvc;         // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using Microsoft.EntityFrameworkCore;    // <--- 'include' in db queries
using movieGame.Models;

namespace movieGame.Controllers
{
    public class PlayGameController : Controller
    {
        private MovieContext _context;

        public PlayGameController (MovieContext context) {
            _context = context;
        }

        // SPOTLIGHT' and 'THISMETHOD' EXTENSION METHODS VARIABLES
        String Start = "STARTED";
        // String Complete = "COMPLETED";




        [HttpGet]
        [Route("Instructions")]

        public IActionResult ViewInstructions ()
        {
            Start.ThisMethod();

            // Complete.ThisMethod();
            return View("instructions");
        }


        public Movie GetSessionMovie()
        {
            Start.ThisMethod();

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

            return movie;
        }


        // get another clue during game; 10 clues per movie
        [HttpGet]
        [Route("getClue")]
        public JsonResult GetClue()
        {
            Start.ThisMethod();

            // GetSessionMovie();

            // var _movie = GetSessionMovie();
            // _movie.Intro("_movie");

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

            // Complete.ThisMethod();
            return Json(movie);
        }


        // guess movie based on the clues given
        [HttpGet]
        [Route("guessMovie")]
        public IActionResult GuessMovie ()
        {
            Start.ThisMethod();

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

            // Complete.ThisMethod();
            return Json(MovieGuessItems);
        }


        [HttpGet]
        [Route("GetMovieHint")]

        public JsonResult GetMovieHint ()
        {
            Start.ThisMethod();

            List<object> MovieHints = HttpContext.Session.GetObjectFromJson<List<object>>("MovieHints");

            Hashtable MovieHintsTable = HttpContext.Session.GetObjectFromJson<Hashtable>("MovieHintsTable");

            Dictionary<string, string> MovieHintsDictionary = HttpContext.Session.GetObjectFromJson<Dictionary<string, string>>("MovieHintsDictionary");

            List<object> AllGroupingTypes = new List<object>();

            AllGroupingTypes.Add(MovieHints);
            AllGroupingTypes.Add(MovieHintsTable);
            AllGroupingTypes.Add(MovieHintsDictionary);

            // Complete.ThisMethod();
            return Json(AllGroupingTypes);
        }



        [HttpGet]
        [Route("GetClueFromJavaScript")]
        public JsonResult GetClueFromJavaScript (Clue clueInfo)
        {
            string CurrentClue = clueInfo.ClueText;
            CurrentClue.Intro("current clue");
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

    }
}