using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace movieGame {

    public class Program {

        public static void Main (string[] args) {
            Console.WriteLine ($"Version: {Environment.Version}");
            CreateWebHostBuilder (args).Build ().Run ();
        }

        public static IWebHostBuilder CreateWebHostBuilder (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .UseStartup<Startup> ();
    }
}

// 2.0 version
// public static void Main (string[] args) {
//     Start.ThisMethod();

//     Console.WriteLine($"Version: {Environment.Version}");
//     BuildWebHost (args).Run ();
// }

// public static IWebHost BuildWebHost (string[] args) =>
//     WebHost.CreateDefaultBuilder (args)
//     .UseStartup<Startup> ()
//     .Build ();

// without azure
// public static IWebHost BuildWebHost (string[] args) =>
// WebHost.CreateDefaultBuilder (args)
// .UseStartup<Startup> ()
//.Build ();

// WITH azure deployment
// public static IWebHost BuildWebHost(string[] args) =>
// WebHost.CreateDefaultBuilder(args)
// .UseKestrel()
// .UseContentRoot(Directory.GetCurrentDirectory())
// .UseStartup<startup>()
// .UseIISIntegration()
// .Build();