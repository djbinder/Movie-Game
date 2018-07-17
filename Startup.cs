using System;
// using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;  // <--- 'CookieAuthenticationDefaults'
using Microsoft.AspNetCore.Builder;                 // <--- 'IApplicationBuilder'
using Microsoft.AspNetCore.Hosting;                 // <--- 'IHostingEnvironment'
using Microsoft.AspNetCore.Http;                    // <--- 'Pathstring'
// using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;                // <--- 'IdentityRole' and 'AddDefaultTokenParameters'
using Microsoft.AspNetCore.Mvc;                     // <--- anything related to mvc (e.g., 'Controller', '[HttpGet]', 'HttpContext.Session')
using Microsoft.EntityFrameworkCore;                // <--- 'UseNpgsql'
using Microsoft.Extensions.Configuration;           // <--- 'IConfiguration' and 'ConfigurationBuilder'
using Microsoft.Extensions.DependencyInjection;     // <--- 'IServiceCollection'
using Microsoft.Extensions.Logging;                 // <--- 'ILoggerFactory'


using movieGame.Models;

namespace movieGame
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;

        }
        public IConfiguration Configuration { get; private set; }

        public Startup (IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder ()
                .SetBasePath (env.ContentRootPath)
                .AddJsonFile ("appsettings.json", optional : true, reloadOnChange : true);

                // if( env.IsDevelopment() )
                // {
                //     builder.AddUserSecrets();
                // }

            builder.AddEnvironmentVariables ();
            Configuration = builder.Build ();
        }

        // reference: https://github.com/aspnet/Security/issues/1310
        public void ConfigureServices (IServiceCollection services)
        {
            services.AddIdentity<Player, IdentityRole>()
                .AddEntityFrameworkStores<MovieContext>()
                .AddDefaultTokenProviders();

            // services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(o => o.LoginPath = new PathString("/login"))
            //     .AddFacebook(o =>
            //     {
            //         o.AppId = Configuration["facebook:appid"];
            //         o.AppSecret = Configuration["facebook:appsecret"];
            //     });
            services.AddMvc ();
            services.AddSession ();
            services.AddDbContext<MovieContext> (options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));
        }

        // to switch to dev environment: export ASPNETCORE_ENVIRONMENT="Development"
        public void Configure (IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            if (env.IsDevelopment ())
            {
                loggerFactory.AddConsole ();
                app.UseDeveloperExceptionPage ();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }

            else {
                app.UseExceptionHandler("/Error/ShowErrorPage");
            }

            app.UseStaticFiles ();
            app.UseSession ();
            app.UseAuthentication();
            app.UseMvc ();
        }
    }
}