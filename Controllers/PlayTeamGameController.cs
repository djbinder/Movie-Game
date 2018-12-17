using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movieGame.Infrastructure;
using movieGame.Controllers.MixedControllers;
using movieGame.Models;
using Newtonsoft.Json.Linq;

namespace movieGame.Controllers.PlayTeamGameController
{
    #pragma warning disable CS1998
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
        public string firstTeamGameTeamJoinIdKey = "FirstTeamGameTeamJoinId";
        public string secondTeamGameTeamJoinIdKey = "SecondTeamGameTeamJoinId";
        public bool isThereACurrentGameInSession = false;
        private readonly GetMovieInfoController _getMovieInfoFromDatabase;

        public PlayTeamGameController (MovieContext context, GetMovieInfoController getMovieInfoFromDatabase)
        {
            _context = context;
            _getMovieInfoFromDatabase = getMovieInfoFromDatabase;
        }




        #region MANAGE ROUTING ------------------------------------------------------------

            [HttpGet("")]
            public async Task<IActionResult> ViewGroupGamePage()
            {
                ViewBag.FirstTeamName = GetTeamNameFromSession(firstTeamNameKey);
                ViewBag.SecondTeamName = GetTeamNameFromSession(secondTeamNameKey);
                await InitializeNewGame();
                PrintThisGamesDetails();

                int firstTeamPoints = ViewBag.FirstTeamPoints = GetTeamsCurrentPoints(firstTeamGameTeamJoinIdKey);
                int secondTeamPoints = ViewBag.SecondTeamPoints = GetTeamsCurrentPoints(secondTeamGameTeamJoinIdKey);

                Console.WriteLine($"First Team Points: {firstTeamPoints}");
                Console.WriteLine($"Second Team Points: {secondTeamPoints}");
                return View("playgroup");
            }


            [HttpGet("add_teams_form")]
            public IActionResult ViewAddTeamPage()
            {
                return View("newteamform");
            }


            [HttpGet("quit_movie_page")]
            public IActionResult ViewQuitCurrentMoviePage()
            {
                int thisGamesMovieId = GetMovieIdFromSession();
                Movie thisGamesMovie = GetMovieInfoFromDatabase(thisGamesMovieId);

                var movieTitle = ViewBag.MovieTitle = thisGamesMovie.Title;
                var moviePoster = ViewBag.Poster = thisGamesMovie.Poster;

                return View("groupgameoverquitmovie");
            }


            [HttpGet("correct_guess_page")]
            public IActionResult ViewCorrectGuessPage()
            {
                Console.WriteLine("group guessed correctly");
                int thisGamesMovieId = GetMovieIdFromSession();
                Movie thisGamesMovie = GetMovieInfoFromDatabase(thisGamesMovieId);

                var movieTitle = ViewBag.MovieTitle = thisGamesMovie.Title;
                var moviePoster = ViewBag.Poster = thisGamesMovie.Poster;
                return View("groupgameovercorrectguess");
            }

        #endregion MANAGE ROUTING ------------------------------------------------------------




        #region SET TEAMS ------------------------------------------------------------

            // this is triggered in the 'NewTeamForm' html when the form is submitted
            [HttpPost("initialize_matchup")]
            public async Task<IActionResult> InitializeNewMatchup(string firstTeamName, string secondTeamName)
            {
                await CreateNewTeam(firstTeamName, firstTeamNameKey, firstTeamIdKey);
                await CreateNewTeam(secondTeamName, secondTeamNameKey, secondTeamIdKey);
                return RedirectToAction("ViewGroupGamePage");
            }


            [HttpPost("initialize_game")]
            public async Task InitializeNewGame()
            {
                SetThisGamesMovie(SetThisGamesMovieId());
                await CreateNewGame();
                await CreateGameTeamJoins();
            }

        #endregion SET TEAMS ------------------------------------------------------------




        #region SET MOVIE ------------------------------------------------------------

