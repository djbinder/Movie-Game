using System;
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using movieGame.Infrastructure;
using movieGame.Controllers.MixedControllers;

namespace movieGame.Controllers.PlayTeamGameController
{
    [Route("group")]
    public class PlayTeamGameController : Controller
    {
        private static MovieContext _context;
        public Helpers _h = new Helpers();

        public string firstTeamNameKey = "FirstTeamName";
        public string firstTeamIdKey = "FirstTeamId";
        public string secondTeamNameKey = "SecondTeamName";
        public string secondTeamIdKey = "SecondTeamId";
        public string gameIdKey = "GameId";

        private static GetMovieInfoController _getMovie = new GetMovieInfoController(context: _context);

        public static GetMovieInfoController GetMovie { get => _getMovie; set => _getMovie = value; }

        public PlayTeamGameController (MovieContext context)
        {
            _context = context;
        }


        #region MANAGE ROUTING ------------------------------------------------------------

            [HttpGet("")]
            public async Task<IActionResult> StartGroupGame()
            {
                ViewBag.FirstTeamName = GetTeamNameFromSession(firstTeamNameKey);
                ViewBag.SecondTeamName = GetTeamNameFromSession(secondTeamNameKey);

                // GetGameIdFromSession();
                // ListAllSessionVariables();
                GetMoviesGuessedAlready();

                return View("playgroup");
            }

            [HttpGet("add_teams")]
            public IActionResult ViewAddTeamPage()
            {
                Console.WriteLine();
                Console.WriteLine("BEGINING NEW GAME");
                Console.WriteLine("--------------------");
                return View("newteamform");
            }

        #endregion MANAGE ROUTING ------------------------------------------------------------



        #region SET TEAMS ------------------------------------------------------------

            [HttpPost("set_group_game")]
            public async Task<IActionResult> SetGroupGame (string firstTeamName, string secondTeamName)
            {
                await CreateNewTeam(firstTeamName, firstTeamNameKey, firstTeamIdKey);
                await CreateNewTeam(secondTeamName, secondTeamNameKey, secondTeamIdKey);
                await CreateNewGame();

                await CreateMovieTeamJoin(GetTeamIdFromSession(firstTeamIdKey), 1, true);
                await CreateMovieTeamJoin(GetTeamIdFromSession(firstTeamIdKey), 4, true);
                await CreateMovieTeamJoin(GetTeamIdFromSession(secondTeamIdKey), 2, true);
                await CreateMovieTeamJoin(GetTeamIdFromSession(secondTeamIdKey), 7, true);

                return RedirectToAction("StartGroupGame");
            }

        #endregion SET TEAMS ------------------------------------------------------------



        #region SET/GET SESSION VARIABLES ------------------------------------------------------------

            // ----- TEAM NAMES AND IDS ----- //
                public void SetTeamNameInSession(string teamNameKey, string teamName)
                {
                    HttpContext.Session.SetString(teamNameKey, teamName);
                }

                public string GetTeamNameFromSession(string teamNameKey)
                {
                    string teamName = HttpContext.Session.GetString(teamNameKey);
                    return teamName;
                }


                public void SetTeamIdInSession(string teamIdKey, int teamId)
                {
                    HttpContext.Session.SetInt32(teamIdKey, teamId);
                }

                public int GetTeamIdFromSession(string teamIdKey)
                {
                    int teamId = (int)HttpContext.Session.GetInt32(teamIdKey);
                    // Console.WriteLine($"GetTeamIdFromSession() : {teamIdKey} 'teamId' --> {teamId}");
                    return teamId;
                }

                public void PrintTeamNamesAndIdsFromSession()
                {
                    string firstTeamName = GetTeamNameFromSession(firstTeamNameKey);
                    string secondTeamName = GetTeamNameFromSession(secondTeamNameKey);
                    int firstTeamId = GetTeamIdFromSession(firstTeamIdKey);
                    int secondTeamId = GetTeamIdFromSession(secondTeamIdKey);
                    Console.WriteLine($"GetMoviesGuessedAlready() : {firstTeamName} Id = {firstTeamId} and {secondTeamName} Id = {secondTeamId}");
                }

            // ----- GAME ID ----- //
                public void SetGameIdInSession(int gameId)
                {
                    HttpContext.Session.SetInt32(gameIdKey, gameId);
                }

