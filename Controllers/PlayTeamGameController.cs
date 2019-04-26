using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using movieGame.Controllers.MixedControllers;
using movieGame.Infrastructure;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

namespace movieGame.Controllers.PlayTeamGameController
{
#pragma warning disable CS1998
    [Route ("group")]
    public class PlayTeamGameController : Controller
    {
        private readonly MovieContext _context;
        // private MovieContext _context;
        public Helpers _h = new Helpers ();

        public string firstTeamNameKey = "FirstTeamName";
        public string firstTeamIdKey = "FirstTeamId";
        public string secondTeamNameKey = "SecondTeamName";
        public string secondTeamIdKey = "SecondTeamId";
        public string gameIdKey = "GameId";
        public string movieIdKey = "MovieId";
        public string roundIdKey = "RoundId";
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

        [HttpGet ("")]
        public async Task<IActionResult> ViewGroupGamePage ()
        {
            ViewBag.FirstTeamName = GetTeamNameFromSession (firstTeamNameKey);
            ViewBag.SecondTeamName = GetTeamNameFromSession (secondTeamNameKey);
            await InitializeNewRound ();
            PrintThisRoundsDetails ();

            int firstTeamPoints = ViewBag.FirstTeamPoints = GetTeamsCurrentPoints (firstTeamGameTeamJoinIdKey);
            int secondTeamPoints = ViewBag.SecondTeamPoints = GetTeamsCurrentPoints (secondTeamGameTeamJoinIdKey);

            Console.WriteLine ($"First Team Points: {firstTeamPoints}");
            Console.WriteLine ($"Second Team Points: {secondTeamPoints}");
            return View ("playgroup");
        }

        // this is referenced in the HTML
        [HttpGet ("add_teams_form")]
        public IActionResult ViewAddTeamPage ()
        {
            return View ("newteamform");
        }

        // this is referenced in javascript
        [HttpGet ("quit_movie_page")]
        public IActionResult ViewQuitCurrentMoviePage ()
        {
            Movie thisGamesMovie = GetMovieFromDatabase ();

            var movieTitle = ViewBag.MovieTitle = thisGamesMovie.Title;
            var moviePoster = ViewBag.Poster = thisGamesMovie.Poster;

            return View ("GroupGameOverQuitMovie");
        }

        // this is referenced in javascript
        [HttpGet ("correct_guess_page")]
        public IActionResult ViewCorrectGuessPage ()
        {
            Movie thisGamesMovie = GetMovieFromDatabase ();

            var movieTitle = ViewBag.MovieTitle = thisGamesMovie.Title;
            var moviePoster = ViewBag.Poster = thisGamesMovie.Poster;
            return View ("GroupGameOverCorrectGuess");
        }

        #endregion MANAGE ROUTING ------------------------------------------------------------

        #region INITIALIZE GAME & ROUND ------------------------------------------------------------

        // this is triggered in the 'NewTeamForm' html when the form is submitted
        [HttpPost ("initialize_game")]
        public async Task<IActionResult> InitializeNewGame (string firstTeamName, string secondTeamName)
        {
            await CreateNewTeam (firstTeamName, firstTeamNameKey, firstTeamIdKey);
            await CreateNewTeam (secondTeamName, secondTeamNameKey, secondTeamIdKey);
            await CreateNewGame ();
            await CreateGameTeamJoins ();
            await InitializeNewRound ();
            return RedirectToAction ("ViewGroupGamePage");
        }

        [HttpPost ("initialize_round")]
        public async Task InitializeNewRound ()
        {
            SetThisRoundsMovie (SetThisRoundsMovieId ());
            await CreateNewRound ();
        }

        #endregion INITIALIZE GAME & ROUND ------------------------------------------------------------

        #region PICK THE MOVIE ------------------------------------------------------------

