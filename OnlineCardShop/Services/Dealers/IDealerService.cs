namespace OnlineCardShop.Services.Dealers
{
    using OnlineCardShop.Data.Models;

    public interface IDealerService
    {
        public bool IsDealer(string userId);

        public int GetDealer(string userId);
    }
}
