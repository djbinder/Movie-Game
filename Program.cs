using System;
using Microsoft.AspNetCore;             // <--- 'WebHost'
using Microsoft.AspNetCore.Hosting;     // <--- 'IWebHost' and '.UseStartup<Startup>'




namespace movieGame {

    public class Program {

        static string Start = "STARTED";
        // static string Complete = "COMPLETED";

        public static void Main (string[] args) {
            Start.ThisMethod();
            Console.WriteLine($"Version: {Environment.Version}");
            BuildWebHost (args).Run ();
        }

        public static IWebHost BuildWebHost (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .UseStartup<Startup> ()
            .Build ();
    }
}
