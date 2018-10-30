using System.Collections.Generic;
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;

namespace movieGame.Controllers {
    public class SetTeamsController : Controller {
        private MovieContext _context;

        public SetTeamsController (MovieContext context) {
            _context = context;
        }

        [HttpPost]
        [Route ("CreateNewTeam")]
        public ActionResult CreateNewTeam (string SubmittedTeamName) {

            var TeamName = SubmittedTeamName;

            var NewTeam = new Team () {
                TeamName = SubmittedTeamName,
                NumberOfPlayersOnTeam = 0,
                TeamPoints = 0,
                GamesPlayed = 0,
                CountOfMoviesGuessedCorrectly = 0,
                CountOfMoviesGuessedIncorrectly = 0,
                MovieTeamJoin = new List<MovieTeamJoin> (),
                GameTeamJoin = new List<GameTeamJoin> (),
                PlayerTeamJoin = new List<PlayerTeamJoin> ()

            };

            // var xTeams = ViewData.Model = _context.Teams.ToList();
            // Console.WriteLine(xTeams);

            // xTeams.Dig();

            _context.Add (NewTeam);
            _context.SaveChanges ();

            return View ("PlayGroup");
        }

    }
}