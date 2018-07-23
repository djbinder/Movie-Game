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
    public class PlaySingleController : Controller
    {
        private MovieContext _context;
        // private GetMovieInfoController _getMovieInfoController;

        public PlaySingleController (MovieContext context) {
            _context = context;
        }

        String Start = "STARTED";
        String Complete = "COMPLETED";

        // public GetMovieInfoController GetMovieInfoController {
        //     get => _getMovieInfoController;
        //     set => _getMovieInfoController = value;
        // }


        public Movie GetSessionMovie()
        {
            Start.ThisMethod();

            // SESSION MOVIE ID ---> retrieved to be used set JSON info below
            int? ThisGamesMovieId = HttpContext.Session.GetInt32("sessionMovieId");

            // ONE MOVIE ---> movieGame.Models.Movie
            var ThisGamesMovie = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == ThisGamesMovieId);

            // MOVIE ---> returns array of all movies objects
                // Clues comes back as 'System.Collections.Generic.List`1[movieGame.Models.Clue]'
            Movie movie = new Movie ()
            {
                Title = ThisGamesMovie.Title,
                Year = ThisGamesMovie.Year,
                Clues = new List<Clue>()
            };

            return movie;
        }


        // get another clue during game; 10 clues per movie
        [HttpGet]
        [Route("GetClue")]
        public JsonResult GetClue()
        {
            Start.ThisMethod();

            #region get current movie info
                // retrieved to be used set JSON info below
                int? ThisGamesMovieId = HttpContext.Session.GetInt32("SessionMovieId");

                // movieGame.Models.Movie
                var ThisGamesMovie = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == ThisGamesMovieId);

                // MOVIE ---> returns array of all movies objects
                Movie movie = new Movie ()
                {
                    Title = ThisGamesMovie.Title,
                    Year = ThisGamesMovie.Year,
                    Clues = new List<Clue>()
                };
            #endregion

            #region get clues
                // CLUES ---> System.Collections.Generic.List`1[movieGame.Models.Clue]
                var Clues = ThisGamesMovie.Clues;
                int CountUp = 0;

                foreach(var item in Clues)
                {
                    // ONE CLUE ---> movieGame.Models.Clue
                    Clue OneClue = new Clue
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

                    movie.Clues.Add(OneClue);
                    CountUp++;
                }
            #endregion

            movie.Dig();

            Complete.ThisMethod();
            return Json(movie);
        }


        // guess movie based on the clues given
        [HttpGet]
        [Route("GuessMovie")]
        public IActionResult GuessMovie ()
        {
            Start.ThisMethod();

            // if previous guesses, it's guess number; if not, it's blank
            int? CurrentGuessCount = HttpContext.Session.GetInt32("Guesscount");

            CurrentGuessCount = CurrentGuessCount - 1;
            HttpContext.Session.SetInt32("Guesscount", (int)CurrentGuessCount);

            CurrentGuessCount.Intro("new guess count");

            // retrieves the title of current movie being guessed
            string ThisGamesMovieTitle = HttpContext.Session.GetString("SessionMovieTitle");

            // System.Collections.ArrayList
            ArrayList MovieGuessItems = new ArrayList();
            MovieGuessItems.Add(ThisGamesMovieTitle);
            MovieGuessItems.Add(CurrentGuessCount);

            // Complete.ThisMethod();
            return Json(MovieGuessItems);
        }


        [HttpGet]
        [Route("GetMovieHint")]

        public JsonResult GetMovieHint ()
        {
            // Start.ThisMethod();

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
            // Start.ThisMethod();

            string CurrentClue = clueInfo.ClueText;
            CurrentClue.Intro("current clue");

            // Complete.ThisMethod();
            return Json(clueInfo);
        }



    }
}