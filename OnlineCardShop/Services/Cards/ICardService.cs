﻿namespace OnlineCardShop.Services.Cards
{
    using OnlineCardShop.Data.Models;
    using OnlineCardShop.Data.Models.Enums;
    using System.Collections.Generic;

    public interface ICardService
    {
        CardQueryServiceModel All(
            string searchTerm,
            CardSorting sorting,
            int currentPage,
            int cardsPerPage,
            int? categoryId,
            SortingOrder? order);

        AllCardsServiceModel ByUser(string userId, int currentPage, int cardsPerPage);

        CardDetailsServiceModel CardByUser(int id, string requestingUserId);

        bool CardIsByDealer(int cardId, int dealerId);

        Image CreateImage(string imageName, string imagePathForDb, string originalImageName);

        Card CreateCard(string title, double price, string description, int categoryId, int conditionId, int dealerId, Image newImage);

        void DeleteCard(int id);

        bool EditCard(int cardId, string title, double price, string description, int categoryId, int conditionId, Image? newImage);

        IEnumerable<CardCategoryServiceViewModel> GetCardCategories();

        IEnumerable<CardConditionServiceViewModel> GetCardConditions();

        public AllCardsServiceModel GetCardsToAddOnPage(AllCardsServiceModel query, CardQueryServiceModel queryResult);

        public bool CategoryExist(int categoryId);

        public bool ConditionExist(int categoryId);

        public void AddImageToDB(Image image);

        public void AddCardToDB(Card card);

        public bool UserIsDealer(string userId);
    }
}
