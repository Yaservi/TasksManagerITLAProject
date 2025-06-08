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
    public class DefaultProfessorRoles
    {
          public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            User ownerUser = new()
            {
                UserName = "Sr_Mike",
                FirstName = "Mike",
                LastName = "Pérez",
                Email = "Mikeperezfeliz203@gmail.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };

            var user = await userManager.FindByEmailAsync(ownerUser.Email);
            if (user == null)
            {
                var result = await userManager.CreateAsync(ownerUser, "Mik30neLe#0");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(ownerUser, Roles.Professor.ToString());
                    await userManager.AddToRoleAsync(ownerUser, Roles.Student.ToString());
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error creando usuario seed: {error.Code} - {error.Description}");
                    }
                }
            }

        }


    }


}
    
