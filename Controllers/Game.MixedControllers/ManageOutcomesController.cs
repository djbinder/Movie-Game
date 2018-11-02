using System;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace movieGame.Controllers.Game.MixedControllers
{
    public class ManageOutcomesController : Controller
    {
        private MovieContext _context;

        public ManageOutcomesController (MovieContext context)
        {
            _context = context;
        }

        List<Clue> _clueList = new List<Clue> ();

        [HttpPost]
        [Route ("CreateMovieUserJoin")]
        public JsonResult CreateMovieUserJoin (int GameOutcome)
        {
            int? playerId = HttpContext.Session.GetInt32 ("id");
            // playerId.Intro("player id");
            User retrievedUser = _context.Users.SingleOrDefault (p => p.UserId == playerId);

            // SESSION MOVIE ID --> id of current movie user was guessing
            var sessionMovieId = HttpContext.Session.GetInt32 ("SessionMovieId");

            // EXISTING JOINS --> System.Collections.Generic.List`1[movieGame.Models.MovieUserJoin]
            // JOINS LIST --> check if a join of player and movie already exists
            var playerJoins = _context.MovieUserJoin.Where (p => p.MovieId == sessionMovieId).Where (t => t.UserId == playerId);
            var joinsList = playerJoins.ToList ();

            List<int> maxes = new List<int> ();

            // check the game outcome - 1 is a win, 0 is a loss
            int oneGameOutcome = GameOutcome;
            // oneGameOutcome.Intro("one game outcome");

            // the player won, the join needs to reflect that
            if (oneGameOutcome == 1)
            {
                // since the player won, check if any joins with player/movie combo already exists
                int? newPoints = HttpContext.Session.GetInt32 ("newPoints");
                // newPoints.Intro("new points");

                if (joinsList.Count > 0)
                {
                    // this is not the first time the player has attempted to guess this movie
                    ExtensionsD.Spotlight ("MPJ_A -- WIN _____ NOT FIRST JOIN");
                    foreach (var item in joinsList)
                    {
                        // get current attempt count and count of joins
                        int currAttemptCount = item.AttemptCount;
                        maxes.Add (currAttemptCount);
                        currAttemptCount.Intro ("curr attempts count");
                    }

                    // get current max attempt of player/movie combo
                    var newAttemptsMax = maxes.Max () + 1;

                    // MPJ --> create new many-to-many of player and movie
                    MovieUserJoin MPJ_A = new MovieUserJoin
                    {
                        UserId = (int) playerId,
                        MovieId = (int) sessionMovieId,
                        AttemptCount = newAttemptsMax,
                        PointsReceived = (int) newPoints,
                        WinFlag = true,
                    };
                    _context.Add (MPJ_A);
                }

                // this is the first time this player has attempted to guess this movie
                else
                {
                    ExtensionsD.Spotlight ("MPJ_B -- WIN _____ FIRST JOIN");
                    // MPJ --> create new many-to-many of player and movie
                    MovieUserJoin MPJ_B = new MovieUserJoin
                    {
                        UserId = (int) playerId,
                        MovieId = (int) sessionMovieId,
                        AttemptCount = 1,
                        PointsReceived = (int) newPoints,
                        WinFlag = true,
                    };
                    _context.Add (MPJ_B);
                }
            }

            // the player lost
            if (oneGameOutcome == 0)
            {
                // this is not the first time the player has attempted to guess this movie
                if (joinsList.Count > 0) {
                    ExtensionsD.Spotlight ("MPJ_C -- LOSS _____ NOT FIRST JOIN");
                    foreach (var item in joinsList)
                    {
                        // get current attempt count and count of joins
                        int currAttemptCount = item.AttemptCount;
                        maxes.Add (currAttemptCount);
                        currAttemptCount.Intro ("curr attempts count");
                    }

                    var newAttemptsMax = maxes.Max () + 1;

                    // MPJ --> create new many-to-many of player and movie
                    MovieUserJoin MPJ_C = new MovieUserJoin
                    {
                        UserId = (int) playerId,
                        MovieId = (int) sessionMovieId,
                        AttemptCount = newAttemptsMax,
                        PointsReceived = 0,
                        WinFlag = false,
                    };
                    _context.Add (MPJ_C);
                }
                else
                {
                    ExtensionsD.Spotlight ("MPJ_D -- LOSS _____ FIRST JOIN");

                    // MPJ --> create new many-to-many of player and movie
                    MovieUserJoin MPJ_D = new MovieUserJoin
                    {
                        UserId = (int) playerId,
                        MovieId = (int) sessionMovieId,
                        AttemptCount = 1,
                        PointsReceived = 0,
                        WinFlag = false,
                    };
                    _context.Add (MPJ_D);
                }
            }
            _context.SaveChanges ();
            return Json (playerJoins);
        }


        [HttpPost]
        [Route ("UpdatePlayerGames")]
        public IActionResult UpdatePlayerGames (int gameOutcome, int playerId)
        {
            gameOutcome.Intro ("this game outcome");

            User retrievedPlayer = _context.Users.SingleOrDefault (p => p.UserId == playerId);
            // var onePlayerId = retrievedPlayer.PlayerId;

            // update number of games attempted
            var currGamesAttempted = retrievedPlayer.GamesAttempted;
            int newGamesAttempted = currGamesAttempted + 1;
            newGamesAttempted.Intro ("new games played");
            retrievedPlayer.GamesAttempted = newGamesAttempted;

            // if the player won, update games won
            if (gameOutcome == 1)
            {
                var currGamesWon = retrievedPlayer.GamesWon;
                int newGamesWon = currGamesWon + 1;
                retrievedPlayer.GamesWon = newGamesWon;
                newGamesWon.Intro ("new games won");
                // Console.WriteLine("UP_GAMES  |  W  | send to C_MPJ");
                CreateMovieUserJoin (1);
            }
            else
            {
                // Console.WriteLine("UP_GAMES  |  L  | send to C_MPJ");
                CreateMovieUserJoin (0);
            }
            retrievedPlayer.UpdatedAt = DateTime.Now;
            _context.SaveChanges ();
            return View ();
        }


        public JsonResult UpdatePlayerPoints (Clue clueInfo)
        {
            // PLAYER ID ---> the PlayerId of the current player (e.g., 8 OR 4 OR 12 etc.)
            var playerId = HttpContext.Session.GetInt32 ("id");

            // MOVIE LIST ---> System.Collections.Generic.List`1[movieGame.Models.Movie]
            _clueList.Add (clueInfo);

            // ONE GAME OUTCOME --> 1 means the player won, 0 means the player lost
            int oneGameOutcome = clueInfo.MovieId;

            if (oneGameOutcome > 0) {
                // RETRIEVED PLAYER ---> movieGame.Models.Player
                User retrievedPlayer = _context.Users.FirstOrDefault (p => p.UserId == playerId);

                // CURRENT POINTS --> players current points pulled from the database
                var currentPoints = retrievedPlayer.Points;
                // currentPoints.Intro("current points");

                // NEW POINTS ---> the value of the clue the movie was correctly guessed on; retrieved from 'UpdatePlayerPoints' javascript function
                int newPoints = clueInfo.CluePoints;
                HttpContext.Session.SetInt32 ("newPoints", newPoints);
                // newPoints.Intro("new points to add");

                // RETRIEVED PLAYER POINT --> adds current points and new points; then saves them to the database
                retrievedPlayer.Points = newPoints + currentPoints;

                UpdatePlayerGames (1, (int) playerId);
            }
            else
            {
                UpdatePlayerGames (0, (int) playerId);
            }
            return Json (clueInfo);
        }
    }
}