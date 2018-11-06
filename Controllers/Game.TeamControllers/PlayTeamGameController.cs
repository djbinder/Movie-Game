using System;
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;
using movieGame.Controllers.Game.SingleControllers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace movieGame.Controllers.Game.TeamControllers
{
    [Route("group")]
    public class PlayTeamGameController : Controller
    {
        private MovieContext _context;

        public PlayTeamGameController (MovieContext context)
        {
            _context = context;
        }


        [HttpGet("")]
        public IActionResult PlayGroupGame()
        {
            ViewBag.FirstTeamName = GetTeamNameFromSession("FirstTeamName");
            ViewBag.SecondTeamName = GetTeamNameFromSession("SecondTeamName");
            return View("playgroup");
        }

        [HttpGet("add_teams")]
        public IActionResult ViewAddTeamPage()
        {
            return View("newteamform");
        }


        [HttpPost("set_team_names")]
        public async Task<IActionResult> SetTeamNames (string firstTeamName, string secondTeamName)
        {
            SetTeamNameInSession("FirstTeamName", firstTeamName);
            SetTeamNameInSession("SecondTeamName", secondTeamName);

            // Console.WriteLine($"team1 {firstTeamName}");
            // Console.WriteLine($"team2 {secondTeamName}");

            // await CreateNewTeam(firstTeamName);
            // await CreateNewTeam(secondTeamName);

            return RedirectToAction("PlayGroupGame");
        }


        public async Task<ActionResult> CreateNewTeam (string teamName)
        {
            var newTeam = new Team ()
            {
                TeamName = teamName,
                NumberOfPlayersOnTeam = 0,
                TeamPoints = 0,
                GamesPlayed = 0,
                CountOfMoviesGuessedCorrectly = 0,
                CountOfMoviesGuessedIncorrectly = 0,
                MovieTeamJoin = new List<MovieTeamJoin> (),
                GameTeamJoin = new List<GameTeamJoin> (),
                UserTeamJoin = new List<UserTeamJoin> ()
            };
            _context.Add (newTeam);
            await _context.SaveChangesAsync ();
            return Content("this is some content");
        }


        public void SetTeamNameInSession(string str, string teamName)
        {
            HttpContext.Session.SetString(str, teamName);
        }

        public string GetTeamNameFromSession(string str)
        {
            string teamName = HttpContext.Session.GetString(str);
            return teamName;
        }



    }
}