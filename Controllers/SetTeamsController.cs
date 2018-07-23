using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movieGame.Models;

namespace movieGame.Controllers
{
    public class SetTeamsController : Controller
    {
        private MovieContext _context;

        private string _start = "STARTED";
        private string _complete = "COMPLETED";


        public SetTeamsController (MovieContext context) {
            _context = context;
        }

        public string Start {
            get => _start;
            set => _start = value;
        }

        public string Complete {
            get => _complete;
            set => _complete = value;
        }


        [HttpPost]
        [Route("CreateNewTeam")]
        public ActionResult CreateNewTeam (string SubmittedTeamName)
        {
            Start.ThisMethod();

            var TeamName = SubmittedTeamName;

            var NewTeam = new Team()
            {
                TeamName = SubmittedTeamName,
                NumberOfPlayersOnTeam = 0,
                TeamPoints = 0,
                GamesPlayed = 0,
                CountOfMoviesGuessedCorrectly = 0,
                CountOfMoviesGuessedIncorrectly = 0,
                MovieTeamJoin = new List<MovieTeamJoin>(),
                GameTeamJoin = new List<GameTeamJoin>(),
                PlayerTeamJoin = new List<PlayerTeamJoin>()

            };

            var xTeams = ViewData.Model = _context.Teams.ToList();
            Console.WriteLine(xTeams);

            xTeams.Dig();

            _context.Add(NewTeam);
            _context.SaveChanges();

            Complete.ThisMethod();
            return View("PlayGroup");
        }


    }
}