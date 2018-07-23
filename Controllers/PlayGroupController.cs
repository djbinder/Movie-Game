using System;
using Microsoft.AspNetCore.Mvc;
using movieGame.Models;

namespace movieGame.Controllers
{
    public class PlayGroupController : Controller
    {
        private MovieContext _context;

        private PlaySingleController _playsinglecontroller;

        public PlayGroupController (MovieContext context ) {
            _context = context;
        }

        String Start = "STARTED";
        String Complete = "COMPLETED";

        public PlaySingleController Playsinglecontroller {
            get => _playsinglecontroller;
            set => _playsinglecontroller = value;
        }


        [HttpGet]
        [Route("GroupGame")]
        public IActionResult ViewGroupGamePage ()
        {
            Start.ThisMethod();

            // Complete.ThisMethod();
            return View("PlayGroup");
        }


        [HttpGet]
        [Route("InitiateGroupGame")]

        public IActionResult InitiateGroupGame()
        {
            Start.ThisMethod();
            // Complete.ThisMethod();
            return RedirectToAction("ViewGroupGamePage");
        }

    }
}