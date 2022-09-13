namespace CardShop.Infrastructure
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using System.Linq;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();

            var data = scopedServices.ServiceProvider.GetService<OnlineCardShopDbContext>();

            data.Database.Migrate();

            SeedCategories(data);
            SeedConditions(data);
            
            return app;
        }

        private static void SeedCategories(OnlineCardShopDbContext data)
        {
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

        private static void SeedConditions(OnlineCardShopDbContext data)
        {
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