            public List<int> GetMovieIdsGuessedAlready()
            {
                var firstTeam = GetTeamInfoFromDatabase(GetTeamIdFromSession(firstTeamIdKey));
                var secondTeam = GetTeamInfoFromDatabase(GetTeamIdFromSession(secondTeamIdKey));

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


            public List<int> GetListOfAllMovieIdsInDatabase()
            {
                int numberOfMoviesInDatabase = _getMovieInfoFromDatabase.GetCountOfMoviesInDb();
                List<int> movieIdsInDatabase = new List<int>();
                for(var movieId = 1; movieId <= numberOfMoviesInDatabase; movieId++)
                {
                    movieIdsInDatabase.Add(movieId);
                }
                // PrintNumbersInList(movieIdsInDatabase, "GetListOfAllMovieIdsInDatabase()", "movieIdsInDatabase");
                return movieIdsInDatabase;
            }


            public List<int> GetListOfMovieIdsNotGuessed()
            {
                List<int> movieIdsGuessedList = GetMovieIdsGuessedAlready();
                List<int> movieIdsFromDatabase = GetListOfAllMovieIdsInDatabase();
                List<int> eligibleMovieIds = new List<int>();

                for(var databaseId = 1; databaseId <= movieIdsFromDatabase.Count(); databaseId++)
                {
                    if(!movieIdsGuessedList.Contains(databaseId)) { eligibleMovieIds.Add(databaseId);}
                }
                return eligibleMovieIds;
            }


            public int PickRandomMovieIdFromList()
            {
                List<int> movieIdsNotGuessed = GetListOfMovieIdsNotGuessed();
                Random random = new Random();
                int index = random.Next(movieIdsNotGuessed.Count);
                int randomMovieId = movieIdsNotGuessed[index];
                return randomMovieId;
            }


            public int SetThisGamesMovieId()
            {
                int thisGamesMovieId = PickRandomMovieIdFromList();
                SetMovieIdInSession(thisGamesMovieId);
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

            // ----- GAME TEAM JOIN ID ----- //
                public void SetGameTeamJoinIdInSession(string gameTeamJoinIdKey, int gameTeamJoinId)
                {
                    HttpContext.Session.SetInt32(gameTeamJoinIdKey, gameTeamJoinId);
                    // int checkKey = (int)HttpContext.Session.GetInt32(gameTeamJoinIdKey);
                    // Console.WriteLine($"Check Key: {checkKey}");
                }

                public int GetGameTeamJoinIdFromSession(string gameTeamJoinIdKey)
                {
                    int gtjId = (int)HttpContext.Session.GetInt32(gameTeamJoinIdKey);
                    return gtjId;
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
                SetTeamNameInSession(teamNameKey, newTeam.TeamName);
                SetTeamIdInSession(teamIdKey, newTeam.TeamId);
                return newTeam;
            }


            public async Task<Models.Game> CreateNewGame()
            {
                Models.Game newGame = new Models.Game
                {
                    FirstTeam = GetTeamInfoFromDatabase(GetTeamIdFromSession(firstTeamIdKey)),
                    SecondTeam = GetTeamInfoFromDatabase(GetTeamIdFromSession(secondTeamIdKey)),
                    MoviesGuessedCorrectly = new List<Movie>(),
                    GameTeamJoin = new List<GameTeamJoin>()
                };
                await AddAndSaveChangesAsync(newGame);
                SetGameIdInSession(newGame.GameId);
                return newGame;
            }


            [HttpPost("create_game_team_joins")]
            public async Task CreateGameTeamJoins()
            {
                Console.WriteLine($"Is there a current game in session? {isThereACurrentGameInSession}");

                if(isThereACurrentGameInSession == false)
                {
                    await CreateGameTeamJoin(firstTeamIdKey, secondTeamIdKey, firstTeamGameTeamJoinIdKey);
                    await CreateGameTeamJoin(secondTeamIdKey, firstTeamIdKey, secondTeamGameTeamJoinIdKey);

                    isThereACurrentGameInSession = true;
                    Console.WriteLine($"Is there a current game in session? {isThereACurrentGameInSession}");
                }

                isThereACurrentGameInSession = true;
            }


            [HttpPost("create_game_team_join")]
            public async Task<GameTeamJoin> CreateGameTeamJoin(string teamKey, string opponentKey, string teamGameTeamJoinIdKey)
            {
                GameTeamJoin newGameTeamJoin = new GameTeamJoin
                {
                    GameId = GetGameIdFromSession(),
                    TeamId = GetTeamIdFromSession(teamKey),
                    OpponentId = GetTeamIdFromSession(opponentKey),
                    WinFlag = false,
                    TotalPoints = 0,
                    TotalTimeTakenForGuesses = TimeSpan.Zero,
                    ListOfMoviesGuessedCorrectly = new List<Movie>(),
                };
                await AddAndSaveChangesAsync(newGameTeamJoin);
                SetGameTeamJoinIdInSession(teamGameTeamJoinIdKey, newGameTeamJoin.GameTeamJoinId);
                return newGameTeamJoin;
            }


            [HttpPost("create_movie_team_join")]
            public async Task<MovieTeamJoin> CreateMovieTeamJoin(int teamId, int movieId, int pointsReceived, int clueGameWonAt)
            {
                MovieTeamJoin newMovieTeamJoin = new MovieTeamJoin
                {
                    TeamId = teamId,
                    MovieId = movieId,
                    GameId = GetGameIdFromSession()
                };

                if(pointsReceived > 0)
                {
                    newMovieTeamJoin.WinFlag = true;
                    newMovieTeamJoin.PointsReceived = pointsReceived;
                    newMovieTeamJoin.ClueGameWonAt = clueGameWonAt;
                }

                if(pointsReceived == 0)
                {
                    newMovieTeamJoin.WinFlag = false;
                    newMovieTeamJoin.PointsReceived = 0;
                    newMovieTeamJoin.ClueGameWonAt = 0;
                }
                Console.WriteLine($"newMovieTeamJoin.WinFlag: {newMovieTeamJoin.WinFlag}");
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
            public Team GetTeamInfoFromDatabase(int teamId)
            {
                Team thisTeam = _context.Teams.Include(gtj => gtj.GameTeamJoin).ThenInclude(g => g.Game).Include(mtj => mtj.MovieTeamJoin).ThenInclude(m => m.Movie).SingleOrDefault(team => team.TeamId == teamId);
                return thisTeam;
            }


            [HttpGet("get_game_info")]
            public Models.Game GetGameInfoFromDatabase()
            {
                int thisGamesId = (int)HttpContext.Session.GetInt32(gameIdKey);
                Models.Game thisGame = _context.Games.Include(gtj => gtj.GameTeamJoin).ThenInclude(t => t.Team).SingleOrDefault(game => game.GameId == thisGamesId);
                return thisGame;
            }


            [HttpGet("get_movie_info")]
            public Movie GetMovieInfoFromDatabase(int movieId)
            {
                Movie thisGamesMovie = _getMovieInfoFromDatabase.GetAllMovieInfo(GetMovieIdFromSession());
                return thisGamesMovie;
            }


            [HttpGet("get_movie_info_object")]
            public JsonResult GetMovieInfoObjectFromDatabase(int movieId)
            {
                var blank = "blank";

                try {
                    Movie thisGamesMovie = _getMovieInfoFromDatabase.GetAllMovieInfo(GetMovieIdFromSession());
                    return Json(thisGamesMovie);
                }

                catch(Exception ex)
                {
                    Console.WriteLine($"EXCEPTION MESSAGE: {ex.Message}");
                }
                return Json(blank);
            }


            [HttpGet("get_movie_clues_list")]
            public List<Clue> GetMoviesCluesInfoFromDatabase(Movie movie)
            {
                List<Clue> thisMoviesClues = _getMovieInfoFromDatabase.GetMovieClues(movie);
                return thisMoviesClues;
            }


            [HttpGet("get_movie_clues_object")]
            public JsonResult GetMoviesClueObjectFromDatabase()
            {
                Movie thisGamesMovie = GetMovieInfoFromDatabase(GetMovieIdFromSession());
                List<Clue> thisMoviesClue = GetMoviesCluesInfoFromDatabase(thisGamesMovie);
                return Json(thisGamesMovie);
            }


            public GameTeamJoin GetGameTeamJoin(int gameTeamJoinId)
            {
                GameTeamJoin currentGtj = _context.GameTeamJoin.SingleOrDefault(gtj => gtj.GameTeamJoinId == gameTeamJoinId);

                return currentGtj;
            }


            public int GetTeamsCurrentPoints(string key)
            {
                GameTeamJoin currentGtj = GetGameTeamJoin(GetGameTeamJoinIdFromSession(key));

                int currentPoints = currentGtj.TotalPoints;
                return currentPoints;
            }

        #endregion GET INFO FROM DATABASE ------------------------------------------------------------




        #region UPDATE DATABASE RECORD ------------------------------------------------------------

            [HttpPost("update_game_team_join")]
            public async Task UpdateOneGameTeamJoin(int teamId, int pointsReceived, string gameTeamJoinIdKey)
            {
                _h.StartMethod();
                var joinKey = GetGameTeamJoinIdFromSession(firstTeamGameTeamJoinIdKey);
                // var firstTeamGameJoinId = GetGameTeamJoinIdFromSession(firstTeamGameTeamJoinIdKey);

                GameTeamJoin gameTeamJoin = await _context.GameTeamJoin.SingleOrDefaultAsync(gtj => gtj.GameTeamJoinId == joinKey);

                var teamCurrentPoints = gameTeamJoin.TotalPoints;

                var teamsUpdatedPoints = teamCurrentPoints + pointsReceived;
                gameTeamJoin.TotalPoints = teamsUpdatedPoints;

                var updatedTime = DateTime.Now;
                gameTeamJoin.UpdatedAt = updatedTime;

                _context.Update(gameTeamJoin);
                await _context.SaveChangesAsync();
            }

        #endregion UPDATE DATABASE RECORD ------------------------------------------------------------




        #region CONNECT WITH JAVASCRIPT ------------------------------------------------------------

            [HttpGet("get_clue_number_from_javascript")]
            public JsonResult GetClueNumberFromJavaScript(int clueNumber)
            {
                List<Clue> thisMoviesClues = GetMoviesCluesInfoFromDatabase(GetMovieInfoFromDatabase(GetMovieIdFromSession()));

                var thisClue = thisMoviesClues[clueNumber - 1];
                // Console.WriteLine($"Clue#: {thisClue.ClueDifficulty}. {thisClue.ClueText}");

                // GetNameOfTeamGuessing(clueNumber);

                return Json(clueNumber);
            }


            public string GetNameOfTeamGuessing(int clueNumber)
            {
                string guessingTeamName = "";

                if(clueNumber == 1 || clueNumber == 4 || clueNumber == 5 || clueNumber == 8 || clueNumber == 9 )
                    guessingTeamName = HttpContext.Session.GetString(firstTeamNameKey);

                if (clueNumber == 2 || clueNumber == 3 || clueNumber == 6 || clueNumber == 7 || clueNumber == 10)
                    guessingTeamName = HttpContext.Session.GetString(secondTeamNameKey);

                Console.WriteLine($"Guessing team: {guessingTeamName}");
                return guessingTeamName;
            }


            [HttpPost("get_winning_game_outcome_from_javascript")]
            public void GetWinningGameOutcomeFromJavascript(int winningTeamId, int cluePoints, int movieId)
            {
                PrintGameOutcomeInformation(winningTeamId, cluePoints, movieId);
            }


            [HttpGet("send_team_ids_to_javascript")]
            public JsonResult SendTeamIdsToJavaScript()
            {
                // var firstTeamId = GetTeamIdFromSession(firstTeamIdKey);
                // var secondTeamId = GetTeamIdFromSession(secondTeamIdKey);
                ArrayList teamIdsArrayList = new ArrayList();
                teamIdsArrayList.Add(GetTeamIdFromSession(firstTeamIdKey));
                teamIdsArrayList.Add(GetTeamIdFromSession(secondTeamIdKey));
                return Json(teamIdsArrayList);
            }

        #endregion CONNECT WITH JAVASCRIPT ------------------------------------------------------------




        #region HELPERS ------------------------------------------------------------

            public void PrintThisGamesDetails()
            {
                Game thisGame = GetGameInfoFromDatabase();
                    var thisGamesId = thisGame.GameId;
                    DateTime thisGameStartedAt = thisGame.CreatedAt;

                var thisGamesMovie = GetMovieInfoFromDatabase(GetMovieIdFromSession());
                    var thisGamesMovieId = thisGamesMovie.MovieId;
                    var thisGamesMovieTitle = thisGamesMovie.Title;
                    var thisGamesMovieYear = thisGamesMovie.Released;
                    var thisGamesMovieGenre = thisGamesMovie.Genre;
                    var thisGamesMovieDirector = thisGamesMovie.Director;

                var firstTeam = GetTeamInfoFromDatabase(GetTeamIdFromSession(firstTeamIdKey));
                    var firstTeamId = firstTeam.TeamId;
                    var firstTeamName = firstTeam.TeamName;

                var secondTeam = GetTeamInfoFromDatabase(GetTeamIdFromSession(secondTeamIdKey));
                    var secondTeamId = secondTeam.TeamId;
                    var secondTeamName = secondTeam.TeamName;

                var firstTeamGameJoinId = GetGameTeamJoinIdFromSession(firstTeamGameTeamJoinIdKey);
                var secondTeamGameJoinId = GetGameTeamJoinIdFromSession(secondTeamGameTeamJoinIdKey);

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

                Console.WriteLine($"FIRST TEAM / GAME JOIN ID: {firstTeamGameJoinId}");
                // Console.WriteLine($"SECOND TEAM / GAME JOIN ID: {secondTeamGameJoinId}");

                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine();
            }


            public void PrintGameOutcomeInformation(int winningTeamId, int cluePoints, int movieId)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("GAME OVER");
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine();
                Console.WriteLine($"GAME ID: {GetGameIdFromSession()}");
                Console.WriteLine($"---> MOVIE ID: {movieId}");
                Console.WriteLine($"---> WINNING TEAM ID: {winningTeamId}");
                Console.WriteLine($"---> POINTS WON: {cluePoints}");
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------");
                Console.WriteLine("--------------------------------------------------");
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




// GameTeamJoin initialGameFirstTeamJoin = new GameTeamJoin
// {
//     GameId = GetGameIdFromSession(),
//     TeamId = GetTeamIdFromSession(firstTeamIdKey),
//     OpponentId = GetTeamIdFromSession(secondTeamIdKey),
//     WinFlag = false,
//     TotalPoints = 0,
//     TotalTimeTakenForGuesses = TimeSpan.Zero,
//     ListOfMoviesGuessedCorrectly = new List<Movie>(),
// };

// await AddAndSaveChangesAsync(initialGameFirstTeamJoin);
// SetGameTeamJoinIdInSession(firstTeamGameTeamJoinIdKey, initialGameFirstTeamJoin.GameTeamJoinId);



// GameTeamJoin initialGameSecondTeamJoin = new GameTeamJoin
// {
//     GameId = GetGameIdFromSession(),
//     TeamId = GetTeamIdFromSession(secondTeamIdKey),
//     OpponentId = GetTeamIdFromSession(firstTeamIdKey),
//     WinFlag = false,
//     TotalPoints = 0,
//     TotalTimeTakenForGuesses = TimeSpan.Zero,
//     ListOfMoviesGuessedCorrectly = new List<Movie>(),
// };

// await AddAndSaveChangesAsync(initialGameSecondTeamJoin);
// SetGameTeamJoinIdInSession(secondTeamGameTeamJoinIdKey, initialGameSecondTeamJoin.GameTeamJoinId);




// [HttpPost("create_game_team_join")]
// public async Task<GameTeamJoin> CreateGameTeamJoin(int teamId, bool winLoseBool, int pointsReceived, int clueGameWonAt)
// {
//     Console.WriteLine($"Team Id: {teamId}");
//     Console.WriteLine($"Win?: {winLoseBool}");
//     Console.WriteLine($"Points Received: {pointsReceived}");
//     Console.WriteLine($"Game won at Clue #: {clueGameWonAt}");
//     Console.WriteLine();

//     GameTeamJoin newGameTeamJoin = new GameTeamJoin
//     {
//         GameId = GetGameIdFromSession(),
//         TeamId = teamId,
//         WinFlag = winLoseBool,
//         PointsReceived = pointsReceived,
//         ClueGameWonAt = clueGameWonAt,
//     };

//     await AddAndSaveChangesAsync(newGameTeamJoin);
//     return newGameTeamJoin;
// }


// [HttpPost("set_group_game")]
// public async Task<IActionResult> SetGroupGame (string firstTeamName, string secondTeamName)
// {
//     // await CreateNewTeam(firstTeamName, firstTeamNameKey, firstTeamIdKey);
//     // await CreateNewTeam(secondTeamName, secondTeamNameKey, secondTeamIdKey);
//     await CreateNewGame();

//     return RedirectToAction("StartGroupGame");
// }


// [HttpPost("select_next_movie")]
// public async Task<IActionResult> StartNextGame()
// {
//     ViewBag.FirstTeamName = GetTeamNameFromSession(firstTeamNameKey);
//     ViewBag.SecondTeamName = GetTeamNameFromSession(secondTeamNameKey);
//     SetThisGamesMovie(SetThisGamesMovieId());
//     await CreateNewGame();
//     return View("playgroup");
// }