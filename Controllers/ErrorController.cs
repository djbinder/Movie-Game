using Microsoft.AspNetCore.Mvc;

namespace movieGame.Controllers
{
    public class ErrorController : Controller
    {
        protected ErrorController()
        {
        }

        public IActionResult ShowErrorPage()
        {
            return View("~/Views/Shared/Error.cshtml");
            // return View("~/Views/Shared/Errors/Default.cshtml");
        }
    }
}