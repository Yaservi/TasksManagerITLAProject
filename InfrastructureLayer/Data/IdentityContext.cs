using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using InfrastructureLayer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Data
{
    public class IdentityContext: IdentityDbContext<User>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDefaultSchema("Identity");
            builder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
            });

            builder.Entity<IdentityRole>(entity => {
                entity.ToTable("Role");
            });

            builder.Entity<IdentityUserRole<string>>(entity => {
                entity.ToTable("UserRole");

            });
            builder.Entity<IdentityUserLogin<string>>(entity => {

                entity.ToTable("UserLogin");

            });
        }
    }
    
}
