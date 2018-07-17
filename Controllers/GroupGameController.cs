using System;
using Microsoft.AspNetCore.Mvc;
using movieGame.Models;

namespace movieGame.Controllers
{
    public class GroupGameController : Controller
    {
        private MovieContext _context;

        public GroupGameController (MovieContext context ) {
            _context = context;
        }

        String Start = "STARTED";
        String Complete = "COMPLETED";


        [HttpGet]
        [Route("GroupGame")]

        public IActionResult ViewGroupGamePage ()
        {
            Start.ThisMethod();

            Complete.ThisMethod();
            return View("GroupGame");
        }

    }
}