        public List<int> GetMovieIdsGuessedAlready ()
        {
            Team firstTeam = GetTeamFromDatabase (GetTeamIdFromSession (firstTeamIdKey));
            Team secondTeam = GetTeamFromDatabase (GetTeamIdFromSession (secondTeamIdKey));

            List<int> movieIdsGuessed = new List<int> ();

            IList<MovieTeamJoin> firstMovieTeamJoin = firstTeam.MovieTeamJoin;
            AddMovieIdsToList (firstMovieTeamJoin, movieIdsGuessed);

            IList<MovieTeamJoin> secondMovieTeamJoin = secondTeam.MovieTeamJoin;
            AddMovieIdsToList (secondMovieTeamJoin, movieIdsGuessed);

            return movieIdsGuessed;
        }

        public void AddMovieIdsToList (IList<MovieTeamJoin> mtj, List<int> listOfMovieIds)
        {
            mtj.ToList ().ForEach (item => listOfMovieIds.Add (item.MovieId));
        }

        public List<int> GetListOfAllMovieIdsInDatabase ()
        {
            int numberOfMoviesInDatabase = _getMovieInfoFromDatabase.GetCountOfMoviesInDb ();
            List<int> movieIdsInDatabase = new List<int> ();
            for (var movieId = 1; movieId <= numberOfMoviesInDatabase; movieId++)
            {
                movieIdsInDatabase.Add (movieId);
            }
            return movieIdsInDatabase;
        }

        public List<int> GetListOfMovieIdsNotGuessed ()
        {
            List<int> movieIdsGuessedList = GetMovieIdsGuessedAlready ();
            List<int> movieIdsFromDatabase = GetListOfAllMovieIdsInDatabase ();
            List<int> eligibleMovieIds = new List<int> ();

            for (var databaseId = 1; databaseId <= movieIdsFromDatabase.Count (); databaseId++)
            {
                if (!movieIdsGuessedList.Contains (databaseId)) { eligibleMovieIds.Add (databaseId); }
            }
            return eligibleMovieIds;
        }

        public int PickRandomMovieIdFromList ()
        {
            List<int> movieIdsNotGuessed = GetListOfMovieIdsNotGuessed ();
            Random random = new Random ();
            int index = random.Next (movieIdsNotGuessed.Count);
            int randomMovieId = movieIdsNotGuessed[index];
            return randomMovieId;
        }

        public int SetThisRoundsMovieId ()
        {
            int thisRoundsMovieId = PickRandomMovieIdFromList ();
            SetMovieIdInSession (thisRoundsMovieId);
            return thisRoundsMovieId;
        }

        public Movie SetThisRoundsMovie (int movieId)
        {
            Movie thisRoundsMovie = _context.Movies.Include (c => c.Clues).SingleOrDefault (i => i.MovieId == movieId);
            return thisRoundsMovie;
        }

        #endregion PICK THE MOVIE ------------------------------------------------------------

        #region CREATE NEW DATABASE RECORD ------------------------------------------------------------

        [HttpPost ("create_team")]
        public async Task<Team> CreateNewTeam (string teamName, string teamNameKey, string teamIdKey)
        {
            var newTeam = new Team ()
            {
                TeamName = teamName + AddSuffix (),
                AllTimePoints = 0,
                AllTimeGamesPlayed = 0,
                AllTimeCountOfMoviesGuessedCorrectly = 0,
                AllTimeCountOfMoviesGuessedIncorrectly = 0,
                MovieTeamJoin = new List<MovieTeamJoin> (),
                GameTeamJoin = new List<GameTeamJoin> (),
            };
            await AddAndSaveChangesAsync (newTeam);
            SetTeamNameInSession (teamNameKey, newTeam.TeamName);
            SetTeamIdInSession (teamIdKey, newTeam.TeamId);
            return newTeam;
        }

