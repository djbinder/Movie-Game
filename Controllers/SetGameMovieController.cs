﻿using System;
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
    public class SetGameMovieController : Controller
    {
        private MovieContext _context;
        private String _start = "STARTED";
        private String _complete = "COMPLETED";
        private GetMovieInfoController _getMovieInfoController;

        public SetGameMovieController (MovieContext context ) {
            _context = context;
        }


        public GetMovieInfoController GetMovieInfoController {
            get => _getMovieInfoController;
            set => _getMovieInfoController = value;
        }
        public string Start {
            get => _start;
            set => _start = value;
        }
        public string Complete {
            get => _complete;
            set => _complete = value;
        }




        // initiate game; select movie that will be guessed
        [HttpGet]
        [Route ("InitiateSinglePlayerGame")]
        public IActionResult InitiateSinglePlayerGame ()
        {
            Start.ThisMethod();

            int? PlayerId = ViewBag.PlayerId = HttpContext.Session.GetInt32("id");

            // QUERY PLAYER ---> movieGame.Models.Player
            Player ThisGamesPlayer = _context.Players.Include(m => m.MoviePlayerJoin).SingleOrDefault(p => p.PlayerId == PlayerId);

            // GAMES WON --> how many games has the player won; the next movie served is based off of this
            var ThisGamesPlayer_CurrentGamesWon = ViewBag.GamesWon = ThisGamesPlayer.GamesWon;

            int ThisGamesMovieId = ThisGamesPlayer_CurrentGamesWon + 1;
            ThisGamesMovieId.Intro("set movie id");

            GetMovieInfoController = new GetMovieInfoController(_context);

            // MOVIES IN DATABASE ---> '2' OR '3' etc.
            var CountOfMoviesInDatabase = GetMovieInfoController.MoviesInDatabaseCount();
            CountOfMoviesInDatabase.Intro("count of movies in db");

            if(ThisGamesMovieId > CountOfMoviesInDatabase)
            {
                ExtensionsD.Spotlight("user is caught up; no new movies to guess");
                ViewBag.MovieCount = CountOfMoviesInDatabase;
                return View("NoGame");
            }

            SetThisGamesMovie(ThisGamesMovieId);
            SetGameGuessCount();
            SetThisGamesHints();

            // Complete.ThisMethod();
            return RedirectToAction ("ViewPlaySingle", "ShowViews");
        }


        public void SetThisGamesMovie(int MovieId)
        {
            // Start.ThisMethod();
            int ThisGamesMovieId = MovieId;

            // MOVIES IN DATABASE ---> '2' OR '3' etc.
            var CountOfMoviesInDatabase = GetMovieInfoController.MoviesInDatabaseCount();
            // var CountOfMoviesInDatabase = _context.Movies.Count();

            // ONE MOVIE ---> movieGame.Models.Movie
            var ThisGamesMovieObject = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == ThisGamesMovieId);
            ThisGamesMovieObject.Title.Intro("this games movie");

            // SESSION MOVIE ID & NAME ---> sets the name and id for the movie that was selected
            HttpContext.Session.SetString("SessionMovieTitle", ThisGamesMovieObject.Title);
            HttpContext.Session.SetInt32("SessionMovieYear", ThisGamesMovieObject.Year);
            HttpContext.Session.SetInt32("SessionMovieId", ThisGamesMovieId);

            Hashtable SessionMovieInfoTable = new Hashtable();
                SessionMovieInfoTable.Add("SessionMovieTitle", ThisGamesMovieObject.Title);
                SessionMovieInfoTable.Add("SessionMovieYear", ThisGamesMovieObject.Year);
            HttpContext.Session.SetObjectAsJson("SessionMovieInfo", SessionMovieInfoTable);
        }



        public void SetGameGuessCount()
        {
            // Start.ThisMethod();
            HttpContext.Session.SetInt32("Guesscount", 3);
            int? guessCount = HttpContext.Session.GetInt32("Guesscount");
        }



        public void SetThisGamesHints()
        {
            // Start.ThisMethod();
            var SessionMovieTitle = HttpContext.Session.GetString("SessionMovieTitle");
            int SessionMovieYear = (int)HttpContext.Session.GetInt32("SessionMovieYear");

            // var _movieController = new GetMovieInfoController(_context);

            // Microsoft.AspNetCore.Mvc.JsonResult
            JObject OneMovieJObject = GetMovieInfoController.GetMovieJSON(SessionMovieTitle, SessionMovieYear);

            // System.Collections.Hashtable
            Hashtable OneMovieHashTable = GetMovieInfoController.GetMovieInfoHashTable(OneMovieJObject);

            string MovieGenre = ViewBag.MovieGenre = OneMovieHashTable["MovieGenre"].ToString();
            string MovieDirector = ViewBag.MovieDirector = OneMovieHashTable["MovieDirector"].ToString();
            string MovieTitle = ViewBag.MovieTitle = OneMovieHashTable["MovieTitle"].ToString();
            string MovieReleaseYear = ViewBag.MovieReleaseYear = OneMovieHashTable["MovieReleaseYear"].ToString();

            // MovieGenre.Intro("movie genre");
            // MovieDirector.Intro("movie Director");
            // MovieTitle.Intro("movie Title");
            // MovieReleaseYear.Intro("movie ReleaseYear");

            // the below represents three options to set movie info in JSON object; only one is truly needed

            // option #1
            List<object> SetHintsList = new List<object>();
                SetHintsList.Add(MovieGenre);
                SetHintsList.Add(MovieReleaseYear);
                SetHintsList.Add(MovieDirector);
                    // these set each of the above options as session objects/json
                    HttpContext.Session.SetObjectAsJson("MovieHints", SetHintsList);


            // option #2
                Hashtable SetHintsHashTable = new Hashtable();
                SetHintsHashTable.Add("Genre", MovieGenre);
                SetHintsHashTable.Add("Decade", MovieReleaseYear);
                SetHintsHashTable.Add("Director", MovieDirector);
                    HttpContext.Session.SetObjectAsJson("MovieHintsTable", SetHintsHashTable);

            // option #3
                Dictionary<string, string> SetHintsDictionary = new Dictionary<string, string>();
                    SetHintsDictionary.Add("Genre", MovieGenre);
                    SetHintsDictionary.Add("Decade", MovieReleaseYear);
                    SetHintsDictionary.Add("Director", MovieDirector);
                        HttpContext.Session.SetObjectAsJson("MovieHintsDictionary", SetHintsDictionary);
        }

    }
}