﻿namespace OnlineCardShop.Areas.Admin.Services
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
        public AllCardsServiceModel All(int currentPage, int cardsPerPage)
        {
            var totalCards = this.data.Cards;

            var cardsQuery = totalCards
                .Skip((currentPage - 1) * cardsPerPage)
                .Take(cardsPerPage)
                .Select(c => new CardServiceModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    DealerId = c.DealerId,
                    IsDeleted = c.IsDeleted,
                    IsPublic = c.IsPublic
                })
                .ToList();

            var cards = new AllCardsServiceModel();

            cards.Cards = cardsQuery;
            cards.TotalCards = totalCards.Count();
            cards.CurrentPage = currentPage;

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

        public void HideCard(int id)
        {
            var card = this.data
                .Cards
                .Where(c => c.Id == id)
                .FirstOrDefault();

            card.IsPublic = false;

            this.data.SaveChanges();
        }
    }
}
