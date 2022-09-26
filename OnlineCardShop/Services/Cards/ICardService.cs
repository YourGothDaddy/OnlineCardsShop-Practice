namespace OnlineCardShop.Services.Cards
{
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

        AllCardsServiceModel ByUser(string userId);

        CardServiceModel CardByUser(int id);

    }
}
