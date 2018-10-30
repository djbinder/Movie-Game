using System;
using System.Linq;
using System.Threading.Tasks;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace movieGame.Controllers {
    public class ManageUsersController : Controller {
        private MovieContext _context;

        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        public ManageUsersController (
            MovieContext context,
            UserManager<User> userManager,
            SignInManager<User> signInManager) {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route ("RegisterUser")]
        public IActionResult RegisterUser (UserViewModel model) {
            string NameEntered = "dan binder";
            string FirstName = "dan";
            string LastName = "binder";
            string Email = "dan@test.com";

            if (ModelState.IsValid) {
                User ExistingUser = _context.Users.FirstOrDefault (p => p.UserName == NameEntered);
                if (ExistingUser != null) {
                    ModelState.AddModelError ("RegisterViewModel.Email", "This is email is already registered!");
                    return View ("LoginRegister");
                }

                PasswordHasher<UserViewModel> hasher = new PasswordHasher<UserViewModel> ();
                string HashedPassword = hasher.HashPassword (model, model.RegisterViewModel.UserPassword);

                User NewUser = new User () {

                    UserFirstName = FirstName,
                    UserLastName = LastName,
                    UserPassword = HashedPassword,
                    UserEmail = Email,
                };

                _context.Add (NewUser);
                _context.SaveChanges ();

                int id = _context.Users.Where (u => u.UserEmail == model.RegisterViewModel.UserEmail).Select (p => p.UserId).SingleOrDefault ();
                HttpContext.Session.SetInt32 ("id", id);
                return RedirectToAction ("ViewGameList", "ShowViewsController");
            }

            return RedirectToAction ("ViewLogInRegisterPage", "ShowViews");
        }

        [HttpPost]
        [Route ("LogUserIn")]
        public IActionResult LogUserIn (UserViewModel model) {
            string NameEntered = "Dan Binder";

            if (ModelState.IsValid) {
                User ExistingUser = _context.Users.FirstOrDefault (p => p.UserName == NameEntered);
                if (ExistingUser == null) {
                    ModelState.AddModelError ("Login.UserEmail", "Email not found");
                    return View ("LoginRegister");
                }

                PasswordHasher<User> hasher = new PasswordHasher<User> ();

                if (hasher.VerifyHashedPassword (ExistingUser, ExistingUser.UserPassword, model.LoginViewModel.UserPassword) == 0) {
                    ModelState.AddModelError ("Login.UserPassword", "Incorrect password");
                    return View ("LoginRegister");
                }
                HttpContext.Session.SetInt32 ("id", ExistingUser.UserId);
                return RedirectToAction ("ViewGameList", "InitiateGame");
            }
            return View ("ViewLogInRegisterPage", "ShowViews");
        }

        //Other code
        public async Task<IActionResult> CreateUser (UserViewModel model) {
            if (ModelState.IsValid) {
                //Create a new User object, without adding a UserPassword
                User NewUser = new User {
                    UserName = model.RegisterViewModel.UserFirstName,
                    Email = model.RegisterViewModel.UserEmail
                };

                //CreateAsync will attempt to create the User in the database, simultaneously hashing the
                //password
                IdentityResult result = await _userManager.CreateAsync (NewUser, model.RegisterViewModel.UserPassword);

                //If the User was added to the database successfully
                if (result.Succeeded) {
                    //Sign In the newly created User
                    //We're using the SignInManager, not the UserManager!
                    await _signInManager.SignInAsync (NewUser, isPersistent : false);
                }
                //If the creation failed, add the errors to the View Model
                foreach (var error in result.Errors) {
                    ModelState.AddModelError (string.Empty, error.Description);
                }
            }
            return View (model);
        }

        public async Task<IActionResult> FindByEmailAsync (string Email) {
            await _userManager.FindByEmailAsync (Email);
            return View ();
        }

        public async Task<IActionResult> FindByIdAsync (string id) {
            await _userManager.FindByIdAsync (id);
            return View ();
        }

        public async Task<IActionResult> FindByNameAsync (string name) {
            await _userManager.FindByNameAsync (name);
            return View ();
        }

        public async Task<IActionResult> UpdateAsync (User User) {
            await _userManager.UpdateAsync (User);
            return View ();
        }

        public async Task<IActionResult> SignUserOut () {
            await _signInManager.SignOutAsync ();
            return View ();
        }

        private Task<User> GetCurrentUserAsync () {
            return _userManager.GetUserAsync (HttpContext.User);
        }

        // alternative login method
        // public IActionResult LoginMethod(string Email, string PasswordToCheck)
        // {
        //     // Attempt to retrieve a user from your database based on the Email submitted
        //     var User = userFactory.GetUserByEmail(Email);
        //     if(User != null && PasswordToCheck != null)
        //     {
        //         var Hasher = new PasswordHasher<User>();
        //         // Pass the user object, the hashed password, and the PasswordToCheck
        //         if(0 != Hasher.VerifyHashedPassword(User, User.Password, PasswordToCheck))
        //         {
        //             //Handle success
        //         }
        //     }

        //     return View("GameList");
        //     //Handle failure
        // }

    }
}