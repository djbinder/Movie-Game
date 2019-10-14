using Microsoft.AspNetCore.Mvc;
using movieGame.Infrastructure;
using movieGame.Models;
using C = System.Console;

namespace Movie_Game.Controllers.ModelControllers
{
    public class GameController : Controller
    {
        private readonly Helpers          _helpers;
        private readonly MovieGameContext _context;


        public GameController(Helpers helpers, MovieGameContext context)
        {
            _helpers = helpers;
            _context = context;
        }
    }
}