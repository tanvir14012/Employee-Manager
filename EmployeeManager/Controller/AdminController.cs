using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EmployeeManager.Models;

namespace EmployeeManager.Controller
{
    public class AdminController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<IdentityUser> userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Owner, Admin")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> CreateRole(RoleModel model)
        {       
            if (ModelState.IsValid)
            {
                if(model.AllUserIds != null && model.AllUserIds.Count > 0)
                {
                    IdentityResult result = null;
                    IdentityRole identityRole = new IdentityRole { Name = model.Name.ToString() };

                    foreach (var userEmail in model.AllUserIds)
                    {
                        var user = await userManager.FindByEmailAsync(userEmail);
                        if (user != null)
                        {
                            var role = await roleManager.FindByNameAsync(model.Name.ToString());
                            if (role == null)
                            {
                                  await roleManager.CreateAsync(identityRole);
                            }
                            result = await userManager.AddToRoleAsync(user, role.Name);
                        }

                    }

                    if (result.Succeeded)
                    {
                        return RedirectToAction("index", "home");
                    }
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                
            }

            return View(model);
        }
    }
}