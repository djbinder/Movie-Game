using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Threading;
// using System.Threading.Tasks;
using movieGame.Models;
using Microsoft.AspNetCore.Builder;                 // <--- 'IApplicationBuilder'
using Microsoft.AspNetCore.Hosting;                 // <--- 'IHostingEnvironment'
using Microsoft.EntityFrameworkCore;                // <--- 'UseNpgsql'
using Microsoft.Extensions.Configuration;           // <--- 'IConfiguration' and 'ConfigurationBuilder'
using Microsoft.Extensions.DependencyInjection;     // <--- 'IServiceCollection'
using Microsoft.Extensions.Logging;                 // <--- 'ILoggerFactory'









namespace movieGame {
    public class Startup {
        public IConfiguration Configuration { get; private set; }



        public Startup (IHostingEnvironment env) {

            Console.WriteLine();
            Console.WriteLine("---------------'STARTUP METHOD' STARTED---------------");

            var builder = new ConfigurationBuilder ()
                .SetBasePath (env.ContentRootPath)
                .AddJsonFile ("appsettings.json", optional : true, reloadOnChange : true)

                .AddEnvironmentVariables ();
            Configuration = builder.Build ();


            Console.WriteLine("---------------'STARTUP METHOD' COMPLETED---------------");
            Console.WriteLine();
        }
        public void ConfigureServices (IServiceCollection services) {

            Console.WriteLine();
            Console.WriteLine("---------------'CONFIGURE SERVICES METHOD' STARTED---------------");

            services.AddMvc ();
            services.AddSession ();
            services.AddDbContext<MovieContext> (options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));


            Console.WriteLine("---------------'CONFIGURE SERVICES METHOD' COMPLETED---------------");
            Console.WriteLine();
        }

        // to switch to dev environment: export ASPNETCORE_ENVIRONMENT="Development"
        public void Configure (IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            Console.WriteLine();
            Console.WriteLine("---------------'CONFIGURE METHOD' STARTED---------------");

            if( env.IsDevelopment() )
            {
                loggerFactory.AddConsole ();
                app.UseDeveloperExceptionPage ();
                app.UseStaticFiles ();
                app.UseSession ();
                app.UseMvc ();
            }

            else {
                app.UseStaticFiles ();
                app.UseSession ();
                app.UseMvc ();
            }

            Console.WriteLine("---------------'CONFIGURE METHOD' COMPLETED---------------");
            Console.WriteLine();

        }
    }
}
