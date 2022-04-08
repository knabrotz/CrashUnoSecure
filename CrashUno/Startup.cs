using CrashUno.Data;
using CrashUno.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrashUno
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder() //
                .AddEnvironmentVariables(); //prefix: "TrafficConnection"

            Configuration = builder.Build(); //
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TrafficContext>(options =>
            {
                //options.UseMySql(Configuration["ConnectionStrings:TrafficConnection"], new MySqlServerVersion(new Version()));
                options.UseMySql(Environment.GetEnvironmentVariable("TrafficConnection"), new MySqlServerVersion(new Version()));
            });
            services.AddDbContext<ApplicationDbContext>(options => {
                //options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")); // ) put this back in later
                options.UseSqlite(Environment.GetEnvironmentVariable("Identity"));
            });

            //services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();

            
            services.AddScoped<IRepository, EFRepository>();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential 
                // cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                // requires using Microsoft.AspNetCore.Http;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddRazorPages();
            //we tried to get hsts half point?
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
                options.ExcludedHosts.Add("is404.net"); 
            });

            services.AddSingleton<InferenceSession>(
                new InferenceSession("wwwroot/traffic.onnx")
            );

            //this is where the password settings change
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 10;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
        
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy-Report-Only", "default-src 'self'");
                await next();

                //context.Response.Headers.Add("Content-Security-Policy-Report-Only", "style-src");
                //await next();

                //context.Response.Headers.Add("Content-Security-Policy-Report-Only", "img-src");
                //await next();

                //context.Response.Headers.Add("Content-Security-Policy-Report-Only", "script-src");
                //await next();

                //context.Response.Headers.Add("HSTS");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("admin", "admin", new { Controller = "Admin", action = "Index" });

                endpoints.MapControllerRoute("typepage",
                    "{crashseverityid}/Page{pageNum}",
                    new { Controller = "Home", action = "Crash" });

                endpoints.MapControllerRoute(
                    name: "Paging",
                    pattern: "Page{pageNum}",
                    defaults: new { Controller = "Home", action = "Index", pageNum = 1 });

                endpoints.MapControllerRoute("type",
                    "{crashseverityid}",
                    new { Controller = "Home", action = "Crash", pageNum = 1 });

                endpoints.MapDefaultControllerRoute();

                endpoints.MapRazorPages();
            });
        }
    }
}
