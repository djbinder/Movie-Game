using System;
using Microsoft.AspNetCore;             // <--- 'WebHost'
using Microsoft.AspNetCore.Hosting;     // <--- 'IWebHost' and '.UseStartup<Startup>'
// using System.IO;
// using System.Threading.Tasks;
// using movieGame.Models;


namespace movieGame {
    public class Program {
        public static void Main (string[] args) {

            Console.WriteLine();
            Console.WriteLine("---------------'MAIN METHOD' STARTED---------------");

            BuildWebHost (args).Run ();

            Console.WriteLine("---------------'MAIN METHOD' COMPLETED---------------");
            Console.WriteLine();
        }

        public static IWebHost BuildWebHost (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .UseStartup<Startup> ()
            .UseUrls(urls: "http://localhost:10003")
            .Build ();

    }
}
