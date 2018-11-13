using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Generic;

namespace movieGame.Controllers.MixedControllers
{
    public class GetMovieInfoController : Controller
    {
        private MovieContext _context;

        public GetMovieInfoController (MovieContext context)
        {
            _context = context;
        }

        [HttpGet("movies")]
        public IActionResult ViewAllMovies ()
        {
            ViewBag.Movies = _context.Movies.Include (w => w.Clues).OrderBy (d => d.MovieId).ToList ();
            return View ("AllMovies");
        }

        public int GetCountOfMoviesInDb ()
        {
            int movieCount = _context.Movies.Count();
            // Console.WriteLine($"GetCountOfMoviesInDb() : 'movieCount' = {movieCount}");
            return movieCount;
        }

        public Movie GetAllMovieInfo(int movieId)
        {
            Movie movie = _context.Movies.Include (w => w.Clues).SingleOrDefault (x => x.MovieId == movieId);
            return movie;
        }


        // get all of a movie's JSON info based on movie title and release year
        public JObject GetMovieJSON (string MovieName, int MovieYear)
        {
            string apiQueryTitleYear = "https://www.omdbapi.com/?t=" + MovieName + "&y=" + MovieYear + "&apikey=4514dc2d";

            RestClient client = new RestClient (apiQueryTitleYear);
            RestRequest request = new RestRequest (Method.GET);
                request.AddHeader ("Postman-Token", "688d818e-29a5-498f-9c1c-907c7cb77c7f")
                        .AddHeader ("Cache-Control", "no-cache");

            IRestResponse response = client.Execute (request);
                string responseJSON = response.Content;

            JObject movieJObject = JObject.Parse (responseJSON);

            return movieJObject;
        }


        public int? GetMovieId(Movie movie)
        {
            int? movieId = movie.MovieId;
            return movieId;

        }


        public string GetMovieTitle(Movie movie)
        {
            string movieTitle = movie.Title;
            return movieTitle;
        }


        public string GetMovieTitle(JObject movieJObject)
        {
            string movieTitle = (string) movieJObject["Title"];
            return movieTitle;
        }


        public int? GetMovieReleaseYear(Movie movie)
        {
            int? movieReleaseYear = movie.Year;
            return movieReleaseYear;
        }


        public string GetMovieReleaseYear(JObject movieJObject)
        {
            string movieReleaseYear = (string) movieJObject["Year"];
            return movieReleaseYear;
        }


        public string GetMovieGenre(Movie movie)
        {
            string movieGenre = movie.Genre;
            return movieGenre;
        }


        public string GetMovieGenre(JObject movieJObject)
        {
            string movieGenre = (string) movieJObject["Genre"];
            return movieGenre;
        }


        public string GetMovieDirector(Movie movie)
        {
            string movieDirector = movie.Director;
            return movieDirector;
        }


        public string GetMovieDirector(JObject movieJObject)
        {
            string movieDirector = (string) movieJObject["Director"];
            return movieDirector;
        }


        public List<Clue> GetMovieClues(Movie movie)
        {
            List<Clue> clues = movie.Clues.ToList();
            return clues;
        }


        public Hints GetMoviesHints(Movie movie)
        {
            Hints thisMoviesHints = new Hints();
            thisMoviesHints.Director = GetMovieDirector(movie);
            thisMoviesHints.Genre = GetMovieGenre(movie);
            thisMoviesHints.ReleaseYear = GetMovieReleaseYear(movie).ToString();
            return thisMoviesHints;
        }


        public Hints GetMoviesHints(JObject movieJObject)
        {
            Hints thisMoviesHints = new Hints();
            thisMoviesHints.Director = GetMovieDirector(movieJObject);
            thisMoviesHints.Genre = GetMovieGenre(movieJObject);
            thisMoviesHints.ReleaseYear = GetMovieReleaseYear(movieJObject);

            return thisMoviesHints;
        }


        public void PrintHints(Hints h)
        {
            Console.WriteLine($"HINTS: Genre = {h.Genre}  |  ReleaseYear = {h.ReleaseYear}  |  Director = {h.Director}");
        }


        [HttpGet("movie/{id}")]
        public IActionResult ShowMovie (int id)
        {
            ViewBag.Movies = _context.Movies.Include (w => w.Clues).SingleOrDefault (x => x.MovieId == id);

            var currentMovie = _context.Movies.Include (w => w.Clues).SingleOrDefault (x => x.MovieId == id);
            var currentMovieTitle = currentMovie.Title;
            var currentMovieYear = currentMovie.Year;

            JObject movieJObject = GetMovieJSON (currentMovieTitle, currentMovieYear);

            string movieTitle = movieJObject["Title"].ToString ();
            string movieRating = (string) movieJObject["Rated"];
            string movieYear = (string) movieJObject["Year"];

            string movieActors = ViewBag.Actors = (string) movieJObject["Actors"];
            string movieWriter = ViewBag.MovieWriter = (string) movieJObject["Writer"];
            string movieDirector = ViewBag.MovieDirector = (string) movieJObject["Director"];
            string movieGenre = ViewBag.MovieGenre = (string) movieJObject["Genre"];
            string moviePoster = ViewBag.MoviePoster = (string) movieJObject["Poster"];

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
            int pictureSizeLarge = 400;
            int pictureSizeMedium = 300;
            int pictureSizeSmall = 200;
            int pictureSizeSmallest = 92;

            string actorPictureURL = (string) actorJSON["results"][0]["profile_path"];

            string actorPicLarge = ViewBag.ActorPicLarge = pictureBaseURL + pictureSizeLarge + actorPictureURL;
            string actorPicMedium = ViewBag.ActorPicMedium = pictureBaseURL + pictureSizeMedium + actorPictureURL;
            string actorPicSmall = ViewBag.ActorPicSmall = pictureBaseURL + pictureSizeSmall + actorPictureURL;
            string actorPicSmallest = ViewBag.ActorPicSmallest = pictureBaseURL + pictureSizeSmallest + actorPictureURL;

            return Json (actorJSON);
        }



    }
}