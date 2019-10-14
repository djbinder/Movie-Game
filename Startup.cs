using System;
using movieGame.Controllers.MixedControllers;
using movieGame.Controllers.PlayerControllers.ManageUsersController;
using movieGame.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using movieGame.Infrastructure;

namespace movieGame
{
    public class Startup
    {
        public Startup (IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        public IConfiguration Configuration { get; private set; }

        public Startup (IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder ()
                .SetBasePath (env.ContentRootPath)
                .AddJsonFile ("appsettings.json", optional : true, reloadOnChange : true);

            builder.AddEnvironmentVariables ();
            Configuration = builder.Build ();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // reference: https://github.com/aspnet/Security/issues/1310
        public void ConfigureServices (IServiceCollection services)
        {
            services.AddTransient<SessionUser>();

            services.AddMvc ().SetCompatibilityVersion (CompatibilityVersion.Version_2_1)
                .AddJsonOptions (options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver ();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddMvc ().AddControllersAsServices ();
            services.AddTransient<GetMovieInfoController> ();
            services.AddTransient<ManageUsersController> ();
            services.AddSingleton<Helpers>();
            services.AddTransient<GetMovieInfoController>();

            // services.AddDistributedMemoryCache ();

            services.AddSession ();

            services.AddDbContext<MovieGameContext> (options => options.UseNpgsql (Configuration["DBInfo:ConnectionString"]));

            Console.WriteLine($"DBInfo:ConnectionString : {Configuration["DBInfo:ConnectionString"]}");
        }

        // to switch to dev environment: export ASPNETCORE_ENVIRONMENT="Development"
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            if (env.IsDevelopment ())
            {
                loggerFactory.AddConsole ();
                app.UseDeveloperExceptionPage ();
                app.UseDatabaseErrorPage ();
            }

            // added for 2.1
            else
            {
                app.UseExceptionHandler ("/Error");
                app.UseHsts ();
            }

            app.UseHttpsRedirection ();
            app.UseStaticFiles ();
            app.UseSession ();
            app.UseAuthentication ();
            app.UseMvc ();
        }
    }
}

