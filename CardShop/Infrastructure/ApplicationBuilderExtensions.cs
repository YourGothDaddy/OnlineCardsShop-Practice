namespace CardShop.Infrastructure
{
    using CardShop.Data;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder PrepareDatabase(
            this IApplicationBuilder app)
        {
            using var scopedServices = app.ApplicationServices.CreateScope();

            var data = scopedServices.ServiceProvider.GetService<CardShopDbContext>();

            data.Database.Migrate();
            
            return app;
        }
    }
}