        [HttpPost ("create_game")]
        public async Task<Models.Game> CreateNewGame ()
        {
            Models.Game newGame = new Models.Game
            {
                FirstTeam = GetTeamFromDatabase (GetTeamIdFromSession (firstTeamIdKey)),
                SecondTeam = GetTeamFromDatabase (GetTeamIdFromSession (secondTeamIdKey)),
                ListOfMoviesGuessedCorrectly = new List<Movie> (),
                ListOfMoviesQuit = new List<Movie> (),
                ListOfRounds = new List<Round> (),
                GameTeamJoin = new List<GameTeamJoin> ()
            };
            await AddAndSaveChangesAsync (newGame);
            SetGameIdInSession (newGame.GameId);
            return newGame;
        }

        [HttpPost ("create_round")]
        public async Task<Round> CreateNewRound ()
        {
            Round newRound = new Round
            {
                GameId = GetGameIdFromSession (),
                Game = GetGameFromDatabase (),
                Movie = GetMovieFromDatabase (),
                FirstTeam = GetTeamFromDatabase (GetTeamIdFromSession (firstTeamIdKey)),
                SecondTeam = GetTeamFromDatabase (GetTeamIdFromSession (secondTeamIdKey)),
                FirstTeamGuesses = 0,
                SecondTeamGuesses = 0,
                FirstTeamWinFlag = false,
                SecondTeamWinFlag = false,
                BothTeamsQuitFlag = false,
                FirstTeamPointsReceived = 0,
                SecondTeamPointsReceived = 0,
                ClueGameIsWonAt = 0,
            };
            await AddAndSaveChangesAsync (newRound);
            SetRoundIdInSession (newRound.RoundId);
            return newRound;
        }

        [HttpPost ("create_game_team_joins")]
        public async Task CreateGameTeamJoins ()
        {
            Console.WriteLine ($"Is there a current game in session? {isThereACurrentGameInSession}");

            if (isThereACurrentGameInSession == false)
            {
                await CreateGameTeamJoin (firstTeamIdKey, secondTeamIdKey, firstTeamGameTeamJoinIdKey);
                await CreateGameTeamJoin (secondTeamIdKey, firstTeamIdKey, secondTeamGameTeamJoinIdKey);

                isThereACurrentGameInSession = true;
                Console.WriteLine ($"Is there a current game in session? {isThereACurrentGameInSession}");
            }

            isThereACurrentGameInSession = true;
        }

        [HttpPost ("create_game_team_join")]
        public async Task<GameTeamJoin> CreateGameTeamJoin (string teamKey, string opponentKey, string teamGameTeamJoinIdKey)
        {
            GameTeamJoin newGameTeamJoin = new GameTeamJoin
            {
                GameId = GetGameIdFromSession (),
                TeamId = GetTeamIdFromSession (teamKey),
                OpponentId = GetTeamIdFromSession (opponentKey),
                WinFlag = false,
                TotalPoints = 0,
                ListOfMoviesGuessedCorrectly = new List<Movie> (),
            };
            _h.DigObj (newGameTeamJoin);
            await AddAndSaveChangesAsync (newGameTeamJoin);
            SetGameTeamJoinIdInSession (teamGameTeamJoinIdKey, newGameTeamJoin.GameTeamJoinId);
            return newGameTeamJoin;
        }

        // this is referenced in javascript
        [HttpPost ("create_movie_team_join")]
        public async Task<MovieTeamJoin> CreateMovieTeamJoin (int teamId, int movieId, int pointsReceived, int clueGameWonAt)
        {
            Console.WriteLine ("create_movie_team_join");
            Console.WriteLine ($"Team ID: {teamId}");
            Console.WriteLine ($"Movie ID: {movieId}");
            Console.WriteLine ($"Points Received: {pointsReceived}");
            Console.WriteLine ($"Clue Game Won At: {clueGameWonAt}");

            MovieTeamJoin newMovieTeamJoin = new MovieTeamJoin
            {
                TeamId = teamId,
                MovieId = movieId,
                GameId = GetGameIdFromSession ()
            };

            if (pointsReceived > 0)
            {
                Console.WriteLine ("Points Received > 0");
                newMovieTeamJoin.WinFlag = true;
                newMovieTeamJoin.PointsReceived = pointsReceived;
                newMovieTeamJoin.ClueGameWonAt = clueGameWonAt;
            }

            if (pointsReceived == 0)
            {
                Console.WriteLine ("Points Received = 0");
                newMovieTeamJoin.WinFlag = false;
                newMovieTeamJoin.PointsReceived = 0;
                newMovieTeamJoin.ClueGameWonAt = 0;
            }
            Console.WriteLine ($"newMovieTeamJoin.ID: {newMovieTeamJoin.MovieTeamJoinId}");
            await AddAndSaveChangesAsync (newMovieTeamJoin);
            return newMovieTeamJoin;
        }

