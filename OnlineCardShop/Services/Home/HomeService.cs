namespace OnlineCardShop.Services.Home
{
    using Microsoft.EntityFrameworkCore;
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Models.Home;
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

        public StatisticsViewModel GetStatistics()
        {
            var usersCount = this.data
                .Users
                .Count();

            var dealersCount = this.data
                .Dealers
                .Count();

            var kpopCardsCount = this.data
                .Cards
                .Include(x => x.Category)
                .Where(c => c.Category.Name == "Kpop" && c.IsPublic == true)
                .Count();

            var gameCardsCount = this.data
                .Cards
                .Include(x => x.Category)
                .Where(c => c.Category.Name == "Game" && c.IsPublic == true)
                .Count();

            var statistics = new StatisticsViewModel();
            statistics.UsersCount = usersCount;
            statistics.DealersCount = dealersCount;
            statistics.KpopCardsCount = kpopCardsCount;
            statistics.GameCardsCount = gameCardsCount;

            return statistics;
        }
    }
}
