using System;
using System.Linq;
using System.Threading.Tasks;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace movieGame.Controllers.PlayerControllers
{
    public class ManageUsersController : Controller
    {
        private MovieContext _context;

        public ManageUsersController (MovieContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("allplayers")]
        public void ListAllPlayers()
        {
            var allPlayers = _context.Users.ToList();
            foreach(var player in allPlayers)
            {
                Console.WriteLine(player.UserEmail);
                Console.WriteLine(player.UserId);
            }
        }

        [HttpPost]
        [Route ("register")]
        public IActionResult RegisterUser (UserViewModel model) {
            Console.WriteLine ("lets try to register a new user");
            // string NameEntered = "dan binder";
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
                }
                int id = _context.Users.Where (u => u.UserEmail == model.RegisterViewModel.UserEmail).Select (p => p.UserId).SingleOrDefault ();
                Console.WriteLine ($"the player id for this session is {id}");
                HttpContext.Session.SetInt32 ("id", id);
                return RedirectToAction ("ViewGameListPage", "ShowViews");
            }

            return RedirectToAction ("ViewHomePage", "ShowViews");
        }

        [HttpPost]
        [Route ("login")]
        public IActionResult LogUserIn (UserViewModel model)
        {
            var email = model.LoginViewModel.UserEmail;
            var password = model.LoginViewModel.UserPassword;
            // Console.WriteLine($"EMAIL IS: {email}");
            // Console.WriteLine($"PASSWORD IS: {password}");

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
                        Console.WriteLine("user exists but password is wrong");
                        ModelState.AddModelError ("LoginViewModel.UserPassword", "Incorrect password. Please try again!");
                        return View ("Index");
                    }
                    HttpContext.Session.SetInt32 ("id", existingUser.UserId);
                    HttpContext.Session.SetString("playername", existingUser.UserFirstName);
                    return RedirectToAction("ViewGameListPage", "ShowViews");
                }
            }
            return View ("Index");
        }


        [HttpGet]
        [Route ("LogPlayerOut")]
        public IActionResult LogPlayerOut ()
        {
            HttpContext.Session.Clear ();
            return RedirectToAction ("Index");
        }


        public int CheckSession ()
        {
            int? id = HttpContext.Session.GetInt32 ("id");

            if (id == null) {
                Console.WriteLine ($"start new session with id {id}");
                return 0;
            }

            HttpContext.Session.SetInt32 ("id", (int) id);
            Console.WriteLine ($"continuing session with id {id}");

            return (int) id;
        }


        [HttpGet]
        [Route ("ClearSession")]
        public IActionResult ClearSession ()
        {
            HttpContext.Session.Clear ();
            return RedirectToAction ("Index");
        }
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