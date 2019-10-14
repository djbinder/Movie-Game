using movieGame.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using C = System.Console;
using CsvHelper;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using movieGame.Infrastructure;


#pragma warning disable IDE0052
namespace movieGame.Controllers.ModelControllers
{
    [Route("model/movie")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly Helpers          _helpers;
        private readonly MovieGameContext _context;


        public MovieController(MovieGameContext context, Helpers helpers)
        {
            _helpers = helpers;
            _context = context;
        }

        readonly string csvFileLocation = "CSVfiles/movieGame_movies.110218-1.csv";


        // https://127.0.0.1:5001/movie/seed
        [HttpGet("test")]
        public void TestController()
        {
            _helpers.StartMethod();
            GetMoviesFromCsv();
            // var movieRecords = GetMoviesFromCsv().ToList();
            // C.WriteLine($"movieRecords [2] : {movieRecords.Count()}");
            // _context.AddRange(movieRecords);
            // _context.SaveChanges();

        }


        [HttpGet("async")]
        public async Task TestControllerAsync()
        {
            _helpers.StartMethod();
        }


        [Route("seed")]
        public void GetMoviesFromCsv()
        {
            _helpers.StartMethod();
            // IEnumerable<Movie> movieRecords;

            using(StreamReader reader = new StreamReader(csvFileLocation))
            using(CsvReader csv       = new CsvReader(reader))
            {
                csv.Configuration.RegisterClassMap<MovieMap>();

                var movieRecords = csv.GetRecords<Movie>();
                C.WriteLine($"movieRecords [1] : {movieRecords.Count()}");

                foreach(var movie in movieRecords)
                {
                    _context.Attach(movie);
                    // _context.Add(movie);
                    _context.SaveChanges();
                }
            } 
        }


        // public void AddMoviesToDatabase(IEnumerable<Movie> movieRecords)
        // {
        //     _helpers.StartMethod();
        //     C.WriteLine($"movieRecords [2] : {movieRecords.Count()}");

        //     _context.AddRange(movieRecords);
        //     _context.SaveChanges();
            
        //     _helpers.CompleteMethod();
        // }


        // public void SeedMoviesFromCsv()
        // {
        //     _helpers.StartMethod();

        //     IEnumerable<Movie> movieRecords = GetMoviesFromCsv();
            
        //     C.WriteLine($"movieRecords [3] : {movieRecords.Count()}");
        //     AddMoviesToDatabase(movieRecords);
        //     _helpers.CompleteMethod();
        // }





        #region FROM IMDB ------------------------------------------------------------

        // * Get all of a movie's JSON info based on movie title and release year
        public JObject GetMovieJSON (string movieName, int movieYear)
        {
            string apiQueryTitleYear = $"https://www.omdbapi.com/?t={movieName}&y={movieYear}&apikey=4514dc2d";

            RestClient client   = new RestClient  (apiQueryTitleYear);
            RestRequest request = new RestRequest (Method.GET);

            request.AddHeader ("Postman-Token", "688d818e-29a5-498f-9c1c-907c7cb77c7f")
                .AddHeader ("Cache-Control", "no-cache");

            IRestResponse response = client.Execute (request);
            string responseJSON    = response.Content;

            JObject movieJObject = JObject.Parse (responseJSON);

            return movieJObject;
        }


        #endregion FROM IMDB ------------------------------------------------------------






        #region PRINTING PRESS ------------------------------------------------------------





        #endregion PRINTING PRESS ------------------------------------------------------------



    }
}