        public async Task AddAndSaveChangesAsync (object obj)
        {
            _h.StartMethod ();
            _context.Add (obj);
            await _context.SaveChangesAsync ();
        }

        #endregion CREATE NEW DATABASE RECORD ------------------------------------------------------------

        #region GET INFO FROM DATABASE ------------------------------------------------------------

        [HttpGet ("get_team")]
        public Team GetTeamFromDatabase (int teamId)
        {
            Team thisTeam = _context.Teams.Include (gtj => gtj.GameTeamJoin).ThenInclude (g => g.Game).Include (mtj => mtj.MovieTeamJoin).ThenInclude (m => m.Movie).SingleOrDefault (team => team.TeamId == teamId);
            return thisTeam;
        }

        [HttpGet ("get_game")]
        public Models.Game GetGameFromDatabase ()
        {
            int thisGamesId = (int) HttpContext.Session.GetInt32 (gameIdKey);
            Models.Game thisGame = _context.Games.Include (gtj => gtj.GameTeamJoin).ThenInclude (t => t.Team).SingleOrDefault (game => game.GameId == thisGamesId);
            return thisGame;
        }

        [HttpGet ("get_movie")]
        public Movie GetMovieFromDatabase ()
        {
            Movie movie = _getMovieInfoFromDatabase.GetAllMovieInfo (GetMovieIdFromSession ());
            return movie;
        }

        // this is called by javascript
        [HttpGet ("get_movie_json")]
        public JsonResult GetMovieJsonFromDatabase ()
        {
            try
            {
                Movie thisGamesMovie = _getMovieInfoFromDatabase.GetAllMovieInfo (GetMovieIdFromSession ());
                return Json (thisGamesMovie);
            }

            catch (Exception ex) { Console.WriteLine ($"EXCEPTION MESSAGE: {ex.Message}"); }

            var blank = "blank";
            return Json (blank);
        }

        [HttpGet ("get_clues")]
        public List<Clue> GetCluesFromDatabase (Movie movie)
        {
            List<Clue> listOfClues = _getMovieInfoFromDatabase.GetMovieClues (movie);
            return listOfClues;
        }

        // this is called by javascript
        [HttpGet ("get_clues_object")]
        public JsonResult GetCluesFromDatabase ()
        {
            Movie thisGamesMovie = GetMovieFromDatabase ();
            return Json (thisGamesMovie);
        }

        [HttpGet ("get_round")]
        public Round GetRoundFromDatabase ()
        {
            Round currentRound = _context.Rounds.SingleOrDefault (r => r.RoundId == GetRoundIdFromSession ());
            return currentRound;
        }

        public GameTeamJoin GetGameTeamJoin (int gameTeamJoinId)
        {
            GameTeamJoin currentGtj = _context.GameTeamJoin.SingleOrDefault (gtj => gtj.GameTeamJoinId == gameTeamJoinId);
            return currentGtj;
        }

        public int GetTeamsCurrentPoints (string key)
        {
            GameTeamJoin currentGtj = GetGameTeamJoin (GetGameTeamJoinIdFromSession (key));
            int currentPoints = currentGtj.TotalPoints;
            return currentPoints;
        }

        #endregion GET INFO FROM DATABASE ------------------------------------------------------------

        #region UPDATE DATABASE RECORD ------------------------------------------------------------

