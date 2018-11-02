// using System;
// using System.Collections.Generic;
// using System.Linq;
// using movieGame.Models;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;

// namespace movieGame.Controllers.Game.SingleControllers
// {
//     public class SetSingleGameController : Controller
//     {
//         private MovieContext _context;

//         public SetSingleGameController (MovieContext context)
//         {
//             _context = context;
//         }


//         [HttpPost]
//         [Route ("SetGameUser")]
//         public IActionResult SetGameUser (string nameEntered)
//         {
//             if (nameEntered == null)
//             {
//                 HttpContext.Session.SetString ("message", "Please Enter a Name to Play!");
//                 return RedirectToAction ("ViewHomePage", "ShowViews");
//             }
//             else
//             {
//                 // EXISTS ---> checks if the User is already in the database; movieGame.Models.User
//                 User existingUser = _context.Users.FirstOrDefault (p => p.UserFirstName == nameEntered);

//                 if (existingUser == null)
//                 {
//                     User newUser = new User ()
//                     {
//                         UserFirstName = nameEntered,
//                         Points = 0,
//                         GamesAttempted = 0,
//                         GamesWon = 0,
//                         UserCoins = 0,
//                         MovieUserJoin = new List<MovieUserJoin> (),
//                         UserTeamJoin = new List<UserTeamJoin> ()
//                     };
//                     _context.Add (newUser);
//                     _context.SaveChanges ();

//                     // QUERY User --> movieGame.Models.User
//                     User queryUser = _context.Users.FirstOrDefault (p => p.UserFirstName == nameEntered);
//                     HttpContext.Session.SetInt32 ("id", queryUser.UserId);
//                 }

//                 // if the User is not a new User, go this way
//                 else
//                 {
//                     User currentUser = new User ()
//                     {
//                         UserFirstName = existingUser.UserFirstName,
//                         Points = existingUser.Points,
//                         GamesAttempted = existingUser.GamesAttempted,
//                         GamesWon = existingUser.GamesWon,
//                         UserCoins = existingUser.UserCoins,
//                         MovieUserJoin = existingUser.MovieUserJoin,
//                         UserTeamJoin = existingUser.UserTeamJoin,
//                     };
//                     ExtensionsD.Spotlight ("this is an existing User");
//                     HttpContext.Session.SetInt32 ("id", existingUser.UserId);
//                 }
//             }

//             HttpContext.Session.SetString ("User", nameEntered);

//             string thisGamesUsersName = HttpContext.Session.GetString ("User");
//             int? thisGamesUserId = HttpContext.Session.GetInt32 ("id");

//             return RedirectToAction ("ViewGameList", "ShowViews");
//             // return RedirectToAction("InitiateSingleUserGame", "SetGameMovie");
//         }
//     }
// }