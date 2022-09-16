namespace OnlineCardShop.Models.Cards
{
    using OnlineCardShop.Data.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AllCardsQueryModel
    {
        [Display(Name = "Search term")]
        public string SearchTerm { get; init; }

        public CardSorting Sorting { get; init; }

        public IEnumerable<CardListingViewModel> Cards { get; init; }

    }
}
