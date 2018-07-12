using System;

using Microsoft.AspNetCore.Builder; // <--- 'IApplicationBuilder'
using Microsoft.AspNetCore.Hosting; // <--- 'IHostingEnvironment'
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc; // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using Microsoft.EntityFrameworkCore; // <--- 'UseNpgsql'
using Microsoft.Extensions.Configuration; // <--- 'IConfiguration' and 'ConfigurationBuilder'
using Microsoft.Extensions.DependencyInjection; // <--- 'IServiceCollection'
using Microsoft.Extensions.Logging; // <--- 'ILoggerFactory'

using movieGame.Controllers;
using movieGame.Models;

namespace movieGame
{
    public class Startup
    {


        public IConfiguration Configuration { get; private set; }

        public Startup (IHostingEnvironment env)
        {

            Console.WriteLine("test write");

            var builder = new ConfigurationBuilder ()
                .SetBasePath (env.ContentRootPath)
                .AddJsonFile ("appsettings.json", optional : true, reloadOnChange : true)
                .AddEnvironmentVariables ();
            Configuration = builder.Build ();

            // Complete.ThisMethod();
        }

        public void ConfigureServices (IServiceCollection services)
        {

            // Start.ThisMethod();

            services.AddMvc ();
            services.AddSession ();
            services.AddDbContext<MovieContext> (options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));

            // Complete.ThisMethod();
        }

        // to switch to dev environment: export ASPNETCORE_ENVIRONMENT="Development"
        public void Configure (IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            // Start.ThisMethod();

            if (env.IsDevelopment ())
            {
                loggerFactory.AddConsole ();
                app.UseDeveloperExceptionPage ();
            }

            // if (options == null)
            // {
            //     throw new ArgumentNullException (nameof (options));
            // }

            // if (env.IsProduction())
            // {
            //     app.UseForwardedHeaders(new ForwardedHeadersOptions
            //     {
            //         ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //     });
            // }
            app.UseStaticFiles ();
            app.UseSession ();
            app.UseMvc ();

            // Complete.ThisMethod();
        }
    }
}