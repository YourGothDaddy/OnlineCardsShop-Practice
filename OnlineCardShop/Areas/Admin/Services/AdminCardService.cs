namespace OnlineCardShop.Areas.Admin.Services
{
    using OnlineCardShop.Areas.Admin.Services.Cards;
    using OnlineCardShop.Data;
    using System.Linq;

    public class AdminCardService : IAdminCardService
    {
        private readonly OnlineCardShopDbContext data;

        public AdminCardService(OnlineCardShopDbContext data)
        {
            this.data = data;
        }
        public CardQueryServiceModel All()
        {
            var cardsQuery = this.data
                .Cards
                .Select(c => new CardServiceModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    DealerId = c.DealerId,
                    IsDeleted = c.IsDeleted
                })
                .ToList();

            var cards = new CardQueryServiceModel();

            cards.Cards = cardsQuery;

            return cards;
        }

        public void ApproveCard(int id)
        {
            var card = this.data
                .Cards
                .Where(c => c.Id == id)
                .FirstOrDefault();

            card.IsPublic = true;

            this.data.SaveChanges();
        }

        public void DeleteCard(int id)
        {
            var card = this.data
                .Cards
                .Where(c => c.Id == id)
                .FirstOrDefault();

            card.IsDeleted = true;
            card.IsPublic = false;

            this.data.SaveChanges();
        }
    }
}
