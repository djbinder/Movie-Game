using System;
using Microsoft.AspNetCore.Http;        // <--- set session variables (e.g., 'SetString', 'SetInt32', etc.)
using Microsoft.AspNetCore.Mvc;         // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')

using movieGame.Models;

namespace movieGame.Controllers
{
    public class HomeController : Controller
    {
        private MovieContext _context;

        public HomeController (MovieContext context ) {
            _context = context;
        }

        String Start = "STARTED";
        String Complete = "COMPLETED";


        public int CheckSession()
        {
            Start.ThisMethod();
            int? id = HttpContext.Session.GetInt32("id");

            if(id == null)
            {
                Console.WriteLine($"start new session with id {id}");
                return 0;
            }

            HttpContext.Session.SetInt32("id", (int)id);
            Console.WriteLine($"continuing session with id {id}");

            Complete.ThisMethod();
            return (int)id;
        }


        [HttpGet]
        [Route("LogPlayerOut")]
        public IActionResult LogPlayerOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        // clear session
        [HttpGet]
        [Route("ClearSession")]
        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
}}