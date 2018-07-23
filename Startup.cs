using System;
using System.Net;                                   // <--- 'WebUtility'
using System.Text;                                  // <--- 'StringBuilder'
using System.Text.Encodings.Web;                     // <--- 'HtmlEncoder'
using Microsoft.AspNetCore.Builder;                 // <--- 'IApplicationBuilder'
using Microsoft.AspNetCore.Hosting;                 // <--- 'IHostingEnvironment'
using Microsoft.AspNetCore.Http;                     // <--- 'HttpContext' and 'WriteAsync'
using Microsoft.AspNetCore.Identity;                // <--- 'IdentityRole' and 'AddDefaultTokenParameters'
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

        // This method gets called by the runtime. Use this method to add services to the container.
        // reference: https://github.com/aspnet/Security/issues/1310
        public void ConfigureServices (IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>()
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
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {

            if (env.IsDevelopment ())
            {
                loggerFactory.AddConsole ();
                app.UseDeveloperExceptionPage ();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }

            // //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-2.0
            #region
                app.UseStatusCodePages(async context =>
                {
                    ExtensionsD.Spotlight("mark 1");
                    context.HttpContext.Response.ContentType = "text/plain";

                    await context.HttpContext.Response.WriteAsync(
                        "Status code page, status code: " +
                        context.HttpContext.Response.StatusCode);
                });

                app.UseStatusCodePagesWithRedirects("/error/{0}");
                app.MapWhen(context => context.Request.Path == "/missingpage", builder => { });
                app.Map("/error", error =>
                {
                    ExtensionsD.Spotlight("mark 2");
                    error.Run(async context =>
                    {
                        ExtensionsD.Spotlight("mark 3");
                        var builder = new StringBuilder();
                        builder.AppendLine("<html><body>");
                        builder.AppendLine("An error occurred.<br />");
                        var path = context.Request.Path.ToString();

                        if (path.Length > 1)
                        {
                            ExtensionsD.Spotlight("mark 4");
                            builder.AppendLine("Status Code: " +
                                HtmlEncoder.Default.Encode(path.Substring(1)) + "<br />");
                        }

                        var referrer = context.Request.Headers["referer"];
                        if (!string.IsNullOrEmpty(referrer))
                        {
                            ExtensionsD.Spotlight("mark 5");
                            builder.AppendLine("Return to <a href=\"" +
                                HtmlEncoder.Default.Encode(referrer) + "\">" +
                                WebUtility.HtmlEncode(referrer) + "</a><br />");
                        }

                        ExtensionsD.Spotlight("mark 6");
                        builder.AppendLine("</body></html>");
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync(builder.ToString());
                    });
                });

                // app.Run(async (context) =>
                // {
                //     ExtensionsD.Spotlight("mark 7");

                //     if (context.Request.Query.ContainsKey("throw"))
                //     {
                //         ExtensionsD.Spotlight("mark 8");
                //         throw new Exception("Exception triggered!");
                //     }

                //     ExtensionsD.Spotlight("mark 9");
                //     var builder = new StringBuilder();
                //     builder.AppendLine("<html><body>Hello World!");
                //     builder.AppendLine("<ul>");
                //     builder.AppendLine("<li><a href=\"/?throw=true\">Throw Exception</a></li>");
                //     builder.AppendLine("<li><a href=\"/missingpage\">Missing Page</a></li>");
                //     builder.AppendLine("</ul>");
                //     builder.AppendLine("</body></html>");

                //     context.Response.ContentType = "text/html";
                //     await context.Response.WriteAsync(builder.ToString());
                //     ExtensionsD.Spotlight("mark 10");
                // });
            #endregion

            app.UseStaticFiles ();
            app.UseSession ();
            app.UseAuthentication();
            app.UseMvc ();

        }
    }
}