namespace OnlineCardShop.Areas.Admin.Services
{
    using OnlineCardShop.Areas.Admin.Services.Cards;

    public interface IAdminCardService
    {
        AllCardsServiceModel All(int currentPage, int cardsPerPage);

        void DeleteCard(int id);

        void ApproveCard(int id);

        void HideCard(int id);
    }
}
