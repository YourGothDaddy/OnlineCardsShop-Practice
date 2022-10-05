namespace OnlineCardShop.Areas.Admin.Services
{
    using OnlineCardShop.Areas.Admin.Services.Cards;

    public interface IAdminCardService
    {
        CardQueryServiceModel All();

        void DeleteCard(int id);

        void ApproveCard(int id);
    }
}
