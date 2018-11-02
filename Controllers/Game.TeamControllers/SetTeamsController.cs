using System.Collections.Generic;
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;

namespace movieGame.Controllers.Game.TeamControllers
{
    public class SetTeamsController : Controller
    {
        private MovieContext _context;

        public SetTeamsController (MovieContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route ("CreateNewTeam")]
        public ActionResult CreateNewTeam (string submittedTeamName)
        {
            var teamName = submittedTeamName;

            var newTeam = new Team ()
            {
                TeamName = submittedTeamName,
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
            _context.SaveChanges ();

            return View ("PlayGroup");
        }

    }
}