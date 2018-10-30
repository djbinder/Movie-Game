using System;
using movieGame.Models;
using Microsoft.AspNetCore.Mvc;

namespace movieGame.Controllers {
    public class PlayTeamGameController : Controller {
        private MovieContext _context;

        private PlaySingleController _playsinglecontroller;

        public PlayTeamGameController (MovieContext context) {
            _context = context;
        }

        protected PlayTeamGameController () { }

        public PlaySingleController Playsinglecontroller {
            get => _playsinglecontroller;
            set => _playsinglecontroller = value;
        }

        [HttpGet]
        [Route ("GroupGame")]
        public IActionResult ViewGroupGamePage () {

            return View ("PlayGroup");
        }

        [HttpGet]
        [Route ("InitiateGroupGame")]

        public IActionResult InitiateGroupGame () {
            return RedirectToAction ("ViewGroupGamePage");
        }

    }
}