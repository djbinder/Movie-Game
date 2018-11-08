using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movieGame.Controllers.MixedControllers;
using movieGame.Infrastructure;


namespace movieGame.Controllers.PlaySingleController
{
    [Route("single")]
    public class PlaySingleController : Controller
    {
        private static MovieContext _context;
        private Helpers _h = new Helpers();


        private static GetMovieInfoController _getMovie = new GetMovieInfoController(context: _context);

        public static GetMovieInfoController GetMovie { get => _getMovie; set => _getMovie = value; }

        public PlaySingleController (MovieContext context)
        {
            _context = context;
        }

        #region MANAGE ROUTING ------------------------------------------------------------

            [HttpGet("")]
            public IActionResult PlaySinglePlayerGame ()
            {
                Console.WriteLine();
                Console.WriteLine("BEGINING NEW GAME");
                Console.WriteLine("--------------------");
                SetThisGamesMovie();
                return View ("PlaySingle");
            }

            public IActionResult RedirectPlayersOutOfMovies(int moviesInDb)
            {
                Console.WriteLine("user is caught up; no new movies to guess");
                ViewBag.MovieCount = moviesInDb;
                return View ("NoGame");
            }

            [HttpGet("game_over_win")]
            public IActionResult ViewSingleGameOverWinPage()
            {
                // Console.WriteLine("ViewSingleGameOverWinPage()");
                string movieTitle = ViewBag.MovieTitle = GetMovieTitleFromSession();

                int? releaseYear = GetMovieReleaseYearFromSession();

                var movie = GetMovie.GetMovieJSON(movieTitle, (int)releaseYear);

                var moviePoster = ViewBag.MoviePoster = movie["Poster"];
                Console.WriteLine($"Poster: {moviePoster}");

                return View("SingleGameOverWin");
            }


            [HttpGet("game_over_loss")]
            public IActionResult ViewSingleGameOverLossPage()
            {
                return View("SingleGameOverLoss");
            }

            public void PrintFromJavaScript()
            {

            }

        #endregion MANAGE ROUTING ------------------------------------------------------------



        #region SET MOVIE ------------------------------------------------------------

            public int GetCountOfMoviesInDb ()
            {
                int movieCount = _context.Movies.Count ();
                return movieCount;
            }

            public int IdentifyThisGamesMovieId()
            {
                int? playerId = HttpContext.Session.GetInt32 ("id");
                User player = _context.Users.Include (m => m.MovieUserJoin).SingleOrDefault (p => p.UserId == playerId);

                // GAMES WON --> how many games has the player won; the next movie served is based off of this
                var playersGamesWon = ViewBag.GamesWon = player.GamesWon;
                int currentMovieId = playersGamesWon + 1;
                int numberOfMoviesInDatabase = GetCountOfMoviesInDb();

                // Console.WriteLine($"IdentifyThisGamesMovie() : player has won {playersGamesWon} games");

                if (currentMovieId > numberOfMoviesInDatabase)
                    RedirectPlayersOutOfMovies(numberOfMoviesInDatabase);

                return currentMovieId;
            }

            public Movie SetThisGamesMovie ()
            {
                int currentMovieId = IdentifyThisGamesMovieId();

                // ONE MOVIE ---> movieGame.Models.Movie
                Movie thisGamesMovie = _context.Movies.Include(c => c.Clues).Include(h => h.Hints).SingleOrDefault(i => i.MovieId == currentMovieId);
                Console.WriteLine($"SetThisGamesMovie() --> {thisGamesMovie.MovieId}. {thisGamesMovie.Title}");

                SetMovieInfoInSession(thisGamesMovie);
                SetGameGuessCountInSession ();

                Hints thisMoviesHints = GetMovie.GetMoviesHints(thisGamesMovie);

                return thisGamesMovie;
            }

        #endregion SET MOVIE ------------------------------------------------------------



        #region SET SESSION VARIABLES ------------------------------------------------------------

            public void SetMovieInfoInSession(Movie m)
            {
                SetMovieIdInSession(m);
                SetMovieTitleInSession(m);
                SetMovieReleaseYearInSession(m);
                SetMovieGenreInSession(m);
                SetMovieDirectorInSession(m);
            }

            public void GetMovieInfoFromSession()
            {
                int? movieId = GetMovieIdFromSession();
                string movieTitle = GetMovieTitleFromSession();
                int? movieYear = GetMovieReleaseYearFromSession();
                string movieGenre = GetMovieGenreFromSession();
                string movieDirector = GetMovieDirectorFromSession();
                int? guessCount = GetGuessCountFromSession();
            }

            public Movie GetThisGamesMovieFromSession ()
            {
                int? movieId = GetMovieIdFromSession();
                Movie thisGamesMovie = _context.Movies.Include(c => c.Clues).Include(h => h.Hints).SingleOrDefault(i => i.MovieId == movieId);
                return thisGamesMovie;
            }

