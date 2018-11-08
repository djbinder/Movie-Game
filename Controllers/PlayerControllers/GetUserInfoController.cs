using System;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using movieGame.Controllers.MixedControllers;

namespace movieGame.Controllers.UserControllers
{
    public class GetUserInfoController : Controller
    {
        private static MovieContext _context;

        private static GetMovieInfoController _getMovie = new GetMovieInfoController(context: _context);

        public static GetMovieInfoController GetMovie { get => _getMovie; set => _getMovie = value; }

        public GetUserInfoController (MovieContext context)
        {
            _context = context;
        }


        // [HttpGet("user/{id}")]
        // public IActionResult ViewUserProfilePage (int id)
        // {
        //     Console.WriteLine($"users id: {id}");

        //     User thisUser = ViewBag.User = GetUser(1);

        //     var moviesInDatabase = ViewBag.MovieCount = _context.Movies.Count ();

        //     List<MovieUserJoin> usersMovieJoinList = GetMovieUserJoinList (id);
        //     ViewBag.MovieUserJoinList = usersMovieJoinList;

        //     List<Movie> usersMoviesList = GetListOfUsersMovies(id);
        //     ViewBag.UsersMoviesList = usersMoviesList;

        //     return View("Userprofile");
        // }


        // #region GET USER INFO ------------------------------------------------------------

        //     public User GetUser (int id)
        //     {
        //         User thisUser = _context.Users
        //             .Include (m => m.MovieUserJoin)
        //             .ThenInclude (n => n.Movie)
        //             .SingleOrDefault (p => p.UserId == 1);
        //         return thisUser;
        //     }

        //     public List<MovieUserJoin> GetMovieUserJoinList (int id)
        //     {
        //         User thisUser = GetUser (id);
        //         var userMovieList = thisUser.MovieUserJoin.ToList ();
        //         return userMovieList;
        //     }

        //     public List<Movie> GetListOfUsersMovies (int id)
        //     {
        //         User thisUser = GetUser (id);
        //         MovieUserJoin newMovieUserJoin = new MovieUserJoin();
        //         List<MovieUserJoin> movieUserJoinList = thisUser.MovieUserJoin.ToList ();
        //         List<Movie> usersMovies = new List<Movie> ();

        //         foreach (var movie in movieUserJoinList)
        //         {
        //             Movie _movie = new Movie
        //             {
        //                 MovieId = movie.MovieId,
        //                 Title = movie.Movie.Title,
        //                 Year = movie.Movie.Year,
        //                 Poster = movie.Movie.Poster
        //             };

        //             GetMoviePosters (_movie.Title, _movie.Year);

        //             int gamesWon = thisUser.GamesWon;
        //             if (movie.MovieId <= gamesWon)
        //             {
        //                 usersMovies.Add (_movie);
        //             }
        //         }
        //         return usersMovies;
        //     }

        //     public List<string> GetMoviePosters (string title, int year)
        //     {
        //         List<string> usersMoviesPosters = new List<string> ();

        //         JObject UserMoviesJObject = GetMovie.GetMovieJSON (title, year);

        //         string moviePoster = UserMoviesJObject["Poster"].ToString ();
        //         usersMoviesPosters.Add (moviePoster);

        //         return usersMoviesPosters;
        //     }

        //     public IList<User> GetTopTenLeaders ()
        //     {
        //         var topTenLeaders = _context.Users.OrderByDescending (t => t.Points).Take (10).ToList ();
        //         return topTenLeaders;
        //     }

        // #endregion GET USER INFO ------------------------------------------------------------
    }
}