                public int GetGameIdFromSession()
                {
                    int gameId = (int)HttpContext.Session.GetInt32(gameIdKey);
                    Console.WriteLine($"GetGameIdFromSession() : GameId = {gameId}");
                    return gameId;
                }

        #endregion SET/GET SESSION VARIABLES ------------------------------------------------------------



        #region CREATE NEW DATABASE RECORD ------------------------------------------------------------

            public async Task<Team> CreateNewTeam (string teamName, string teamNameKey, string teamIdKey)
            {
                var newTeam = new Team ()
                {
                    TeamName = teamName + AddSuffix(),
                    TeamPoints = 0,
                    GamesPlayed = 0,
                    CountOfMoviesGuessedCorrectly = 0,
                    CountOfMoviesGuessedIncorrectly = 0,
                    MovieTeamJoin = new List<MovieTeamJoin> (),
                    GameTeamJoin = new List<GameTeamJoin> (),
                };

                await AddAndSaveChangesAsync(newTeam);

                SetTeamNameInSession(teamNameKey, teamName);
                SetTeamIdInSession(teamIdKey, newTeam.TeamId);

                return newTeam;
            }

            public async Task<Models.Game> CreateNewGame()
            {
                // Team firstTeam = GetTeamInfo(GetTeamIdFromSession(firstTeamIdKey));
                // Team secondTeam = GetTeamInfo(GetTeamIdFromSession(secondTeamIdKey));
                Models.Game newGame = new Models.Game
                {
                    NumberOfTeamsInGame = 2,
                    FirstTeam = GetTeamInfo(GetTeamIdFromSession(firstTeamIdKey)),
                    SecondTeam = GetTeamInfo(GetTeamIdFromSession(secondTeamIdKey)),
                    GameTeamJoin = new List<GameTeamJoin>()
                };

                await AddAndSaveChangesAsync(newGame);
                SetGameIdInSession(newGame.GameId);
                return newGame;
            }

            public async Task<GameTeamJoin> CreateGameTeamJoin(int gameId, int teamId, bool winTrueOrFalse)
            {
                _h.StartMethod();
                GameTeamJoin newGameTeamJoin = new GameTeamJoin
                {
                    GameId = gameId,
                    TeamId = teamId,
                    ThisTeamWonId = 0,
                    WinFlag = false,
                    PointsReceived = 0,
                    ClueGameWonAt = 0,
                };
                newGameTeamJoin.TotalTimeTakenForGuesses = newGameTeamJoin.UpdatedAt - newGameTeamJoin.CreatedAt;

                await AddAndSaveChangesAsync(newGameTeamJoin);
                return newGameTeamJoin;
            }

            public async Task<MovieTeamJoin> CreateMovieTeamJoin(int teamId, int movieId, bool win)
            {
                MovieTeamJoin newMovieTeamJoin = new MovieTeamJoin
                {
                    TeamId = teamId,
                    MovieId = movieId,
                    WinFlag = win
                };

                if(win == true)
                    newMovieTeamJoin.PointsReceived = 5;
                    newMovieTeamJoin.ClueGameWonAt = 5;

                if(win == false)
                    newMovieTeamJoin.PointsReceived = 0;
                    newMovieTeamJoin.ClueGameWonAt = 0;

                await AddAndSaveChangesAsync(newMovieTeamJoin);
                return newMovieTeamJoin;
            }

            public async Task AddAndSaveChangesAsync(object obj)
            {
                _context.Add(obj);
                await _context.SaveChangesAsync();
            }

        #endregion CREATE NEW DATABASE RECORD ------------------------------------------------------------



        #region GET INFO FROM DATABASE ------------------------------------------------------------

            public Team GetTeamInfo(int teamId)
            {
                _h.StartMethod();
                Team thisTeam = _context.Teams.Include(gtj => gtj.GameTeamJoin).ThenInclude(g => g.Game).Include(mtj => mtj.MovieTeamJoin).ThenInclude(m => m.Movie).SingleOrDefault(team => team.TeamId == teamId);
                return thisTeam;
            }


