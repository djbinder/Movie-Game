using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movieGame.Controllers.Game.MixedControllers;
using movieGame.Infrastructure;

namespace movieGame.Controllers.Game.SingleControllers
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

            [HttpGet]
            [Route ("")]
            public IActionResult PlaySinglePlayerGame ()
            {
                SetThisGamesMovie();
                return View ("PlaySingle");
            }

            public IActionResult RedirectPlayersOutOfMovies(int moviesInDb)
            {
                Console.WriteLine("user is caught up; no new movies to guess");
                ViewBag.MovieCount = moviesInDb;
                return View ("NoGame");
            }

        #endregion MANAGE ROUTING ------------------------------------------------------------



        #region SET MOVIE ------------------------------------------------------------

            public int GetCountOfMoviesInDb ()
            {
                int movieCount = _context.Movies.Count ();
                // Console.WriteLine($"GetCountOfMoviesInDb() : {movieCount} movies database");
                return movieCount;
            }

            public int IdentifyThisGamesMovieId()
            {
                Console.WriteLine("identifying this games movie id");
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
                // Console.WriteLine("setting this games movie");
                int currentMovieId = IdentifyThisGamesMovieId();

                // ONE MOVIE ---> movieGame.Models.Movie
                Movie thisGamesMovie = _context.Movies.Include(c => c.Clues).Include(h => h.Hints).SingleOrDefault(i => i.MovieId == currentMovieId);
                // Console.WriteLine($"SetThisGamesMovie() : movie is {thisGamesMovie.Title}");

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

            // public JsonResult SetMovieInfoJObject(List<object> movieInfoList, string key)
            // {
            //     JsonResult movieJsonResult = _h.SetObjectAsJson(session: _session, key: key, value: movieInfoList);
            // }

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
            public IActionResult GuessMovie ()
            {
                Console.WriteLine("CLICKED GUESS MOVIE BUTTON");
                // if previous guesses, it's guess number; if not, it's blank
                int? currentGuessCount = GetGuessCountFromSession();
                Console.WriteLine($"GuessMovie() : Current guess count is {currentGuessCount}");

                currentGuessCount = currentGuessCount - 1;
                HttpContext.Session.SetInt32 ("GuessCount", (int) currentGuessCount);

                // retrieves the title of current movie being guessed
                string thisGamesMovieTitle = GetMovieTitleFromSession();
                Console.WriteLine($"guess_movie Movie Title: {thisGamesMovieTitle}");

                // System.Collections.ArrayList
                ArrayList movieGuessItems = new ArrayList ();
                movieGuessItems.Add (thisGamesMovieTitle);
                movieGuessItems.Add (currentGuessCount);

                return Json (movieGuessItems);
            }


            // [HttpGet("get_movie_hint")]
            // // [Route ("get_movie_hint")]
            // public JsonResult GetMovieHint ()
            // {
            //     Console.WriteLine($"GetMovieHint()");
            //     List<object> movieHints = HttpContext.Session.GetObjectFromJson<List<object>> ("MovieHints");
            //     return Json (movieHints);
            // }


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



// public Movie GetSessionMovie ()
// {
//     int? thisGamesMovieId = HttpContext.Session.GetInt32 ("SessionMovieId");
//     Movie thisGamesMovie = _context.Movies.Include (w => w.Clues).SingleOrDefault (x => x.MovieId == thisGamesMovieId);
//     Console.WriteLine($"GetSessionMovie() : {thisGamesMovie.Title}");

//     Movie movie = new Movie ()
//     {
//         Title = thisGamesMovie.Title,
//         Year = thisGamesMovie.Year,
//         Clues = new List<Clue> ()
//     };

//     foreach(var clue in thisGamesMovie.Clues)
//     {
//         Console.WriteLine($"Clue: {clue.ClueText}");
//     }
//     return movie;