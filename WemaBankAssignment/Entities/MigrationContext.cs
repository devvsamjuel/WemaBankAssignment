using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WemaBankAssignment.Data;

namespace WemaBankAssignment.Entities
{
    public static class MigrationContext
    {
        public static void RunMigration(this IApplicationBuilder app, UserManager<ApplicationUser> appUser, RoleManager<ApplicationRole> roleManager)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                System.Console.WriteLine("Applying Migrations...");
                Migrate(serviceScope.ServiceProvider.GetService<ApplicationDbContext>());
            }
        }

        public static void SeedRoleData(this IApplicationBuilder app, UserManager<ApplicationUser> appUser, RoleManager<ApplicationRole> roleManager)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                System.Console.WriteLine("Applying Migrations...");
                context.Database.Migrate();

                SeedRoles(roleManager);

                context.SaveChanges();
            }
        }

        public static void SeedRoles(RoleManager<ApplicationRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync(UserRole.SuperAdmin.ToString()).Result)
            {
                ApplicationRole theRole = new ApplicationRole
                {
                    Name = UserRole.SuperAdmin.ToString(),
                    NormalizedName = UserRole.SuperAdmin.ToString().ToUpper(),
                    Description = "This Role is a SuperAdministrator role",
                    Status = StatusType.Active.ToString(),
                    Code = "001",
                };

                IdentityResult roleResult = roleManager.CreateAsync(theRole).Result;
            }

            if (!roleManager.RoleExistsAsync(UserRole.Admin.ToString()).Result)
            {
                ApplicationRole theRole = new ApplicationRole
                {
                    Name = UserRole.Admin.ToString(),
                    NormalizedName = UserRole.Admin.ToString().ToUpper(),
                    Description = "This Role is a Administrator role",
                    Status = StatusType.Active.ToString(),
                    Code = "002",
                };

                IdentityResult roleResult = roleManager.CreateAsync(theRole).Result;
            }

            if (!roleManager.RoleExistsAsync(UserRole.Auditor.ToString()).Result)
            {
                ApplicationRole theRole = new ApplicationRole
                {
                    Name = UserRole.Auditor.ToString(),
                    NormalizedName = UserRole.Auditor.ToString().ToUpper(),
                    Description = "This Role is a Auditor role",
                    Status = StatusType.Active.ToString(),
                    Code = "003",
                };

                IdentityResult roleResult = roleManager.CreateAsync(theRole).Result;
            }

            if (!roleManager.RoleExistsAsync(UserRole.Support.ToString()).Result)
            {
                ApplicationRole theRole = new ApplicationRole
                {
                    Name = UserRole.Admin.ToString(),
                    NormalizedName = UserRole.Support.ToString().ToUpper(),
                    Description = "This Role is a Support role",
                    Status = StatusType.Active.ToString(),
                    Code = "004",
                };

                IdentityResult roleResult = roleManager.CreateAsync(theRole).Result;
            }

            if (!roleManager.RoleExistsAsync(UserRole.Customer.ToString()).Result)
            {
                ApplicationRole theRole = new ApplicationRole
                {
                    Name = UserRole.Customer.ToString(),
                    NormalizedName = UserRole.Customer.ToString().ToUpper(),
                    Description = "This Role is an Customer Role",
                    Status = StatusType.Active.ToString(),
                    Code = "005",
                };
                IdentityResult roleResult = roleManager.CreateAsync(theRole).Result;
            }
        }

        public static void Migrate(ApplicationDbContext context)
        {
            context.Database.Migrate();
        }
    }
}