            public void SetMovieIdInSession(Movie m)
            {
                HttpContext.Session.SetInt32("SessionMovieId", m.MovieId);
            }

            [HttpGet("get_movie_id")]
            public int? GetMovieIdFromSession()
            {
                int? movieId = HttpContext.Session.GetInt32("SessionMovieId");
                return movieId;
            }

            public void SetMovieTitleInSession(Movie m)
            {
                HttpContext.Session.SetString("SessionMovieTitle", m.Title);
            }

            [HttpGet("get_movie_title")]
            public string GetMovieTitleFromSession()
            {
                string movieTitle = HttpContext.Session.GetString("SessionMovieTitle");
                return movieTitle;
            }

            public void SetMovieReleaseYearInSession(Movie m)
            {
                HttpContext.Session.SetInt32("SessionMovieReleaseYear", m.Year);
            }

            [HttpGet("get_movie_release_year")]
            public int? GetMovieReleaseYearFromSession()
            {
                int? releaseYear = HttpContext.Session.GetInt32("SessionMovieReleaseYear");
                return releaseYear;
            }

            public void SetMovieGenreInSession(Movie m)
            {
                HttpContext.Session.SetString("SessionMovieGenre", m.Genre);
            }

            [HttpGet("get_movie_genre")]
            public string GetMovieGenreFromSession()
            {
                string genre = HttpContext.Session.GetString("SessionMovieGenre");
                Console.WriteLine($"GetMovieGenre() : {genre}");
                return genre;
            }

            public void SetMovieDirectorInSession(Movie m)
            {
                HttpContext.Session.SetString("SessionMovieDirector", m.Director);
            }

            [HttpGet("get_movie_director")]
            public string GetMovieDirectorFromSession()
            {
                string director = HttpContext.Session.GetString("SessionMovieDirector");
                Console.WriteLine($"GetMovieDirector() : {director}");
                return director;
            }

            public void SetGameGuessCountInSession ()
            {
                HttpContext.Session.SetInt32 ("GuessCount", 3);
                int? guessCount = HttpContext.Session.GetInt32 ("GuessCount");
            }

            [HttpGet("get_guess_count")]
            public int? GetGuessCountFromSession()
            {
                int? guessCount = HttpContext.Session.GetInt32("GuessCount");
                return guessCount;
            }


        #endregion SET SESSION VARIABLES ------------------------------------------------------------



        #region PLAY GAME ------------------------------------------------------------

            [HttpGet ("get_clue")]
            public JsonResult GetClue ()
            {
                // Console.WriteLine("clicked GetClue() button");
                Movie movie = GetThisGamesMovieFromSession();
                List<Clue> cluesList = new List<Clue>();

                int countUp = 0;
                foreach (var item in movie.Clues)
                {
                    // Console.WriteLine($"creating clue {countUp}");
                    Clue oneClue = new Clue
                    {
                        ClueDifficulty = movie.Clues[countUp].ClueDifficulty,
                        CluePoints = movie.Clues[countUp].CluePoints,
                        ClueText = movie.Clues[countUp].ClueText,
                        MovieId = movie.Clues[countUp].MovieId,
                        ClueId = movie.Clues[countUp].ClueId
                    };
                    cluesList.Add (oneClue);
                    countUp++;
                }
                return Json (movie);
            }

            public void PrintClues(List<Clue> clues)
            {
                foreach(var clue in clues)
                {
                    Console.WriteLine(clue.ClueText);
                }
            }


            [HttpGet("guess_movie")]
            public JsonResult GuessMovie ()
            {
                // if previous guesses, it's guess number; if not, it's blank
                int? currentGuessCount = GetGuessCountFromSession();

                currentGuessCount = currentGuessCount - 1;
                HttpContext.Session.SetInt32 ("GuessCount", (int) currentGuessCount);
                // Console.WriteLine($"GuessMovie() : new guess count: {currentGuessCount}");

                string thisGamesMovieTitle = GetMovieTitleFromSession();

                ArrayList movieGuessItems = new ArrayList ();
                movieGuessItems.Add (thisGamesMovieTitle);
                movieGuessItems.Add (currentGuessCount);

                return Json (movieGuessItems);
                // return RedirectToAction("ViewSingleGameOverWinPage");
            }


            [HttpGet("get_clue_from_javascript")]
            public JsonResult GetClueFromJavaScript (Clue clueInfo)
            {
                string currentClue = clueInfo.ClueText;
                // Console.WriteLine($"GetClueFromJavaScript(clue) : {currentClue}");
                return Json (clueInfo);
            }

        #endregion PLAY GAME ------------------------------------------------------------

    }
}