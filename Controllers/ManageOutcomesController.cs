using System;
using System.Collections.Generic;       // <--- 'List'
// using System.Diagnostics;               // <-- 'StackFrame'
using System.Linq;                      // <--- various db queries (e.g., 'FirstOrDefault', 'OrderBy', etc.)
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Mvc;         // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using movieGame.Models;

namespace movieGame.Controllers
{
    public class ManageOutcomesController : Controller
    {
        private MovieContext _context;

        public ManageOutcomesController (MovieContext context) {
            _context = context;
        }

        #region 'SPOTLIGHT' and 'THISMETHOD' EXTENSION METHODS VARIABLES
            String Start = "STARTED";
            String Complete = "COMPLETED";
        #endregion

        // public int CheckSession()
        // {
        //     int? id = HttpContext.Session.GetInt32("id");

        //     if(id == null)
        //     {
        //         Extensions.Spotlight("start new session");
        //         return 0;
        //     }
        //     Extensions.Spotlight("continuing session");

        //     id.Intro("session id");

        //     return (int)id;
        // }

        List<Clue> _clueList = new List<Clue>();

        [HttpPost]
        [Route("createMoviePlayerJoin")]

        public JsonResult CreateMoviePlayerJoin (int GameOutcome)
        {
            Start.ThisMethod();

            int? playerId = HttpContext.Session.GetInt32("id");
            // playerId.Intro("player id");
            Player RetrievedPlayer = _context.Players.SingleOrDefault(p => p.PlayerId == playerId);

            // SESSION MOVIE ID --> id of current movie user was guessing
            var sessionMovieId = HttpContext.Session.GetInt32("sessionMovieId");

            // EXISTING JOINS --> System.Collections.Generic.List`1[movieGame.Models.MoviePlayerJoin]
            // JOINS LIST --> check if a join of player and movie already exists
            var PlayerJoins = _context.MoviePlayerJoin.Where(p => p.MovieId == sessionMovieId).Where(t => t.PlayerId == playerId);
            var JoinsList = PlayerJoins.ToList();

            List<int> Maxes = new List<int>();

            // check the game outcome - 1 is a win, 0 is a loss
            int oneGameOutcome = GameOutcome;
            // oneGameOutcome.Intro("one game outcome");

            // the player won, the join needs to reflect that
            if(oneGameOutcome == 1)
            {
                // since the player won, check if any joins with player/movie combo already exists
                int? newPoints = HttpContext.Session.GetInt32("newPoints");
                // newPoints.Intro("new points");

                if(JoinsList.Count > 0)
                {
                    // this is not the first time the player has attempted to guess this movie
                    Extensions.Spotlight("MPJ_A -- WIN _____ NOT FIRST JOIN");
                    foreach(var item in JoinsList)
                    {
                        // get current attempt count and count of joins
                        int currAttemptCount = item.AttemptCount;

                        // add them to lists to get maxes
                        Maxes.Add(currAttemptCount);

                        // intro
                        currAttemptCount.Intro("curr attempts count");
                    }

                    // get current max attempt of player/movie combo
                    var newAttemptsMax = Maxes.Max() + 1;

                    // MPJ --> create new many-to-many of player and movie
                    MoviePlayerJoin MPJ_A = new MoviePlayerJoin
                    {
                        PlayerId = (int)playerId,
                        MovieId = (int)sessionMovieId,
                        AttemptCount = newAttemptsMax,
                        PointsReceived = (int)newPoints,
                        WinLoss = true,
                    };

                    _context.Add(MPJ_A);
                }

                // this is the first time this player has attempted to guess this movie
                else
                {
                    Extensions.Spotlight("MPJ_B -- WIN _____ FIRST JOIN");

                    // MPJ --> create new many-to-many of player and movie
                    MoviePlayerJoin MPJ_B = new MoviePlayerJoin
                    {
                        PlayerId = (int)playerId,
                        MovieId = (int)sessionMovieId,
                        AttemptCount = 1,
                        PointsReceived = (int)newPoints,
                        WinLoss = true,
                    };

                    _context.Add(MPJ_B);
                }
            }

            // the player lost
            if(oneGameOutcome == 0)
            {
                // this is not the first time the player has attempted to guess this movie
                if(JoinsList.Count > 0)
                {
                    Extensions.Spotlight("MPJ_C -- LOSS _____ NOT FIRST JOIN");
                    foreach(var item in JoinsList)
                    {
                        // get current attempt count and count of joins
                        int currAttemptCount = item.AttemptCount;

                        // add them to lists to get maxes
                        Maxes.Add(currAttemptCount);

                        // intro
                        currAttemptCount.Intro("curr attempts count");
                    }

                    var newAttemptsMax = Maxes.Max() + 1;

                    // MPJ --> create new many-to-many of player and movie
                    MoviePlayerJoin MPJ_C = new MoviePlayerJoin
                    {
                        PlayerId = (int)playerId,
                        MovieId = (int)sessionMovieId,
                        AttemptCount = newAttemptsMax,
                        PointsReceived = 0,
                        WinLoss = false,
                    };

                    _context.Add(MPJ_C);
                }

                else
                {
                    Extensions.Spotlight("MPJ_D -- LOSS _____ FIRST JOIN");

                    // MPJ --> create new many-to-many of player and movie
                    MoviePlayerJoin MPJ_D = new MoviePlayerJoin
                    {
                        PlayerId = (int)playerId,
                        MovieId = (int)sessionMovieId,
                        AttemptCount = 1,
                        PointsReceived = 0,
                        WinLoss = false,
                    };

                    _context.Add(MPJ_D);
                }
            }

            _context.SaveChanges();

            Complete.ThisMethod();
            return Json(PlayerJoins);
        }


