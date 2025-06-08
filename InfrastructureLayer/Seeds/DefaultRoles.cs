using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Enums;
using InfrastructureLayer.Model;
using Microsoft.AspNetCore.Identity;

namespace InfrastructureLayer.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Roles.Professor.ToString()))
                await roleManager.CreateAsync(new IdentityRole(Roles.Professor.ToString()));

            if (!await roleManager.RoleExistsAsync(Roles.Professor.ToString()))
                await roleManager.CreateAsync(new IdentityRole(Roles.Student.ToString()));
        }


    }
}
