// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using System.Text;
// using System.Text.RegularExpressions;
// using ConsoleTables;


using System;
using System.Collections;
using System.Linq;                      // <--- various db queries (e.g., 'FirstOrDefault', 'OrderBy', etc.)
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Mvc;         // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using Microsoft.EntityFrameworkCore;    // <--- 'include' in db queries
using Newtonsoft.Json.Linq;             // <--- 'JObject'
using RestSharp;                        // <--- REST related things: 'RestClient', 'RestRequest', 'IRestResponse'
using movieGame.Models;



namespace movieGame.Controllers
{
    public class MovieController : Controller
    {
        private MovieContext _context;

        public MovieController (MovieContext context) {
            _context = context;
        }

        String Start = "STARTED";
        String Complete = "COMPLETED";


        // view table of all movies including Id, Title, Description, Year, and list of Clues
        [HttpGet]
        [Route ("allMovies")]
        public IActionResult ViewAllMovies ()
        {
            Start.ThisMethod();

            ViewBag.Movies = _context.Movies.Include(w => w.Clues).OrderBy(d => d.MovieId).ToList();

            // Complete.ThisMethod();
            return View ();
        }


        // get all of a movie's JSON info based on movie title and release year
        [HttpGet]
        [Route("getMovieJSON")]
        public IActionResult GetMovieJSON(string movieName, int movieYear)
        {
            Start.ThisMethod();

            // var APIqueryTitleYear = "https://www.omdbapi.com/?t=" + "Guardians of the Galaxy Vol. 2" + "&y=" + 2017 + "&apikey=4514dc2d";
            var APIqueryTitleYear = "https://www.omdbapi.com/?t=" + movieName + "&y=" + movieYear + "&apikey=4514dc2d";

            #region POSTMAN VARIABLES
                // CLIENT ---> 'RestSharp.RestClient'
                var client = new RestClient(APIqueryTitleYear);

                // REQUEST ---> 'RestSharp.RestRequest'
                var request = new RestRequest(Method.GET);

                request.AddHeader("Postman-Token", "688d818e-29a5-498f-9c1c-907c7cb77c7f");
                request.AddHeader("Cache-Control", "no-cache");

                // RESPONSE ---> 'RestSharp.RestResponse'
                IRestResponse response = client.Execute(request);
            #endregion POSTMAN VARIABLES

            // MESSY JSON ---> all movie JSON all garbled up (i.e., not parsed)
            var responseJSON = response.Content;

            // MOVIE JSON ---> all movie JSON presented more cleanly (i.e., it has been parsed)
            JObject movieJSON= JObject.Parse(responseJSON);

            // MOVIE TITLE ---> the title of the movie
            string MovieTitle = (string)movieJSON["Title"];

            // IMDB ID ---> imdb id number (.e.g, tt3896198)
            string imdbID = (string)movieJSON["imdbID"];

            // Complete.ThisMethod();
            return Json(movieJSON);
        }


        // get information about one movie
        [HttpGet]
        [Route("Movie/{id}")]
        public IActionResult ShowMovie(int id)
        {
            Start.ThisMethod();

            #region DATABASE QUERIES
                ViewBag.Movies = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == id);

                // CURRENT MOVIE ---> movieGame.Models.Movie
                var currentMovie = _context.Movies.Include(w => w.Clues).SingleOrDefault(x => x.MovieId == id);

                // CURRENT MOVIE TITLE ---> the title of the movie pulled from the database
                var currentMovieTitle = currentMovie.Title;
                // currentMovieTitle.Intro("current movie title");

                // CURRENT MOVIE YEAR ---> the release year of the movie pulled from the database
                var currentMovieYear = currentMovie.Year;
            #endregion DATABASE QUERIES

            #region API QUERIES
                // API QUERY ---> 'Microsoft.AspNetCore.Mvc.JsonResult'
                var APIquery = GetMovieJSON(currentMovieTitle, currentMovieYear);

                // API QUERY ---> 'Microsoft.AspNetCore.Mvc.JsonResult'; has a different type than APIquery
                var jsonResult = Json(APIquery);

                // JSON STRING ---> all JSON data; movie JSON data is nested within this data;
                string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult.Value);

                // MOVIE JSON ---> JSON STRING but cleaner/parsed
                JObject movieJSON = JObject.Parse(jsonString);
            #endregion API QUERIES

            // #region API MOVIE INFO
                string MovieTitle = (string)movieJSON["Value"]["Title"];
                string MovieRating = (string)movieJSON["Value"]["Rated"];
                string MovieYear = (string)movieJSON["Value"]["Year"];

                string MovieActors = ViewBag.Actors = (string)movieJSON["Value"]["Actors"];
                string MovieWriter = ViewBag.MovieWriter = (string)movieJSON["Value"]["Writer"];
                string MovieDirector = ViewBag.MovieDirector = (string)movieJSON["Value"]["Director"];
                string MovieGenre = ViewBag.MovieGenre = (string)movieJSON["Value"]["Genre"];
                string MoviePoster = ViewBag.MoviePoster = (string)movieJSON["Value"]["Poster"];
            // #endregion API MOVIE INFO