        [HttpPost]
        [Route("/updatePlayerGames")]
        public IActionResult UpdatePlayerGames (int gameOutcome, int playerId)
        {
            Start.ThisMethod();

            gameOutcome.Intro("this game outcome");

            Player RetrievedPlayer = _context.Players.SingleOrDefault(p => p.PlayerId == playerId);
            // var onePlayerId = RetrievedPlayer.PlayerId;

            // update number of games attempted
            var currGamesAttempted = RetrievedPlayer.GamesAttempted;
            int newGamesAttempted = currGamesAttempted + 1;
            newGamesAttempted.Intro("new games played");
            RetrievedPlayer.GamesAttempted = newGamesAttempted;

            // Extensions.TableIt(RetrievedPlayer, currGamesAttempted, newGamesAttempted);

            // if the player won, update games won
            if(gameOutcome == 1)
            {
                var currGamesWon = RetrievedPlayer.GamesWon;
                int newGamesWon = currGamesWon + 1;
                RetrievedPlayer.GamesWon = newGamesWon;
                newGamesWon.Intro("new games won");
                // Console.WriteLine("UP_GAMES  |  W  | send to C_MPJ");
                CreateMoviePlayerJoin(1);
            }

            else {
                // Console.WriteLine("UP_GAMES  |  L  | send to C_MPJ");
                CreateMoviePlayerJoin(0);
            }

            RetrievedPlayer.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            Complete.ThisMethod();
            return View();
        }


        [HttpPost]
        [Route("/updatePlayerPoints")]
        public JsonResult UpdatePlayerPoints (Clue clueInfo)
        {
            Start.ThisMethod();

            // PLAYER ID ---> the PlayerId of the current player (e.g., 8 OR 4 OR 12 etc.)
            var playerId = HttpContext.Session.GetInt32("id");

            // PLAYER --> the name of the current player
            // var player = HttpContext.Session.GetString("player");

            // SESSION MOVIE --> title of current movie user was guessing
            // var sessionMovie = HttpContext.Session.GetString("sessionMovieTitle");

            // SESSION MOVIE ID --> id of current movie user was guessing
            // var sessionMovieId = HttpContext.Session.GetInt32("sessionMovieId");

            // MOVIE LIST ---> System.Collections.Generic.List`1[movieGame.Models.Movie]
            _clueList.Add(clueInfo);

            // ONE GAME OUTCOME --> 1 means the player won, 0 means the player lost
            int oneGameOutcome = clueInfo.MovieId;

            if(oneGameOutcome > 0)
            {
                // RETRIEVED PLAYER ---> movieGame.Models.Player
                Player RetrievedPlayer = _context.Players.FirstOrDefault(p => p.PlayerId == playerId);

                // CURRENT POINTS --> players current points pulled from the database
                var currentPoints = RetrievedPlayer.Points;
                // currentPoints.Intro("current points");

                // NEW POINTS ---> the value of the clue the movie was correctly guessed on; retrieved from 'UpdatePlayerPoints' javascript function
                int newPoints = clueInfo.CluePoints;
                HttpContext.Session.SetInt32("newPoints", newPoints);
                // newPoints.Intro("new points to add");

                // RETRIEVED PLAYER POINT --> adds current points and new points; then saves them to the database
                RetrievedPlayer.Points = newPoints + currentPoints;

                UpdatePlayerGames(1, (int)playerId);
            }

            else
            {
                UpdatePlayerGames(0, (int)playerId);
            }

            Complete.ThisMethod();
            return Json(clueInfo);
        }

    }
}