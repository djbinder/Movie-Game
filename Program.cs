using System;
using Microsoft.AspNetCore;             // <--- 'WebHost'
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;     // <--- 'IWebHost' and '.UseStartup<Startup>'
using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Logging;                 // <--- 'ILoggerFactory'




namespace movieGame {

    public class Program {

        static string Start = "STARTED";
        // static string Complete = "COMPLETED";

        public static void Main (string[] args) {
            Start.ThisMethod();
            Console.WriteLine($"Version: {Environment.Version}");
            BuildWebHost (args).Run ();

            // https://www.nginx.com/blog/tutorial-proxy-net-core-kestrel-nginx-plus/
                // var host = new WebHostBuilder()
                //     .UseKestrel()
                //     .Configure(app =>
                //     {
                //         app.Run(async (context) => await
                //         context.Response.WriteAsync($"Current date: {DateTime.Now}"));
                //     })
                //     .Build();
                // host.Run();
                // host.Intro("host");

            // Complete.ThisMethod();
        }

        public static IWebHost BuildWebHost (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .UseStartup<Startup> ()
            .Build ();
            // .UseUrls(urls: "http://localhost:10003")

    }
}
