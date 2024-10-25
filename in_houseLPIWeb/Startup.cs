using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using in_houseLPIWeb.Data;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using in_houseLPIWeb.Utilities;

namespace in_houseLPIWeb
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
            services.AddScoped<UserService>();
            services.AddScoped<AuditChanges>(); //Added Services
            services.AddSession();

            services.AddAuthentication("LawCookieAuth").AddCookie("LawCookieAuth", options =>
            {
                options.Cookie.Name = "LawCookieAuth";
                options.LoginPath = "/userLogin/Login";
                options.AccessDeniedPath = "/userLogin/userDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
            });

            // To disable default property naming policy when using JSON
            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddRazorPages();
            services.AddTransient<ifcDataService>();
            services.AddTransient<IDbConnection>(c => new SqlConnection(Configuration.GetConnectionString("ProductionConnection")));
            services.AddDbContext<WebDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("devConnection")
                )); // Check the database connection in appsettings.json =================================================================
            services.AddTransient<ExcelExportService>();
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireClaim("UserLevel", "0");
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
