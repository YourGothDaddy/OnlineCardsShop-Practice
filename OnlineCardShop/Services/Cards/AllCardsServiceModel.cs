namespace OnlineCardShop.Services.Cards
{
    using OnlineCardShop.Data.Models.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AllCardsServiceModel
    {
        public const int CardsPerPage = 3;

        [Display(Name = "Search term")]
        public string SearchTerm { get; init; }

        public CardSorting Sorting { get; init; }

        public SortingOrder Order { get; init; }

        public int CurrentPage { get; set; } = 1;

        public int TotalCards { get; set; }
        public IEnumerable<CardServiceModel> Cards { get; set; }
    }
}
