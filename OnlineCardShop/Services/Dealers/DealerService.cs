namespace OnlineCardShop.Services.Dealers
{
    using OnlineCardShop.Data;
    using System.Linq;

    public class DealerService : IDealerService
    {
        private readonly OnlineCardShopDbContext data;

        public DealerService(OnlineCardShopDbContext data)
        {
            this.data = data;
        }

        public bool IsDealer(string userId)
        {
            return this.data
                .Dealers
                .Any(d => d.UserId == userId);
        }
    }
}
