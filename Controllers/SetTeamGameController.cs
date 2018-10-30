using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using movieGame.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace movieGame.Controllers {
    public class SetTeamGameController : Controller {
        private MovieContext _context;

        public SetTeamGameController (MovieContext context) {
            _context = context;
        }

        [HttpGet]
        [Route ("NewTeamForm")]
        public async Task<IActionResult> ViewNewTeamForm () {
            await CreateTeamList ();
            return View ("NewTeamForm");
        }

        public async Task<IActionResult> CreateNewTeam (string SubmittedTeamName) {

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

            _context.Add (NewTeam);
            await _context.SaveChangesAsync ();

            return View ("PlayGroup");
        }

        public async Task<List<Team>> CreateTeamList () {
            var Teams = ViewBag.Teams = await _context.Teams.OrderBy (t => t.TeamName).ToListAsync ();

            return Teams;
        }

        [HttpGet]
        [Route ("team/{id}")]

        public async Task<IActionResult> JoinTeam (int id) {

            int? PlayerId = HttpContext.Session.GetInt32 ("id");
            PlayerId.Intro ("player id");

            PlayerTeamJoin PTJ = new PlayerTeamJoin {
                PlayerId = (int) PlayerId,
                TeamId = id
            };

            _context.Add (PTJ);
            await _context.SaveChangesAsync ();

            return RedirectToAction ("ViewNewTeamForm");
        }

        [HttpPost]
        [ProducesResponseType (200, Type = typeof (Game))]
        [ProducesResponseType (404)]
        public ActionResult<Game> CreateNewGame (int NumberOfTeams) {

            Game NewGame = new Game {
            DurationOfGame = 0,
            NumberOfTeamsInGame = NumberOfTeams,
            };

            _context.Add (NewGame);
            _context.SaveChanges ();

            return NewGame;
        }

    }

}