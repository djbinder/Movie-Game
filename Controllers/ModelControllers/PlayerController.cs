using movieGame.Models;

namespace movieGame.Controllers.ModelControllers
{
    public class PlayerController
    {
        private readonly MovieContext _context;

        public PlayerController(MovieContext context)
        {
            _context = context;
        }

        public PlayerController() {}
    }
}
