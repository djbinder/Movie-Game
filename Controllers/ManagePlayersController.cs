using System;
using System.Collections.Generic;
using System.Linq;                      // <--- various db queries (e.g., 'FirstOrDefault', 'OrderBy', etc.)
using System.Threading.Tasks;           // <--- 'Task<IActionResult>'
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Identity;    // <--- things related to PW / User security
using Microsoft.AspNetCore.Mvc;         // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using movieGame.Models;

namespace movieGame.Controllers
{
    public class ManagePlayersController : Controller
    {
        private MovieContext _context;

        String Start = "STARTED";
        String Complete = "COMPLETED";

        private readonly UserManager<Player> _userManager;

        private readonly SignInManager<Player> _signInManager;

        public ManagePlayersController(
            MovieContext context,
            UserManager<Player> userManager,
            SignInManager<Player> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        [Route("LogInRegisterPage")]
        public IActionResult ViewLogInRegisterPage()
        {
            Start.ThisMethod();
            Complete.ThisMethod();
            return View("LoginRegister");
        }


        [HttpPost]
        [Route("RegisterPlayer")]
        public IActionResult RegisterPlayer(PlayerViewModel model)
        {
            string NameEntered = "dan binder";
            string FirstName = "dan";
            string LastName = "binder";
            string Email = "dan@test.com";

            if(ModelState.IsValid)
            {
                Player ExistingPlayer = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);
                if(ExistingPlayer != null)
                {
                    ModelState.AddModelError("RegisterViewModel.Email", "This is email is already registered!");
                    return View("LoginRegister");
                }

                PasswordHasher<PlayerViewModel> hasher = new PasswordHasher<PlayerViewModel>();
                string HashedPassword = hasher.HashPassword(model, model.RegisterViewModel.PlayerPassword);

                Player NewPlayer = new Player () {
                    PlayerName = NameEntered,
                    PlayerFirstName = FirstName,
                    PlayerLastName = LastName,
                    PlayerPassword = HashedPassword,
                    PlayerEmail = Email,
                    Points = 0,
                    GamesAttempted = 0,
                    GamesWon = 0,
                    Movies = new List<Movie>(),
                    MoviePlayerJoin = new List<MoviePlayerJoin>(),
                };

                _context.Add(NewPlayer);
                _context.SaveChanges();

                int id = _context.Players.Where(u => u.PlayerEmail == model.RegisterViewModel.PlayerEmail).Select(p => p.PlayerId).SingleOrDefault();
                HttpContext.Session.SetInt32("id", id);
                return RedirectToAction("ViewGameList", "InitiateGame");
            }
            return View("LoginRegister");
        }


        [HttpPost]
        [Route("LogPlayerIn")]
        public IActionResult LogPlayerIn(PlayerViewModel model)
        {
            string NameEntered = "Dan Binder";

            if(ModelState.IsValid)
            {
                Player ExistingPlayer = _context.Players.FirstOrDefault(p => p.PlayerName == NameEntered);
                if(ExistingPlayer == null)
                {
                    ModelState.AddModelError("Login.PlayerEmail", "Email not found");
                    return View("LoginRegister");
                }

                PasswordHasher<Player> hasher = new PasswordHasher<Player>();

                if(hasher.VerifyHashedPassword(ExistingPlayer, ExistingPlayer.PlayerPassword, model.LoginViewModel.PlayerPassword) == 0)
                {
                    ModelState.AddModelError("Login.PlayerPassword", "Incorrect password");
                    return View("LoginRegister");
                }
                HttpContext.Session.SetInt32("id", ExistingPlayer.PlayerId);
                return RedirectToAction("ViewGameList", "InitiateGame");
            }
            return View("LoginRegister");
        }


        //Other code
        public async Task<IActionResult> CreatePlayer(PlayerViewModel model)
        {
            if(ModelState.IsValid)
            {
                //Create a new Player object, without adding a PlayerPassword
                Player NewPlayer = new Player
                {
                    UserName = model.RegisterViewModel.PlayerFirstName,
                    Email = model.RegisterViewModel.PlayerEmail
                };

                //CreateAsync will attempt to create the Player in the database, simultaneously hashing the
                //password
                IdentityResult result = await _userManager.CreateAsync(NewPlayer, model.RegisterViewModel.PlayerPassword);

                //If the Player was added to the database successfully
                if(result.Succeeded)
                {
                    //Sign In the newly created Player
                    //We're using the SignInManager, not the UserManager!
                    await _signInManager.SignInAsync(NewPlayer, isPersistent: false);
                }
                //If the creation failed, add the errors to the View Model
                foreach( var error in result.Errors )
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }


        public async Task<IActionResult> FindByEmailAsync(string Email)
        {
            await _userManager.FindByEmailAsync(Email);
            return View();
        }

        public async Task<IActionResult> FindByIdAsync(string id)
        {
            await _userManager.FindByIdAsync(id);
            return View();
        }

        public async Task<IActionResult> FindByNameAsync(string name)
        {
            await _userManager.FindByNameAsync(name);
            return View();
        }

        public async Task<IActionResult> UpdateAsync(Player player)
        {
            await _userManager.UpdateAsync(player);
            return View();
        }

        public async Task<IActionResult> SignUserOut()
        {
            await _signInManager.SignOutAsync();
            return View();
        }

        private Task<Player> GetCurrentPlayerAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }


        [HttpGet]
        [Route("LogPlayerOut")]
        public IActionResult LogPlayerOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}