        // this is referenced in javascript
        [HttpPost ("update_round")]
        public async Task UpdateRound (int firstTeamGuesses, int secondTeamGuesses)
        {
            Console.WriteLine ($"firstTeamGuesses: {firstTeamGuesses}");
            Console.WriteLine ($"secondTeamGuesses: {secondTeamGuesses}");
            Round currentRound = GetRoundFromDatabase ();
            currentRound.FirstTeamWinFlag = true;
            _context.Update (currentRound);
            await _context.SaveChangesAsync ();
        }

        // this is referenced in javascript
        [HttpPost ("update_game_team_join")]
        public async Task UpdateOneGameTeamJoin (int teamId, int pointsReceived)
        {
            var joinKey = GetGameTeamJoinIdFromSession (firstTeamGameTeamJoinIdKey);

            GameTeamJoin gameTeamJoin = await _context.GameTeamJoin.SingleOrDefaultAsync (gtj => gtj.GameTeamJoinId == joinKey);

            var teamCurrentPoints = gameTeamJoin.TotalPoints;

            var teamsUpdatedPoints = teamCurrentPoints + pointsReceived;
            gameTeamJoin.TotalPoints = teamsUpdatedPoints;

            var updatedTime = DateTime.Now;
            gameTeamJoin.UpdatedAt = updatedTime;

            _context.Update (gameTeamJoin);
            await _context.SaveChangesAsync ();
        }

        #endregion UPDATE DATABASE RECORD ------------------------------------------------------------

        #region CONNECT WITH JAVASCRIPT ------------------------------------------------------------

        // this is referenced in javascript
        [HttpGet ("get_clue_number_from_javascript")]
        public JsonResult GetClueNumberFromJavaScript (int clueNumber)
        {

            // _h.StartMethod();
            Console.WriteLine ($"CLUE_NUMBER: {clueNumber}");
            // List<Clue> thisMoviesClues = GetCluesFromDatabase(GetMovieFromDatabase());
            // GetNameOfTeamGuessing(clueNumber);
            // _h.CompleteMethod();
            return Json (clueNumber);
        }

        // this is referenced in javascript
        [HttpGet ("send_team_ids_to_javascript")]
        public JsonResult SendTeamIdsToJavaScript ()
        {
            ArrayList teamIdsArrayList = new ArrayList ();

            int firstTeamPoints = GetTeamsCurrentPoints (firstTeamIdKey);
            Console.WriteLine ($"1st team points {firstTeamPoints}");
            int secondTeamPoints = GetTeamsCurrentPoints (secondTeamIdKey);
            Console.WriteLine ($"2nd team points {secondTeamPoints}");

            teamIdsArrayList.Add (GetTeamIdFromSession (firstTeamIdKey));
            teamIdsArrayList.Add (GetTeamIdFromSession (secondTeamIdKey));
            teamIdsArrayList.Add (GetTeamNameFromSession (firstTeamNameKey));
            teamIdsArrayList.Add (GetTeamNameFromSession (secondTeamNameKey));
            teamIdsArrayList.Add (firstTeamPoints);
            teamIdsArrayList.Add (secondTeamPoints);
            foreach (var teamId in teamIdsArrayList)
            {
                Console.WriteLine (teamId);
            }
            return Json (teamIdsArrayList);
        }

        #endregion CONNECT WITH JAVASCRIPT ------------------------------------------------------------

        #region SET/GET SESSION VARIABLES ------------------------------------------------------------

        // ----- TEAM NAMES AND IDS ----- //
        public void SetTeamNameInSession (string teamNameKey, string teamName)
        {
            HttpContext.Session.SetString (teamNameKey, teamName);
        }

        public string GetTeamNameFromSession (string teamNameKey)
        {
            string teamName = HttpContext.Session.GetString (teamNameKey);
            return teamName;
        }

        public void SetTeamIdInSession (string teamIdKey, int teamId)
        {
            HttpContext.Session.SetInt32 (teamIdKey, teamId);
        }