            public Models.Game GetGameInfo()
            {
                _h.StartMethod();
                int thisGamesId = (int)HttpContext.Session.GetInt32(gameIdKey);
                Models.Game thisGame = _context.Games.Include(gtj => gtj.GameTeamJoin).ThenInclude(t => t.Team).SingleOrDefault(game => game.GameId == thisGamesId);
                // Models.Game thisGame = _context.Games.Include(gtj => gtj.GameTeamJoin).ThenInclude(t => t.Team).SingleOrDefault(game => game.GameId == gameId);
                return thisGame;
            }


            public List<int> GetMoviesGuessedAlready()
            {
                _h.StartMethod();

                var firstTeam = GetTeamInfo(GetTeamIdFromSession(firstTeamIdKey));
                var secondTeam = GetTeamInfo(GetTeamIdFromSession(secondTeamIdKey));

                List<int> movieIdsGuessed = new List<int>();

                IList<MovieTeamJoin> firstMovieTeamJoin = firstTeam.MovieTeamJoin;
                    AddMovieIdsToList(firstMovieTeamJoin, movieIdsGuessed);

                IList<MovieTeamJoin> secondMovieTeamJoin = secondTeam.MovieTeamJoin;
                    AddMovieIdsToList(secondMovieTeamJoin, movieIdsGuessed);

                return movieIdsGuessed;
            }

            public void AddMovieIdsToList(IList<MovieTeamJoin> mtj, List<int> listOfMovieIds)
            {
                foreach(var movie in mtj)
                {
                    listOfMovieIds.Add(movie.MovieId);
                    Console.Write($"AddMovieIdsToList() : {movie.MovieId}, ");
                }
            }

        #endregion GET INFO FROM DATABASE ------------------------------------------------------------



        #region CONNECT WITH JAVASCRIPT ------------------------------------------------------------

            [HttpGet("get_clue_number_from_javascript")]
            public JsonResult GetClueNumberFromJavaScript(int clueNumber)
            {
                Console.WriteLine($"get_clue_number_from_javascript --> {clueNumber}");
                return Json(clueNumber);
            }

        #endregion CONNECT WITH JAVASCRIPT ------------------------------------------------------------



        #region HELPERS ------------------------------------------------------------

            public string AddSuffix()
            {
                Random rand = new Random();
                string randomNumber = rand.Next(100).ToString();
                string suffix = DateTime.Now.Millisecond.ToString() + "-" + randomNumber;
                return suffix;
            }

            public void ListAllSessionVariables()
            {
                foreach(var key in HttpContext.Session.Keys)
                {
                    var value = HttpContext.Session.GetString(key);
                    Console.WriteLine($"K|V {key} = {value}");
                }
            }

            public void PrintMovieTeamJoin(IList<MovieTeamJoin> mtj)
            {
                foreach(var movie in mtj)
                {
                    Console.WriteLine(movie.MovieId);
                }
            }

        #endregion HELPERS ------------------------------------------------------------


    }
}




// public void SetTeamNamesInSession(string firstTeamName, string secondTeamName)
// {
//     _h.StartMethod();
//     SetTeamNameInSession(firstTeamNameKey, firstTeamName + AddSuffix());
//     SetTeamNameInSession(secondTeamNameKey, secondTeamName + AddSuffix());
// }

// public void SetTeamIdInSessionFromTeamName(string teamName, int teamId)
// {
//     _h.StartMethod();
//     HttpContext.Session.SetInt32(teamName, teamId);
//     Console.WriteLine($"SetTeamIdInSessionFromName() : 'teamName' = {teamName}  'teamId' = {teamId}");
//     // int firstTeamId = HttpContext.Session.GetInt32(teamName);
// }

// public void SetTeamIdsInSession(int firstTeamId, int secondTeamId)
// {
//     HttpContext.Session.SetInt32(firstTeamIdKey, firstTeamId);
//     HttpContext.Session.SetInt32(secondTeamIdKey, secondTeamId);
// }



// [HttpPost("set_game")]
// public async Task SetGame (string firstTeamName, string secondTeamName)
// {
//     _h.StartMethod();
//     await CreateNewGame();
// }



// await CreateMovieTeamJoin(GetTeamIdFromSession(firstTeamIdKey), 1, true);
// await CreateMovieTeamJoin(GetTeamIdFromSession(firstTeamIdKey), 4, true);
// await CreateMovieTeamJoin(GetTeamIdFromSession(secondTeamIdKey), 2, true);
// await CreateMovieTeamJoin(GetTeamIdFromSession(secondTeamIdKey), 7, true);