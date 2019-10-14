using System;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RestSharp;
using C = System.Console;


namespace movieGame.Controllers.MixedControllers
{
    public class GetMovieInfoController : Controller
    {
        private readonly MovieGameContext _context;

        public GetMovieInfoController (MovieGameContext context)
        {
            _context = context;
        }

        [HttpGet ("movies")]
        public IActionResult ViewAllMovies ()
        {
            ViewBag.Movies = _context.Movies.Include (w => w.Clues).OrderBy (d => d.MovieId).ToList ();
            return View ("AllMovies");
        }

        public int GetCountOfMoviesInDb ()
        {
            int movieCount = _context.Movies.Count ();
            return movieCount;
        }

        public Movie GetAllMovieInfo (int movieId)
        {
            Movie movie = _context.Movies.Include (w => w.Clues).SingleOrDefault (x => x.MovieId == movieId);
            return movie;
        }

        // get all of a movie's JSON info based on movie title and release year
        public JObject GetMovieJSON (string MovieName, int MovieYear)
        {
            string apiQueryTitleYear = "https://www.omdbapi.com/?t=" + MovieName + "&y=" + MovieYear + "&apikey=4514dc2d";

            RestClient client   = new RestClient (apiQueryTitleYear);
            RestRequest request = new RestRequest (Method.GET);
            request.AddHeader ("Postman-Token", "688d818e-29a5-498f-9c1c-907c7cb77c7f")
                .AddHeader ("Cache-Control", "no-cache");

            IRestResponse response = client.Execute (request);
            string responseJSON    = response.Content;

            JObject movieJObject = JObject.Parse (responseJSON);

            return movieJObject;
        }

        public int? GetMovieId (Movie movie)
        {
            return movie.MovieId;
        }

        public string GetMovieTitle (Movie movie)
        {
            return movie.Title;
        }

        public string GetMovieTitle (JObject movieJObject)
        {
            return (string) movieJObject["Title"];
        }

        public int? GetMovieReleaseYear (Movie movie)
        {
            return movie.Year;
        }

        public string GetMovieReleaseYear (JObject movieJObject)
        {
            return (string) movieJObject["Year"];
        }

        public string GetMovieGenre (Movie movie)
        {
            return movie.Genre;
        }

        public string GetMovieGenre (JObject movieJObject)
        {
            return (string) movieJObject["Genre"];
        }

        public string GetMovieDirector (Movie movie)
        {
            return movie.Director;
        }

        public string GetMovieDirector (JObject movieJObject)
        {
            return (string) movieJObject["Director"];
        }

        public List<Clue> GetMovieClues (Movie movie)
        {
            return movie.Clues.ToList ();
        }

        public Hint GetMoviesHint (Movie movie)
        {
            return new Hint
            {
                Director    = GetMovieDirector(movie),
                Genre       = GetMovieGenre(movie),
                ReleaseYear = GetMovieReleaseYear(movie).ToString()
            };
        }



        public Hint GetMoviesHint (JObject movieJObject)
        {
            return new Hint
            {
                Director    = GetMovieDirector(movieJObject),
                Genre       = GetMovieGenre(movieJObject),
                ReleaseYear = GetMovieReleaseYear(movieJObject)
            };
        }

        public void PrintHints (Hint h)
        {
            C.WriteLine ($"HINTS: Genre = {h.Genre}  |  ReleaseYear = {h.ReleaseYear}  |  Director = {h.Director}");
        }

        [HttpGet ("movie/{id}")]
        public IActionResult ShowMovie (int id)
        {
            ViewBag.Movies = _context.Movies.Include (w => w.Clues).SingleOrDefault (x => x.MovieId == id);

            Movie currentMovie       = _context.Movies.Include (w => w.Clues).SingleOrDefault (x => x.MovieId == id);
            string currentMovieTitle = currentMovie.Title;
            int? currentMovieYear    = currentMovie.Year;

            JObject movieJObject = GetMovieJSON (currentMovieTitle, (int)currentMovieYear);

            string movieTitle  = movieJObject["Title"].ToString ();
            string movieRating = (string) movieJObject["Rated"];
            string movieYear   = (string) movieJObject["Year"];

            string movieActors   = ViewBag.Actors = (string) movieJObject["Actors"];
            string movieWriter   = ViewBag.MovieWriter = (string) movieJObject["Writer"];
            string movieDirector = ViewBag.MovieDirector = (string) movieJObject["Director"];
            string movieGenre    = ViewBag.MovieGenre = (string) movieJObject["Genre"];
            string moviePoster   = ViewBag.MoviePoster = (string) movieJObject["Poster"];

            return View ("SingleMovie");
        }

        public JsonResult GetActorImage (string actorName)
        {
            var client = new RestClient ("https://api.themoviedb.org/3/search/person?api_key=1a1ef1aa4b51f19d38e4a7cb134a5699&language=en-US&query=" + actorName + "&page=1&include_adult=false");

            var request = new RestRequest (Method.GET);
            request.AddHeader ("Postman-Token", "dbfd1014-ebcb-4c79-80eb-1b0eac81a888")
                .AddHeader ("Cache-Control", "no-cache");

            IRestResponse response = client.Execute (request);

            JObject actorJSON = JObject.Parse (response.Content);

            ViewBag.ActorName = (string) actorJSON["results"][0]["name"];
            // int actorId = ViewBag.ActorId = (int) actorJSON["results"][0]["id"];

            string pictureBaseURL = "https://image.tmdb.org/t/p/w";

            // these are picture sizes
            int pictureSizeLarge    = 400;
            int pictureSizeMedium   = 300;
            int pictureSizeSmall    = 200;
            int pictureSizeSmallest = 92;

            string actorPictureURL = (string) actorJSON["results"][0]["profile_path"];

            string actorPicLarge    = ViewBag.ActorPicLarge    = pictureBaseURL + pictureSizeLarge    + actorPictureURL;
            string actorPicMedium   = ViewBag.ActorPicMedium   = pictureBaseURL + pictureSizeMedium   + actorPictureURL;
            string actorPicSmall    = ViewBag.ActorPicSmall    = pictureBaseURL + pictureSizeSmall    + actorPictureURL;
            string actorPicSmallest = ViewBag.ActorPicSmallest = pictureBaseURL + pictureSizeSmallest + actorPictureURL;

            return Json (actorJSON);
        }

    }
}