        public int GetTeamIdFromSession (string teamIdKey)
        {
            int teamId = (int) HttpContext.Session.GetInt32 (teamIdKey);
            return teamId;
        }

        // ----- GAME ID ----- //
        public void SetGameIdInSession (int gameId)
        {
            HttpContext.Session.SetInt32 (gameIdKey, gameId);
        }

        public int GetGameIdFromSession ()
        {
            int gameId = (int) HttpContext.Session.GetInt32 (gameIdKey);
            return gameId;
        }

        // ----- MOVIE ID ----- //
        public void SetMovieIdInSession (int movieId)
        {
            HttpContext.Session.SetInt32 (movieIdKey, movieId);
        }

        public int GetMovieIdFromSession ()
        {
            var movieId = (int) HttpContext.Session.GetInt32 (movieIdKey);
            return movieId;
        }

        // ----- ROUND ID ----- //
        public void SetRoundIdInSession (int roundId)
        {
            HttpContext.Session.SetInt32 (roundIdKey, roundId);
        }

        public int GetRoundIdFromSession ()
        {
            int roundId = (int) HttpContext.Session.GetInt32 (roundIdKey);
            return roundId;
        }

        // ----- GAME TEAM JOIN ID ----- //
        public void SetGameTeamJoinIdInSession (string gameTeamJoinIdKey, int gameTeamJoinId)
        {
            HttpContext.Session.SetInt32 (gameTeamJoinIdKey, gameTeamJoinId);
            // int checkKey = (int)HttpContext.Session.GetInt32(gameTeamJoinIdKey);
            // Console.WriteLine($"Check Key: {checkKey}");
        }

        public int GetGameTeamJoinIdFromSession (string gameTeamJoinIdKey)
        {
            int gtjId = (int) HttpContext.Session.GetInt32 (gameTeamJoinIdKey);
            return gtjId;
        }

        #endregion SET/GET SESSION VARIABLES ------------------------------------------------------------

        #region HELPERS ------------------------------------------------------------

        public void PrintThisRoundsDetails ()
        {
            Game thisGame = GetGameFromDatabase ();
            var thisGamesId = thisGame.GameId;
            DateTime thisGameStartedAt = thisGame.CreatedAt;

            var thisGamesMovie = GetMovieFromDatabase ();
            var thisGamesMovieId = thisGamesMovie.MovieId;
            var thisGamesMovieTitle = thisGamesMovie.Title;
            var thisGamesMovieYear = thisGamesMovie.Released;
            var thisGamesMovieGenre = thisGamesMovie.Genre;
            var thisGamesMovieDirector = thisGamesMovie.Director;

            var firstTeam = GetTeamFromDatabase (GetTeamIdFromSession (firstTeamIdKey));
            var firstTeamId = firstTeam.TeamId;
            var firstTeamName = firstTeam.TeamName;

            var secondTeam = GetTeamFromDatabase (GetTeamIdFromSession (secondTeamIdKey));
            var secondTeamId = secondTeam.TeamId;
            var secondTeamName = secondTeam.TeamName;

            var firstTeamGameJoinId = GetGameTeamJoinIdFromSession (firstTeamGameTeamJoinIdKey);
            var secondTeamGameJoinId = GetGameTeamJoinIdFromSession (secondTeamGameTeamJoinIdKey);

            Console.WriteLine ();
            Console.WriteLine ("--------------------------------------------------");
            Console.WriteLine ("--------------------------------------------------");
            Console.WriteLine ("BEGINNING NEW GAME");
            Console.WriteLine ("--------------------------------------------------");
            Console.WriteLine ();

            Console.WriteLine ($"GAME: {thisGamesId} @ {thisGameStartedAt}");
            Console.WriteLine ();

            Console.WriteLine ($"MOVIE: {thisGamesMovieId}. {thisGamesMovieTitle}");
            Console.WriteLine ($"---> YEAR: {thisGamesMovieYear}");
            Console.WriteLine ($"---> GENRE: {thisGamesMovieGenre}");
            Console.WriteLine ($"---> DIRECTOR: {thisGamesMovieDirector}");
            Console.WriteLine ();

            Console.WriteLine ($"FIRST TEAM : {firstTeamId}. {firstTeamName}");
            Console.WriteLine ($"SECOND TEAM: {secondTeamId}. {secondTeamName}");
            Console.WriteLine ();

            Console.WriteLine ("--------------------------------------------------");
            Console.WriteLine ("--------------------------------------------------");
            Console.WriteLine ();
        }

