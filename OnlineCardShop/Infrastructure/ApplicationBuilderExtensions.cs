namespace CardShop.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using static OnlineCardShop.Areas.Admin.AdminConstants;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var services = serviceScope.ServiceProvider;

            MigrateDatabase(services);

            SeedCategories(services);
            SeedConditions(services);
            SeedAdministrator(services);
            
            return app;
        }

        private static void MigrateDatabase(IServiceProvider services)
        {
            var data = services.GetRequiredService<OnlineCardShopDbContext>();

            data.Database.Migrate();
        }

        private static void SeedAdministrator(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            Task
                .Run(async () =>
                {
                    if ( await roleManager.RoleExistsAsync(AdministratorRoleName))
                    {
                        return;
                    }

                    var role = new IdentityRole
                    {
                        Name = AdministratorRoleName
                    };

                    await roleManager.CreateAsync(role);

                    const string adminEmail = "admin@ocs.com";
                    const string adminPassword = "passwordToBeChangedAfterDeployment";

                    var user = new User
                    {
                        Email = adminEmail,
                        UserName = adminEmail,
                        FullName = "Admin"
                    };

                    await userManager.CreateAsync(user, adminPassword);

                    await userManager.AddToRoleAsync(user, role.Name);
                })
                .GetAwaiter()
                .GetResult();
        }

        private static void SeedCategories(IServiceProvider services)
        {
            var data = services.GetRequiredService<OnlineCardShopDbContext>();

            if (data.Categories.Any())
            {
                return;
            }

            data.Categories.AddRange(new[]
            {
                new Category{ Name = "Kpop"},
                new Category{ Name = "Game"}
            });

            data.SaveChanges();
        }

        private static void SeedConditions(IServiceProvider services)
        {
            var data = services.GetRequiredService<OnlineCardShopDbContext>();

            if (data.Conditions.Any())
            {
                return;
            }

            data.Conditions.AddRange(new[]
            {
                new Condition{ Name = "Perfect"},
                new Condition{ Name = "Used"}
            });

            data.SaveChanges();
        }
    }
}
