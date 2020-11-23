using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rotativa.AspNetCore;
using EmployeeManager.Models;
using EmployeeManager.Services;
using EmployeeManager.Utilities;
using Wkhtmltopdf.NetCore;
using EmployeeManager.Models.Entity;

namespace EmployeeManager
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config => {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            }).AddXmlSerializerFormatters();

            services.AddDbContextPool<AppDbContext>(options => {
                options.UseSqlServer(_config.GetConnectionString("DbConn"), 
                    serverOps => serverOps.CommandTimeout(1200));
            });

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = true;
                options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirm";

            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<Utilities.EmailTokenProvider<IdentityUser>>("CustomEmailConfirm");

            //services.ConfigureApplicationCookie(options => {
            //    options.AccessDeniedPath = new PathString("/Admin/AccessDenied");
            //});

            services.AddAuthentication()
                .AddGoogle(options => {
                    options.ClientId = ""; //Your google OAuth ClientId
                    options.ClientSecret = "";  //Your google OAuth ClientSecret
                });

            services.Configure<EmailSetting>(_config.GetSection("EmailSetting"));
            services.AddScoped<IEmailSender, EmailService>();
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(1);
            });

            services.Configure<EmailTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(3);
            });

            //services.AddWkhtmltopdf(); //For localhost
            services.AddWkhtmltopdf(System.IO.Path.Combine("wwwroot", "Rotativa")); //For production

            services.AddScoped<IEmployeeCrud, EmployeeCrudDb>();
            services.AddScoped<IRenderViewService, RazorViewRenderService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            RotativaConfiguration.Setup(env);
            SeedInitialData.SeedRolesAndAdmins(app, _config);
        }
    }
}
