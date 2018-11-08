using System;
using System.Collections.Generic;
using System.Linq;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace movieGame.Controllers.MixedControllers
{
    public class ManageOutcomesController : Controller
    {
        private MovieContext _context;

        public ManageOutcomesController (MovieContext context)
        {
            _context = context;
        }

        List<Clue> _clueList = new List<Clue> ();


        // [HttpPost("create_movie_user_join")]
        public void CreateMovieUserJoin (int GameOutcome)
        {
            int? playerId = HttpContext.Session.GetInt32 ("id");

            User retrievedUser = _context.Users.SingleOrDefault (p => p.UserId == playerId);

            var sessionMovieId = HttpContext.Session.GetInt32 ("SessionMovieId");

            // check if a join of player and movie already exists
            var playerJoins = _context.MovieUserJoin.Where (p => p.MovieId == sessionMovieId).Where (t => t.UserId == playerId);
            var joinsList = playerJoins.ToList ();

            List<int> maxes = new List<int> ();

            // check the game outcome - 1 is a win, 0 is a loss
            int oneGameOutcome = GameOutcome;

            // the player won, go this way
            if (oneGameOutcome == 1)
            {
                // since the player won, check if any joins with player/movie combo already exists
                int? newPoints = HttpContext.Session.GetInt32 ("NewPoints");

                if (joinsList.Count > 0)
                {
                    // not first time the player attempted this movie
                    Console.WriteLine("MPJ_A -- WIN _____ NOT FIRST JOIN");
                    foreach (var item in joinsList)
                    {
                        // get current attempt count and count of joins
                        int currAttemptCount = item.AttemptCount;
                        maxes.Add (currAttemptCount);
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

                // first time player attempted this movie
                else
                {
                    Console.WriteLine("MPJ_B -- WIN _____ FIRST JOIN");

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
                    Console.WriteLine("MPJ_C -- LOSS _____ NOT FIRST JOIN");
                    foreach (var item in joinsList)
                    {
                        // get current attempt count and count of joins
                        int currAttemptCount = item.AttemptCount;
                        maxes.Add (currAttemptCount);
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
                    Console.WriteLine("MPJ_D -- LOSS _____ FIRST JOIN");

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
            // return Json (playerJoins);
        }


        // [Route ("update_player_games")]
        // [HttpPost("update_player_games")]
        public void UpdatePlayerGames (int gameOutcome, int playerId)
        {
            User retrievedPlayer = _context.Users.SingleOrDefault (p => p.UserId == playerId);

            // update number of games attempted
            var currGamesAttempted = retrievedPlayer.GamesAttempted;
            int newGamesAttempted = currGamesAttempted + 1;
            retrievedPlayer.GamesAttempted = newGamesAttempted;

            // if the player won, update games won
            if (gameOutcome == 1)
            {
                var currGamesWon = retrievedPlayer.GamesWon;
                int newGamesWon = currGamesWon + 1;
                retrievedPlayer.GamesWon = newGamesWon;
                CreateMovieUserJoin (1);
            }
            else
            {
                CreateMovieUserJoin (0);
            }
            retrievedPlayer.UpdatedAt = DateTime.Now;
            _context.SaveChanges ();
        }


        [HttpPost("update_player_points")]
        public void UpdatePlayerPoints (Clue clueInfo)
        {
            var playerId = HttpContext.Session.GetInt32 ("id");
            // Console.WriteLine($"UpdatePlayerPoints() --> {clueInfo.CluePoints}");

            _clueList.Add (clueInfo);

            // 1 means the player won, 0 means the player lost
            int oneGameOutcome = clueInfo.MovieId;

            if (oneGameOutcome > 0)
            {
                User retrievedPlayer = _context.Users.FirstOrDefault (p => p.UserId == playerId);

                // CURRENT POINTS --> players current points from DB
                // NEW POINTS --> value of clue movie was correctly guessed on; from 'UpdatePlayerPoints' js func.
                var currentPoints = retrievedPlayer.Points;
                int newPoints = clueInfo.CluePoints;
                HttpContext.Session.SetInt32 ("NewPoints", newPoints);

                // adds current points and new points; then saves them to the database
                retrievedPlayer.Points = newPoints + currentPoints;

                UpdatePlayerGames (1, (int) playerId);
            }
            else
            {
                UpdatePlayerGames (0, (int) playerId);
            }
            // return Json (clueInfo);
            // Console.WriteLine("UpdatePlayerPoints() --> redirecting to ViewSingleGameOverWinPage");
            // return RedirectToAction("ViewSingleGameOverWinPage", "PlaySingle");
        }
    }
}