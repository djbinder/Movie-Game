using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace movieGame 
{
    public class Program 
    {
        public static void Main (string[] args) {
            Console.WriteLine ($"Version: {Environment.Version}");

            if(Environment.Version.ToString() == "Production")
            {
                Console.WriteLine();
                Console.WriteLine("***************************************************");
                Console.WriteLine("SWITCH TO DEV ENVIRONMENT");
                Console.WriteLine("***************************************************");
                Console.WriteLine();
            }
            CreateWebHostBuilder (args).Build ().Run ();
        }

        public static IWebHostBuilder CreateWebHostBuilder (string[] args) =>
            WebHost.CreateDefaultBuilder (args)
            .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "False")
            .UseStartup<Startup> ()
            .ConfigureLogging(logging =>
                    logging.SetMinimumLevel(LogLevel.Warning)
                );
    }
}