namespace OnlineCardShop.Services.Cards
{
    using OnlineCardShop.Data.Models.Enums;

    public interface ICardService
    {
        CardQueryServiceModel All(
            string searchTerm,
            CardSorting sorting,
            int currentPage,
            int cardsPerPage,
            int? categoryId,
            SortingOrder? order);
    }
}
