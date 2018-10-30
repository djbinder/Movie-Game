using System;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace movieGame.Controllers {
    public class SetSingleGameController : Controller {
        private MovieContext _context;

        public SetSingleGameController (MovieContext context) {
            _context = context;
        }

        public IActionResult CheckIfNameEntered (string NameEntered) {
            if (NameEntered == null) {
                ExtensionsD.Spotlight ("no name was entered");
                HttpContext.Session.SetString ("message", "Please Enter a Name to Play!");
                return RedirectToAction ("Index");
            } else {
                return RedirectToAction ("TestString");
            }

        }

        public static string TestString (string s) {
            if (String.IsNullOrEmpty (s))
                return "is null or empty";

            else
                return String.Format ("(\"{0}\") is neither null nor empty", s);
        }

        [HttpPost]
        [Route ("SetGamePlayer")]
        public IActionResult SetGamePlayer (string NameEntered) {

            Console.WriteLine ("Name Entered {0}.", TestString (NameEntered));

            if (NameEntered == null) {
                ExtensionsD.Spotlight ("no name was entered");
                HttpContext.Session.SetString ("message", "Please Enter a Name to Play!");
                return RedirectToAction ("ViewHomePage", "ShowViews");
            } else {
                NameEntered.Intro ("current player is");

                // EXISTS ---> checks if the player is already in the database; movieGame.Models.Player
                Player ExistingPlayer = _context.Players.FirstOrDefault (p => p.PlayerName == NameEntered);

                if (ExistingPlayer == null) {
                    ExtensionsD.Spotlight ("this is a new player");

                    Player NewPlayer = new Player () {
                        PlayerName = NameEntered,
                        Points = 0,
                        GamesAttempted = 0,
                        GamesWon = 0,
                        PlayerCoins = 0,
                        MoviePlayerJoin = new List<MoviePlayerJoin> (),
                        PlayerTeamJoin = new List<PlayerTeamJoin> ()
                    };

                    _context.Add (NewPlayer);
                    _context.SaveChanges ();

                    // QUERY PLAYER --> movieGame.Models.Player
                    Player QueryPlayer = _context.Players.FirstOrDefault (p => p.PlayerName == NameEntered);
                    HttpContext.Session.SetInt32 ("id", QueryPlayer.PlayerId);
                }

                // if the player is not a new player, go this way
                else {
                    Player CurrentPlayer = new Player () {
                        PlayerName = ExistingPlayer.PlayerName,
                        Points = ExistingPlayer.Points,
                        GamesAttempted = ExistingPlayer.GamesAttempted,
                        GamesWon = ExistingPlayer.GamesWon,
                        PlayerCoins = ExistingPlayer.PlayerCoins,
                        MoviePlayerJoin = ExistingPlayer.MoviePlayerJoin,
                        PlayerTeamJoin = ExistingPlayer.PlayerTeamJoin,
                    };

                    ExtensionsD.Spotlight ("this is an existing player");

                    HttpContext.Session.SetInt32 ("id", ExistingPlayer.PlayerId);
                }
            }

            HttpContext.Session.SetString ("player", NameEntered);

            string ThisGamesPlayersName = HttpContext.Session.GetString ("player");
            int? ThisGamesPlayerId = HttpContext.Session.GetInt32 ("id");

            return RedirectToAction ("ViewGameList", "ShowViews");
            // return RedirectToAction("InitiateSinglePlayerGame", "SetGameMovie");
        }
    }
}