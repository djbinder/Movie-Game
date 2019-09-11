using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using movieGame.Controllers.MixedControllers;
using movieGame.Infrastructure;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace movieGame.Controllers.PlayerControllers.ManageUsersController
{
    public class ManageUsersController : Controller
    {
        private readonly SessionUser _sessionUser;

        private static MovieContext _context;
        public Helpers _h = new Helpers ();

        private static GetMovieInfoController _getMovie = new GetMovieInfoController (context: _context);

        public static GetMovieInfoController GetMovie { get => _getMovie; set => _getMovie = value; }

        public ManageUsersController (MovieContext context, SessionUser sessionUser)
        {
            _context = context;
            _sessionUser = sessionUser;
        }


        #region MANAGE ROUTING ------------------------------------------------------------

        [HttpGet ("user/{id}")]
        public IActionResult ViewUserProfilePage (int id)
        {
            Console.WriteLine ($"users id: {id}");

            User thisUser = ViewBag.User = GetUser (1);

            var moviesInDatabase = ViewBag.MovieCount = _context.Movies.Count ();

            List<MovieUserJoin> usersMovieJoinList = GetMovieUserJoinList (id);
            ViewBag.MovieUserJoinList = usersMovieJoinList;

            List<Movie> usersMoviesList = GetListOfUsersMovies (id);
            ViewBag.UsersMoviesList = usersMoviesList;

            return View ("Userprofile");
        }

        #endregion MANAGE ROUTING ------------------------------------------------------------

        #region LOGIN AND REGISTER ------------------------------------------------------------

        [HttpPost ("registeruser")]
        public IActionResult RegisterUser (UserViewModel model)
        {
            var firstName = model.RegisterViewModel.UserFirstName;
            var lastName = model.RegisterViewModel.UserLastName;
            var email = model.RegisterViewModel.UserEmail;
            var password = model.RegisterViewModel.UserPassword;
            var confirmPassword = model.RegisterViewModel.Confirm;

            if (ModelState.IsValid)
            {
                User existingUser = _context.Users.FirstOrDefault (p => p.UserEmail == email);
                if (existingUser != null)
                {
                    Console.WriteLine ("this user already exists");
                    ModelState.AddModelError ("RegisterViewModel.Email", "This is email is already registered!");
                    return View ("index");
                }

                else
                {
                    PasswordHasher<UserViewModel> hasher = new PasswordHasher<UserViewModel> ();
                    string hashedPassword = hasher.HashPassword (model, model.RegisterViewModel.UserPassword);

                    User newUser = new User ()
                    {
                        UserFirstName = firstName,
                        UserLastName = lastName,
                        UserPassword = hashedPassword,
                        UserEmail = email,
                    };
                    _context.Add (newUser);
                    _context.SaveChanges ();

                    int userId = _context.Users.Where (u => u.UserEmail == model.RegisterViewModel.UserEmail).Select (p => p.UserId).SingleOrDefault ();

                    SetUserIdInSession (userId);
                    return RedirectToAction ("ViewGameListPage", "ShowViews");
                }
            }
            return View ("Index");
        }



        [HttpPost ("login")]
        public IActionResult LogUserIn (UserViewModel model)
        {
            _h.StartMethod ();

            var email = model.LoginViewModel.UserEmail;
            var password = model.LoginViewModel.UserPassword;

            if (ModelState.IsValid)
            {
                User existingUser = _context.Users.FirstOrDefault (p => p.UserEmail == email);
                if (existingUser == null)
                {
                    Console.WriteLine ("this email does not exist; please enter register");
                    ModelState.AddModelError ("LoginViewModel.UserEmail", "Email not found. Please register!");
                    return View ("Index");
                }
                else
                {
                    PasswordHasher<User> hasher = new PasswordHasher<User> ();

                    if (hasher.VerifyHashedPassword (existingUser, existingUser.UserPassword, model.LoginViewModel.UserPassword) == 0)
                    {
                        Console.WriteLine ("user exists but password is wrong");
                        ModelState.AddModelError ("LoginViewModel.UserPassword", "Incorrect password. Please try again!");
                        return View ("Index");
                    }

                    SetUserIdInSession (existingUser.UserId);
                    SetUserNameInSession (existingUser.UserFirstName);

                    // _sessionUser.SessionUserId = existingUser.UserId;
                    // var sessionUserId = _sessionUser.SessionUserId;
                    // Console.WriteLine($"sessionUserUserId: {sessionUserId}");

                    // SetSessionUserIdInSession(sessionUserId);

                    string userFirstName = HttpContext.Session.GetString ("UserName");
                    // Console.WriteLine ($"userFirstName: {userFirstName}");

                    return RedirectToAction ("ViewGameListPage", "ShowViews");
                }
            }
            return View ("Index");
        }

        public void SetUserIdInSession (int userId)
        {
            // _h.StartMethod ();
            // Console.WriteLine ($"userId: {userId}");
            HttpContext.Session.SetInt32 ("UserId", userId);
        }
        public void SetSessionUserIdInSession (int sessionUserId)
        {
            // _h.StartMethod ();
            // Console.WriteLine ($"sessionUserId: {sessionUserId}");
            HttpContext.Session.SetInt32 ("SessionUserId",sessionUserId);
        }

        public int GetUserIdFromSession ()
        {
            // _h.StartMethod ();
            int userId = (int) HttpContext.Session.GetInt32 ("UserId");
            // Console.WriteLine ($"userId: {userId}");
            return userId;
        }


        public void SetUserNameInSession (string firstName)
        {
            _h.StartMethod ();
            Console.WriteLine ($"firstName: {firstName}");
            HttpContext.Session.SetString ("UserName", firstName);
        }


        [HttpGet ("log_player_out")]
        public IActionResult LogPlayerOut ()
        {
            HttpContext.Session.Clear ();
            return RedirectToAction ("Index");
        }


        // public int CheckSession ()
        // {
        //     _h.StartMethod ();
        //     int? userId = HttpContext.Session.GetInt32 ("UserId");
        //     Console.WriteLine ($"userId': {userId}");

        //     if (userId == null)
        //     {
        //         Console.WriteLine ("NO ID IN SESSION");
        //         return 0;
        //     }

        //     HttpContext.Session.SetInt32 ("UserId", (int) userId);
        //     Console.WriteLine ($"continuing session with id {userId}");

        //     return (int) userId;
        // }

        [HttpGet ("clear_session")]
        public IActionResult ClearSession ()
        {
            HttpContext.Session.Clear ();
            return RedirectToAction ("Index");
        }

        #endregion LOGIN AND REGISTER ------------------------------------------------------------

        #region GET USER INFO ------------------------------------------------------------

        public User GetUser (int id)
        {
            User thisUser = _context.Users
                .Include (m => m.MovieUserJoin)
                .ThenInclude (n => n.Movie)
                .SingleOrDefault (p => p.UserId == 1);
            return thisUser;
        }

        public List<MovieUserJoin> GetMovieUserJoinList (int id)
        {
            User thisUser = GetUser (id);
            var userMovieList = thisUser.MovieUserJoin.ToList ();
            return userMovieList;
        }

        public List<Movie> GetListOfUsersMovies (int id)
        {
            User thisUser = GetUser (id);
            MovieUserJoin newMovieUserJoin = new MovieUserJoin ();
            List<MovieUserJoin> movieUserJoinList = thisUser.MovieUserJoin.ToList ();
            List<Movie> usersMovies = new List<Movie> ();

            foreach (var movie in movieUserJoinList)
            {
                Movie _movie = new Movie
                {
                    MovieId = movie.MovieId,
                    Title = movie.Movie.Title,
                    Year = movie.Movie.Year,
                    Poster = movie.Movie.Poster
                };

                GetMoviePosters (_movie.Title, _movie.Year);

                int gamesWon = thisUser.GamesWon;
                if (movie.MovieId <= gamesWon)
                {
                    usersMovies.Add (_movie);
                }
            }
            return usersMovies;
        }

        public List<string> GetMoviePosters (string title, int year)
        {
            List<string> usersMoviesPosters = new List<string> ();
            JObject UserMoviesJObject = GetMovie.GetMovieJSON (title, year);
            string moviePoster = UserMoviesJObject["Poster"].ToString ();
            usersMoviesPosters.Add (moviePoster);

            return usersMoviesPosters;
        }

        public IList<User> GetTopTenLeaders ()
        {
            var topTenLeaders = _context.Users.OrderByDescending (t => t.Points).Take (10).ToList ();
            return topTenLeaders;
        }

        #endregion GET USER INFO ------------------------------------------------------------

    }
}

// var modelValues = ModelState.Values;
// foreach(var val in modelValues)
// {
//     Console.WriteLine(val.Errors);
//     var errors = val.Errors;
//     foreach(var error in errors)
//     {
//         Console.WriteLine(error.ErrorMessage);
//     }
// }
// Console.WriteLine($"MODEL STATE VALUES: {ModelState.Values}");

// Console.WriteLine (model.RegisterViewModel.UserFirstName);
// Console.WriteLine (model.RegisterViewModel.UserLastName);
// Console.WriteLine (model.RegisterViewModel.UserEmail);
// Console.WriteLine (model.RegisterViewModel.UserPassword);
// Console.WriteLine (model.RegisterViewModel.Confirm);

// [HttpGet]
// [Route("allplayers")]
// public void ListAllPlayers()
// {
//     var allPlayers = _context.Users.ToList();
//     foreach(var player in allPlayers)
//     {
//         Console.WriteLine(player.UserEmail);
//         Console.WriteLine(player.UserId);
//     }
// }
