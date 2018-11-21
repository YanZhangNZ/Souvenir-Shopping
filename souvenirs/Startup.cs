using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using souvenirs.Data;
using souvenirs.Models;
using souvenirs.Services;
using Microsoft.Extensions.Logging;

namespace souvenirs
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            }
            )
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(5);
                options.CookieHttpOnly = true;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory,IServiceProvider serviceProvider,ApplicationDbContext apContext, UserManager<ApplicationUser> userManager)
        {
            app.UseSession(); 

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            await CreateRoles(serviceProvider);
        }

        public async Task CreateRoles(IServiceProvider serviceProvider)
        {
            using (var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                //create database schema if none exists
                var apContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                // apContext.Database.EnsureCreated();
                apContext.Database.Migrate();

                //If there is already an Administrator role, abort
                var _roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
                //var _userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

                string[] roleNames = { "Admin", "Member" };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {

                    bool roleExist = _roleManager.RoleExistsAsync(roleName).Result;
                    if (!roleExist)
                    {
                        roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    }

                }

                var poweruser = new ApplicationUser
                {
                    UserName = Configuration.GetSection("UserSettings")["UserEmail"],
                    Email = Configuration.GetSection("UserSettings")["UserEmail"],
                    EmailConfirmed = true,
                    Enabled = true,
                   Address = "Addmin Address"
                };
                var _userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                var test = _userManager.FindByEmailAsync(Configuration.GetSection("UserSettings")["UserEmail"]);
                if (test.Result == null)
                {
                    string UserPassword = Configuration.GetSection("UserSettings")["UserPassword"];
                    poweruser.EmailConfirmed = true;
                    var createPowerUser = await _userManager.CreateAsync(poweruser, UserPassword);
                    if (createPowerUser.Succeeded)
                    {
                        //here we tie the new user to the "Admin" role 
                        await _userManager.AddToRoleAsync(poweruser, "Admin");
                    }
                }
            }

        }

    }
}
