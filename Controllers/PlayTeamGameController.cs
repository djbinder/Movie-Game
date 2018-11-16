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
using System.Collections;

namespace movieGame.Controllers.PlayTeamGameController
{
    [Route("group")]
    public class PlayTeamGameController : Controller
    {
        private MovieContext _context;

        public Helpers _h = new Helpers();

        public string firstTeamNameKey = "FirstTeamName";
        public string firstTeamIdKey = "FirstTeamId";
        public string secondTeamNameKey = "SecondTeamName";
        public string secondTeamIdKey = "SecondTeamId";
        public string gameIdKey = "GameId";
        public string movieIdKey = "MovieId";

        private readonly GetMovieInfoController _getMovieInfo;

        public PlayTeamGameController (MovieContext context, GetMovieInfoController getMovieInfo)
        {
            _context = context;
            _getMovieInfo = getMovieInfo;
        }


        #region MANAGE ROUTING ------------------------------------------------------------

            [HttpGet("")]
            public async Task<IActionResult> StartGroupGame()
            {
                ViewBag.FirstTeamName = GetTeamNameFromSession(firstTeamNameKey);
                ViewBag.SecondTeamName = GetTeamNameFromSession(secondTeamNameKey);

                // GetGameIdFromSession();
                // ListAllSessionVariables();
                // GetMoviesGuessedAlready();
                // GetAllMovieIdsInDatabase();
                // GetMovieIdsNotGuessed();
                SetThisGamesMovie(SetThisGamesMovieId());

                return View("playgroup");
            }

            [HttpGet("add_teams")]
            public IActionResult ViewAddTeamPage()
            {
                // Console.WriteLine();
                // Console.WriteLine("BEGINING NEW GAME");
                // Console.WriteLine("--------------------");
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



        #region SET MOVIE ------------------------------------------------------------

            public List<int> GetMovieIdsGuessedAlready()
            {
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
                mtj.ToList().ForEach(item => listOfMovieIds.Add(item.MovieId));
            }


            public List<int> GetAllMovieIdsInDatabase()
            {
                int numberOfMoviesInDatabase = _getMovieInfo.GetCountOfMoviesInDb();
                List<int> movieIdsInDatabase = new List<int>();
                for(var movieId = 1; movieId <= numberOfMoviesInDatabase; movieId++)
                {
                    movieIdsInDatabase.Add(movieId);
                }
                // PrintNumbersInList(movieIdsInDatabase, "GetAllMovieIdsInDatabase()", "movieIdsInDatabase");
                return movieIdsInDatabase;
            }


            public List<int> GetMovieIdsNotGuessed()
            {
                List<int> movieIdsGuessedList = GetMovieIdsGuessedAlready();
                List<int> movieIdsFromDatabase = GetAllMovieIdsInDatabase();
                List<int> eligibleMovieIds = new List<int>();

                for(var databaseId = 1; databaseId <= movieIdsFromDatabase.Count(); databaseId++)
                {
                    if(!movieIdsGuessedList.Contains(databaseId)) { eligibleMovieIds.Add(databaseId);}
                }
                return eligibleMovieIds;
            }


            public int PickRandomMovieIdFromList()
            {
                List<int> movieIdsNotGuessed = GetMovieIdsNotGuessed();
                Random random = new Random();
                int index = random.Next(movieIdsNotGuessed.Count);
                int randomMovieId = movieIdsNotGuessed[index];
                return randomMovieId;
            }


            public int SetThisGamesMovieId()
            {
                int thisGamesMovieId = PickRandomMovieIdFromList();
                SetMovieIdInSession(thisGamesMovieId);
                PrintThisGamesDetails();
                return thisGamesMovieId;
            }


            public Movie SetThisGamesMovie(int movieId)
            {
                Movie thisGamesMovie = _context.Movies.Include(c => c.Clues).SingleOrDefault(i => i.MovieId == movieId);
                return thisGamesMovie;
            }

        #endregion SET MOVIE ------------------------------------------------------------



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
                    return gameId;
                }

            // ----- MOVIE ID ----- //
                public void SetMovieIdInSession(int movieId)
                {
                    HttpContext.Session.SetInt32(movieIdKey, movieId);
                }

