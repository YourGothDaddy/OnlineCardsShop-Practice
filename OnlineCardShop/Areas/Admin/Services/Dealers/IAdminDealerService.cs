namespace OnlineCardShop.Areas.Admin.Services.Dealers
{
    public interface IAdminDealerService
    {
        DealerAndReportsServiceModel GetAllReportsOfDealer(int currentPage, int cardsPerPage, int id);
    }
}
