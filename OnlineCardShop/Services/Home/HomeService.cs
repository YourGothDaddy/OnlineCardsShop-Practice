namespace OnlineCardShop.Services.Home
{
    using OnlineCardShop.Data;
    using OnlineCardShop.Services.Cards;
    using System.Linq;

    public class HomeService : IHomeService
    {

        private readonly OnlineCardShopDbContext data;

        public HomeService(OnlineCardShopDbContext data)
        {
            this.data = data;
        }
        public IQueryable<CardCategoryServiceViewModel> GetCategories()
        {
            return this.data
                .Categories
                .Select(c => new CardCategoryServiceViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                });
        }
    }
}