                public int GetMovieIdFromSession()
                {
                    var movieId = (int)HttpContext.Session.GetInt32(movieIdKey);
                    return movieId;
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

            [HttpGet("get_team_info")]
            public Team GetTeamInfo(int teamId)
            {
                Team thisTeam = _context.Teams.Include(gtj => gtj.GameTeamJoin).ThenInclude(g => g.Game).Include(mtj => mtj.MovieTeamJoin).ThenInclude(m => m.Movie).SingleOrDefault(team => team.TeamId == teamId);
                return thisTeam;
            }


            [HttpGet("get_game_info")]
            public Models.Game GetGameInfo()
            {
                int thisGamesId = (int)HttpContext.Session.GetInt32(gameIdKey);
                Models.Game thisGame = _context.Games.Include(gtj => gtj.GameTeamJoin).ThenInclude(t => t.Team).SingleOrDefault(game => game.GameId == thisGamesId);
                // Models.Game thisGame = _context.Games.Include(gtj => gtj.GameTeamJoin).ThenInclude(t => t.Team).SingleOrDefault(game => game.GameId == gameId);
                return thisGame;
            }


            [HttpGet("get_movie_info")]
            public Movie GetMovieInfo(int movieId)
            {
                Movie thisGamesMovie = _getMovieInfo.GetAllMovieInfo(GetMovieIdFromSession());
                return thisGamesMovie;
            }

            [HttpGet("get_movie_info_object")]
            public JsonResult GetMovieInfoObject(int movieId)
            {
                Movie thisGamesMovie = _getMovieInfo.GetAllMovieInfo(GetMovieIdFromSession());
                return Json(thisGamesMovie);
            }


            [HttpGet("get_movie_clues_list")]
            public List<Clue> GetMoviesCluesInfo(Movie movie)
            {
                List<Clue> thisMoviesClues = _getMovieInfo.GetMovieClues(movie);
                return thisMoviesClues;
            }

            [HttpGet("get_movie_clues_object")]
            public JsonResult GetMoviesClueObject()
            {
                Movie thisGamesMovie = GetMovieInfo(GetMovieIdFromSession());
                List<Clue> thisMoviesClue = GetMoviesCluesInfo(thisGamesMovie);

                return Json(thisGamesMovie);
            }

        #endregion GET INFO FROM DATABASE ------------------------------------------------------------




        #region CONNECT WITH JAVASCRIPT ------------------------------------------------------------

            [HttpGet("get_clue_number_from_javascript")]
            public JsonResult GetClueNumberFromJavaScript(int clueNumber)
            {
                List<Clue> thisMoviesClues = GetMoviesCluesInfo(GetMovieInfo(GetMovieIdFromSession()));

                var thisClue = thisMoviesClues[clueNumber - 1];
                Console.WriteLine($"Clue#: {thisClue.ClueDifficulty}. {thisClue.ClueText}");

                return Json(clueNumber);
            }

        #endregion CONNECT WITH JAVASCRIPT ------------------------------------------------------------




        #region HELPERS ------------------------------------------------------------

            public void PrintThisGamesDetails()
            {
                Game thisGame = GetGameInfo();
                    var thisGamesId = thisGame.GameId;
                    DateTime thisGameStartedAt = thisGame.CreatedAt;

                var thisGamesMovie = GetMovieInfo(GetMovieIdFromSession());
                    var thisGamesMovieId = thisGamesMovie.MovieId;
                    var thisGamesMovieTitle = thisGamesMovie.Title;
                    var thisGamesMovieYear = thisGamesMovie.Released;
                    var thisGamesMovieGenre = thisGamesMovie.Genre;
                    var thisGamesMovieDirector = thisGamesMovie.Director;

                var firstTeam = GetTeamInfo(GetTeamIdFromSession(firstTeamIdKey));
                    var firstTeamId = firstTeam.TeamId;
                    var firstTeamName = firstTeam.TeamName;

                var secondTeam = GetTeamInfo(GetTeamIdFromSession(secondTeamIdKey));
                    var secondTeamId = secondTeam.TeamId;
                    var secondTeamName = secondTeam.TeamName;

                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("BEGINNING NEW GAME");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine();

                Console.WriteLine($"GAME: {thisGamesId} @ {thisGameStartedAt}");
                Console.WriteLine();

                Console.WriteLine($"MOVIE: {thisGamesMovieId}. {thisGamesMovieTitle}");
                Console.WriteLine($"---> YEAR: {thisGamesMovieYear}");
                Console.WriteLine($"---> GENRE: {thisGamesMovieGenre}");
                Console.WriteLine($"---> DIRECTOR: {thisGamesMovieDirector}");
                Console.WriteLine();

                Console.WriteLine($"FIRST TEAM : {firstTeamId}. {firstTeamName}");
                Console.WriteLine($"SECOND TEAM: {secondTeamId}. {secondTeamName}");
                Console.WriteLine();

                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine();
            }


            public void PrintNumbersInList(List<int> thisList, string methodName, string itemName)
            {
                thisList.ForEach(item => Console.WriteLine($"{methodName} : {itemName} = {item}"));
            }


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



// public void AddMovieIdsToList(IList<MovieTeamJoin> mtj, List<int> listOfMovieIds)
// {
//     Console.Write($"AddMovieIdsToList() : ");
//     foreach(var movie in mtj)
//     {
//         listOfMovieIds.Add(movie.MovieId);
//         Console.WriteLine(movie.MovieId);
//     }
// }



// await CreateMovieTeamJoin(GetTeamIdFromSession(firstTeamIdKey), 1, true);
// await CreateMovieTeamJoin(GetTeamIdFromSession(firstTeamIdKey), 4, true);
// await CreateMovieTeamJoin(GetTeamIdFromSession(secondTeamIdKey), 2, true);
// await CreateMovieTeamJoin(GetTeamIdFromSession(secondTeamIdKey), 7, true);



// foreach(var guessedId in movieIdsGuessedList)
// {
//     for(var databaseId = 1; databaseId <= numberOfMoviesInDatabase; databaseId++)
//     {
//         if(guessedId == databaseId) {}
//         else
//         {
//             if(eligibleMovieIds.Contains(databaseId)) {}
//             else
//             {
//                 if(movieIdsGuessedList.Contains(databaseId)) {}
//                 else { eligibleMovieIds.Add(databaseId); }
// }}}}