            Complete.ThisMethod();
            return View("Movie");
        }

        [HttpGet]
        [Route("/getActorImage")]
        public JsonResult GetActorImage(string actorName)
        {
            Start.ThisMethod();

            #region API REQUEST INFO
                // var APIkey = "4cbdf8913d9628d339184a127d136d68";
                var PersonsName = actorName;
                var client = new RestClient("https://api.themoviedb.org/3/search/person?api_key=1a1ef1aa4b51f19d38e4a7cb134a5699&language=en-US&query=" + PersonsName + "&page=1&include_adult=false");
                var request = new RestRequest(Method.GET);
                request.AddHeader("Postman-Token", "dbfd1014-ebcb-4c79-80eb-1b0eac81a888");
                request.AddHeader("Cache-Control", "no-cache");
                IRestResponse response = client.Execute(request);
            #endregion


            #region JSON
                var responseJSON = response.Content;
                JObject actorJSON = JObject.Parse(responseJSON);
            #endregion


            #region QUERIES
                string ActorName = ViewBag.ActorName = (string)actorJSON["results"][0]["name"];
                int ActorId = ViewBag.ActorId = (int)actorJSON["results"][0]["id"];
                ActorName.Intro("actor name");
                // ActorId.Intro("actor id");

                string PictureBaseURL = "https://image.tmdb.org/t/p/w";

                // these are picture sizes
                int PictureSizeLarge = 400;
                int PictureSizeMedium = 300;
                int PictureSizeSmall = 200;
                int PictureSizeSmallest = 92;

                string ActorPictureURL = (string)actorJSON["results"][0]["profile_path"];

                string ActorPicLarge = ViewBag.ActorPicLarge = PictureBaseURL + PictureSizeLarge + ActorPictureURL;
                string ActorPicMedium = ViewBag.ActorPicMedium = PictureBaseURL + PictureSizeMedium + ActorPictureURL;
                string ActorPicSmall = ViewBag.ActorPicSmall = PictureBaseURL + PictureSizeSmall + ActorPictureURL;
                string ActorPicSmallest = ViewBag.ActorPicSmallest = PictureBaseURL + PictureSizeSmallest + ActorPictureURL;

                ActorPicSmallest.Intro("smallest");
            #endregion

            Complete.ThisMethod();
            return Json (actorJSON);
        }



        [HttpGet]
        [Route("getBackgroundPosters")]
        public JsonResult GetBackgroundPosters()
        {
            Start.ThisMethod();

            // MOVIE DB PAGES --> set the number of api pages you want to query; 20 posters per page
            int movieDBpages = 2;

            // TOP MOVIE POSTERS ---> System.Collections.ArrayList
            ArrayList TopMoviePosters = new ArrayList();

            ArrayList AllPosterURLs = new ArrayList();

            for(var x = 1; x <= movieDBpages; x++)
            {
                var client = new RestClient("https://api.themoviedb.org/3/movie/popular?api_key=1a1ef1aa4b51f19d38e4a7cb134a5699&language=en-US&page=" + x + "&region=us");

                var request = new RestRequest(Method.GET);

                request.AddHeader("Postman-Token", "1348f187-6d98-4453-ad72-0f795b433479");
                request.AddHeader("Cache-Control", "no-cache");

                // RESPONSE ---> 'RestSharp.RestResponse'
                IRestResponse response = client.Execute(request);

                var responseJSON = response.Content;

                // BACKGROUND JSON ---> all movie JSON presented more cleanly (i.e., it has been parsed)
                JObject backgroundJSON= JObject.Parse(responseJSON);

                // BACKGROUND COUNT --> 20 (i.e., the number of posters on each api page)
                var backgroundCount = backgroundJSON["results"].Count();

                for(var i = 0; i <= backgroundCount - 1; i++)
                {
                    // ONE POSTER ---> '/inVq3FRqcYIRl2la8iZikYYxFNR.jpg' etc.
                    var onePoster = (string)backgroundJSON["results"][i]["poster_path"];

                    // POSTER URL ---> e.g., https://image.tmdb.org/t/p/w200/wridRvGxDqGldhzAIh3IcZhHT5F.jpg
                    var posterURL = "https://image.tmdb.org/t/p/w200" + onePoster;

                    // ONE TITLE ---> 'Deadpool' OR 'Justice League' etc.
                    var oneTitle = (string)backgroundJSON["results"][i]["title"];

                    TopMoviePosters.Add(onePoster);
                    AllPosterURLs.Add(posterURL);
                };

                ViewBag.TopMoviePosters = TopMoviePosters;
            }

            Complete.ThisMethod();
            return Json(TopMoviePosters);
        }
    }
}