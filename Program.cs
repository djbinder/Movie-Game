using System;
using Microsoft.AspNetCore;             // <--- 'WebHost'
using Microsoft.AspNetCore.Hosting;     // <--- 'IWebHost' and '.UseStartup<Startup>'
using Microsoft.Extensions.Logging;                 // <--- 'ILoggerFactory'




namespace movieGame {

    public class Program {

        static string Start = "STARTED";
        static string Complete = "COMPLETED";

        public static void Main (string[] args) {
            Start.ThisMethod();
            BuildWebHost (args).Run ();
            Complete.ThisMethod();
        }

        public static IWebHost BuildWebHost (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .UseStartup<Startup> ()
            .Build ();
            // .UseUrls(urls: "http://localhost:10003")

    }
}
