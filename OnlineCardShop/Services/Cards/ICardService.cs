namespace OnlineCardShop.Services.Cards
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

        CardServiceModel CardByUser(int id);

        Image CreateImage(string imageName, string imagePathForDb);

        Card CreateCard(string title, double price, string description, int categoryId, int conditionId, int dealerId, Image newImage);

        IEnumerable<CardCategoryServiceViewModel> GetCardCategories();

        IEnumerable<CardConditionServiceViewModel> GetCardConditions();

    }
}
