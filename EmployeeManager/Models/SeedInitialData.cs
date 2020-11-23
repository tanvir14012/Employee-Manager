using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManager.Models
{
    public static class SeedInitialData
    {
        public static async void SeedRolesAndAdmins(IApplicationBuilder app, IConfiguration config)
        {
            RoleManager<IdentityRole> roleManager = app.ApplicationServices
                .CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach(var role in Enum.GetValues(typeof (RoleList)))
            {
                if(! await roleManager.RoleExistsAsync(role.ToString()))
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = role.ToString() });
                }
            }

            UserManager<IdentityUser> userManager = app.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            if(await userManager.FindByEmailAsync(config["Users:Admin:Email"]) == null)
            {
                var user = new IdentityUser
                {
                    Email = config["Users:Admin:Email"],
                    UserName = config["Users:Admin:Email"],
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, config["Users:Admin:Password"]);
                user = await userManager.FindByEmailAsync(user.Email);
                await userManager.AddToRoleAsync(user, config["Users:Admin:Role"]);
            }
        }
    }
}