        public void PrintRoundOutcomeInformation (int winningTeamId, int cluePoints, int movieId)
        {
            Console.WriteLine ();
            Console.WriteLine ();
            Console.WriteLine ("--------------------------------------------------");
            Console.WriteLine ("--------------------------------------------------");
            Console.WriteLine ("GAME OVER");
            Console.WriteLine ("--------------------------------------------------");
            Console.WriteLine ();
            Console.WriteLine ($"GAME ID: {GetGameIdFromSession()}");
            Console.WriteLine ($"---> MOVIE ID: {movieId}");
            Console.WriteLine ($"---> WINNING TEAM ID: {winningTeamId}");
            Console.WriteLine ($"---> POINTS WON: {cluePoints}");
            Console.WriteLine ();
            Console.WriteLine ("--------------------------------------------------");
            Console.WriteLine ("--------------------------------------------------");
        }

        public void PrintNumbersInList (List<int> thisList, string methodName, string itemName)
        {
            thisList.ForEach (item => Console.WriteLine ($"{methodName} : {itemName} = {item}"));
        }

        public string AddSuffix ()
        {
            Random rand = new Random ();
            string randomNumber = rand.Next (100).ToString ();
            string suffix = DateTime.Now.Millisecond.ToString () + "-" + randomNumber;
            return suffix;
        }

        public void ListAllSessionVariables ()
        {
            foreach (var key in HttpContext.Session.Keys)
            {
                var value = HttpContext.Session.GetString (key);
                Console.WriteLine ($"K|V {key} = {value}");
            }
        }

        public void PrintMovieTeamJoin (IList<MovieTeamJoin> mtj)
        {
            foreach (var movie in mtj)
            {
                Console.WriteLine (movie.MovieId);
            }
        }

        public void PrintTeamNamesAndIdsFromSession ()
        {
            string firstTeamName = GetTeamNameFromSession (firstTeamNameKey);
            string secondTeamName = GetTeamNameFromSession (secondTeamNameKey);
            int firstTeamId = GetTeamIdFromSession (firstTeamIdKey);
            int secondTeamId = GetTeamIdFromSession (secondTeamIdKey);
            Console.WriteLine ($"GetMoviesGuessedAlready() : {firstTeamName} Id = {firstTeamId} and {secondTeamName} Id = {secondTeamId}");
        }

        #endregion HELPERS ------------------------------------------------------------

    }
}

// public string GetNameOfTeamGuessing(int clueNumber)
// {
//     string guessingTeamName = "";

//     if(clueNumber == 1 || clueNumber == 4 || clueNumber == 5 || clueNumber == 8 || clueNumber == 9 )
//         guessingTeamName = HttpContext.Session.GetString(firstTeamNameKey);

//     if (clueNumber == 2 || clueNumber == 3 || clueNumber == 6 || clueNumber == 7 || clueNumber == 10)
//         guessingTeamName = HttpContext.Session.GetString(secondTeamNameKey);

//     // Console.WriteLine($"Guessing team: {guessingTeamName}");
//     return guessingTeamName;
// }

// [HttpPost("get_winning_game_outcome_from_javascript")]
// public void GetWinningGameOutcomeFromJavascript(int winningTeamId, int cluePoints, int movieId)
// {
//     PrintRoundOutcomeInformation(winningTeamId, cluePoints, movieId);
// }