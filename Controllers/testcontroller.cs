// using System;
// using movieGame.Models;
// using Microsoft.AspNetCore.Mvc;
// using System.Reflection;

// namespace movieGame.Controllers
// {
//     public class testcontroller : Controller
//     {
//         private MovieContext _context;

//         public testcontroller (MovieContext context)
//         {
//             _context = context;
//         }

//         String Start = "STARTED";
//         String Complete = "COMPLETED";

//         [HttpGet]
//         [Route ("TestPage")]
//         public IActionResult ViewTestPage ()
//         {
//             Start.ThisMethod ();

//             //#region ----------- GET THE MEMBERS OF A TYPE -----------
//                 var members = typeof (object)
//                     .GetMembers (BindingFlags.Public |
//                                 BindingFlags.Static |
//                                 BindingFlags.Instance);

//                 foreach (var member in members)
//                 {
//                     bool inherited = member.DeclaringType.Equals (typeof (object).Name);

//                     Console.WriteLine ($"{member.Name} is a {member.MemberType}, " +
//                         $"it has {(inherited ? "":"not")} been inherited.");

//                     Console.WriteLine();
//                 }
//             //#endregion

//             Complete.ThisMethod ();
//             return View ("test");
//         }

//     }
// }