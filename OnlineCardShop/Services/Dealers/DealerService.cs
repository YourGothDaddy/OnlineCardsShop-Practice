namespace OnlineCardShop.Services.Dealers
{
    using OnlineCardShop.Data;
    using OnlineCardShop.Data.Models;
    using System.Linq;

    public class DealerService : IDealerService
    {
        private readonly OnlineCardShopDbContext data;

        public DealerService(OnlineCardShopDbContext data)
        {
            this.data = data;
        }

        public int GetDealer(string userId)
        {
            return this.data
                .Dealers
                .Where(d => d.UserId == userId)
                .Select(d => d.Id)
                .FirstOrDefault();
        }

        public bool IsDealer(string userId)
        {
            return this.data
                .Dealers
                .Any(d => d.UserId == userId);
        }
    }
}
