namespace OnlineCardShop.Models.Cards
{
    using System.Collections.Generic;

    public class AllCardsQueryModel
    {
        public string SearchTerm { get; init; }

        public IEnumerable<CardListingViewModel> Cards { get; init; }

    }
}
