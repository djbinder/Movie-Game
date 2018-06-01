using System;
using movieGame.Models;
using Microsoft.AspNetCore.Builder;                 // <--- 'IApplicationBuilder'
using Microsoft.AspNetCore.Hosting;                 // <--- 'IHostingEnvironment'
using Microsoft.EntityFrameworkCore;                // <--- 'UseNpgsql'
using Microsoft.Extensions.Configuration;           // <--- 'IConfiguration' and 'ConfigurationBuilder'
using Microsoft.Extensions.DependencyInjection;     // <--- 'IServiceCollection'
using Microsoft.Extensions.Logging;                 // <--- 'ILoggerFactry'

namespace movieGame {
    public class Startup {
        public IConfiguration Configuration { get; private set; }

        public Startup (IHostingEnvironment env) {
            var builder = new ConfigurationBuilder ()
                .SetBasePath (env.ContentRootPath)
                .AddJsonFile ("appsettings.json", optional : true, reloadOnChange : true)
                .AddEnvironmentVariables ();
            Configuration = builder.Build ();
        }
        public void ConfigureServices (IServiceCollection services) {

            services.AddMvc ();
            services.AddSession ();
            services.AddDbContext<MovieContext> (options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));
        }

        // to swith to dev environment: export ASPNETCORE_ENVIRONMENT="Development"
        public void Configure (IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
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
        }
    }
}
