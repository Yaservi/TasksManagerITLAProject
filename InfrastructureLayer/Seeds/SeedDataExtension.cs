using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastructureLayer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InfrastructureLayer.Seeds
{
    public static class SeedDataExtension
    {

        public static async Task seedDatabaseAsync(this IServiceProvider service)
        {
            using var scope = service.CreateScope();
            var scopedService = scope.ServiceProvider;
            var logger = scopedService.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

            try
            {
                var userManager = scopedService.GetRequiredService<UserManager<User>>();
                var roleManager = scopedService.GetRequiredService<RoleManager<IdentityRole>>();
                await DefaultRoles.SeedAsync(userManager, roleManager);
                await DefaultProfessorRoles.SeedAsync(userManager, roleManager);
                logger.LogInformation("Database seeding completed successfully");

            }
            catch (Exception ex)
            {

                logger.LogError(ex, "Error while seeding roles and users");


            }

        }
    }
}
